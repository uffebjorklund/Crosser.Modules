namespace Crosser.EdgeNode.Modules
{

    using Crosser.EdgeNode.Common.Abstractions.Models;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Errors;
    using Crosser.EdgeNode.Core.Abstractions.Models.Net.Mqtt;
    using Crosser.EdgeNode.Core.Abstractions.Net.Mqtt;
    using Crosser.EdgeNode.Flows.Abstractions.Models.Messaging;
    using Crosser.EdgeNode.Flows.Abstractions;
    using Crosser.EdgeNode.Flows.Models.Abstractions.Models;
    using Crosser.EdgeNode.Flows;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    public class NodeJsBridgeModule : FlowModule<NodeJsBridgeModuleSettings>
    {
        public override string UserFriendlyName { get; } = "NodeJs Bridge";

        private Process nodeProcess;
        private IMqttBrokerService MqttBroker;

        private string InstanceId;

        public NodeJsBridgeModule(IMqttBrokerService brokerService) : base(FlowModuleType.Function)
        {
            this.MqttBroker = brokerService;
            this.ModuleProtocol = FlowModuleProtocol.MQTT;

            this.Categories.Add("NodeJs");
            this.Categories.Add("code");
            this.Categories.Add("Runtime");
            this.Categories.Add("Bridge");
        }

        protected override async Task MessageReceived(IFlowMessage msg)
        {
            // We might get
            // 1: a message that is supposed to go to the NodeJs code
            // 2: a message that was sent back from the NodeJs code (pass to next module)

            if (msg.HasOwnProperty<string>("topic") && msg.GetValue<string>("topic").StartsWith($"{this.InstanceId}/"))
            {
                if (msg.GetValue<string>("topic") == $"{this.InstanceId}/out")
                {
                    var outMsg = new FlowMessage();
                    var json = Encoding.UTF8.GetString(msg.GetValue<byte[]>("data"));
                    // Pass to next module
                    if (string.IsNullOrEmpty(this.Settings.TargetProperty))
                    {
                        outMsg = FlowMessageExtensions.ToFlowMessage(FlowMessageExtensions.Deserialize(json));
                    }
                    else
                    {
                        outMsg[this.Settings.TargetProperty] = FlowMessageExtensions.Deserialize(json);
                    }
                    await this.Next(outMsg);
                }
                else if (msg.GetValue<string>("topic") == $"{this.InstanceId}/debug")
                {
                    // Send as debug info (events???)
                    this.OnDebug?.Invoke(this, ModuleDebugMessage.Create(this.FlowId, this.Id, msg, ModuleMessageDirection.Out));
                }
            }
            else
            {
                // Send to NodeJs
                dynamic m = msg;
                var b = new byte[0];
                if (msg.HasOwnProperty(this.Settings.SourceProperty))
                {
                    m = msg.GetValue<dynamic>(this.Settings.SourceProperty);
                }

                switch (m)
                {
                    case string str:
                        b = Encoding.UTF8.GetBytes(str);
                        break;
                    case byte[] bytes:
                        b = bytes;
                        break;
                    default:
                        // best effort
                        var json = FlowMessageExtensions.Serialize(m);
                        b = Encoding.UTF8.GetBytes(json);
                        break;
                }
                await this.MqttBroker.Publish(new MqttPubMsg { Message = b, Topic = $"{this.InstanceId}/in" });
            }
        }

        public override async Task<IError> Initialize()
        {
            try
            {
                IError err = null;
                this.Topic = $"{this.Id}/#";
                this.InstanceId = $"{this.Id}/{Guid.NewGuid().ToString("N")}";
                if (File.Exists($"./{this.Id}.js"))
                {
                    File.Delete($"./{this.Id}.js");
                }

                if (!File.Exists(this.Settings.NodeJsPath))
                {
                    return new Error($"NodeJs was not found at the configured path: `{this.Settings.NodeJsPath}`");
                }

                if (!File.Exists(this.Settings.NpmPath))
                {
                    return new Error($"NPM was not found at the configured path: `{this.Settings.NpmPath}`");
                }

                err = this.InstallPackages();
                if (err != null)
                {
                    return err;
                }

                using (var fw = File.CreateText($"./{this.Id}.js"))
                {
                    await fw.WriteAsync(@"import sys
sys.path.append('data/flowresources')");
                    await fw.WriteLineAsync();
                    await fw.WriteAsync(this.GetMainCode3().Replace("\t", "    "));
                    await fw.WriteLineAsync();
                    await fw.WriteAsync(this.GetCrosserCommunicationCode3());
                    await fw.FlushAsync();
                }

                return await this.SetupConnection();
            }
            catch (Exception ex)
            {
                return new Error(ex.Message);
            }
        }

        public IError InstallPackages()
        {
            var packageLines = this.Settings.Packages.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var packages in packageLines)
            {
                if (packages.Trim().StartsWith("#") || packages.Trim() == string.Empty) continue;

                foreach (var package in packages.Split(','))
                {
                    var err = this.InstallPackage(package.Trim());
                    if (err != null)
                    {
                        return err;
                    }
                }
            }
            return null;
        }

        public IError InstallPackage(string package)
        {
            try
            {
                var errorBuilder = new StringBuilder();
                var lastError = string.Empty;
                bool hasError = false;
                using (var process = new Process())
                {
                    process.EnableRaisingEvents = true;
                    process.StartInfo = new ProcessStartInfo(this.Settings.NpmPath)
                    {
                        Arguments = $" install {package}",
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data == null)
                        {
                            if (!hasError) return;

                            // Err completed, send and reset
                            lastError = $"NPM package install error: {errorBuilder.ToString()}";
                            errorBuilder.Clear();
                            hasError = false;
                        }
                        else
                        {
                            hasError = true;
                            errorBuilder.AppendLine(e.Data);
                        }
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (lastError != string.Empty)
                    {
                        return new Error(lastError);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return new Error(ex.Message);
            }
        }

        public override Task Stop()
        {
            try
            {
                if (nodeProcess != null && !nodeProcess.HasExited)
                {
                    nodeProcess.Kill();
                }
                if (File.Exists($"./{this.Id}.js"))
                {
                    File.Delete($"./{this.Id}.js");
                }
                return base.Stop();
            }
            catch
            {
                return base.Stop();
            }
        }
        bool hasError = false;
        StringBuilder errorBuilder = new StringBuilder();
        private object errorLocker = new object();
        public async Task<IError> SetupConnection()
        {
            try
            {
                var connected = false;
                this.nodeProcess = new Process();
                this.nodeProcess.EnableRaisingEvents = true;
                this.nodeProcess.StartInfo = new ProcessStartInfo(this.Settings.NodeJsPath)
                {
                    Arguments = $"-u ./{this.Id}.js", // + " " + string.Join(" ",args),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                this.nodeProcess.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        if (e.Data.StartsWith("CrosserNodeJsError: "))
                        {
                            this.HandleError(e);
                            this.HandleError(null);
                        }
                        else
                        {
                            if (e.Data == "connected")
                            {
                                connected = true;
                            }
                            else if (this.Debug)
                            {
                                var m = new FlowMessage();
                                m["data"] = e.Data;
                                this.OnDebug?.Invoke(this, ModuleDebugMessage.Create(this.FlowId, this.Id, m, ModuleMessageDirection.Log));
                            }
                        }
                    }
                };

                this.nodeProcess.Exited += (s, e) =>
                {

                };

                this.nodeProcess.ErrorDataReceived += (s, e) =>
                {
                    this.HandleError(e);
                };

                if (!this.nodeProcess.Start())
                {
                    throw new Exception("NodeJS process could not be started");
                }

                this.nodeProcess.BeginOutputReadLine();
                this.nodeProcess.BeginErrorReadLine();

                if (!SpinWait.SpinUntil(() => connected == true, 5000))
                {
                    return new Error("No connection was made from NodeJS within the timeout");
                }

                return await FlowModule.NoError;
            }
            catch (Exception ex)
            {
                return new Error(ex.Message);
            }
        }

        private void HandleError(DataReceivedEventArgs e)
        {
            lock (errorLocker)
            {
                if (e == null || e.Data == null)
                {
                    if (!hasError) return;

                    // Err completed, send and reset
                    var err = $"NodeJS Bridge Error: {errorBuilder.ToString()}";
                    errorBuilder.Clear();
                    this.SetStatus(Status.Error, err);
                    hasError = false;
                }
                else
                {
                    hasError = true;
                    errorBuilder.AppendLine(e.Data);
                }
            }
        }
        private string GetCrosserCommunicationCode3()
        {
            return $@"
import paho.mqtt.client as mqtt
import sys, traceback
import json
try:
	import thread
except ImportError:
	import _thread as thread

class Client:
	def __init__(self, broker_host, broker_port, id):
		self.in_topic = id + '/in'
		self.out_topic = id + '/out'
		self.dbg_topic = id + '/debug'
		self.msg_handler = msg_handler
		self.mqtt_client = mqtt.Client(client_id=id, clean_session=True, userdata=None, protocol=mqtt.MQTTv311, transport='tcp')
		self.mqtt_client.on_message = self.on_message
		self.mqtt_client.on_connect = self.on_connect
		self.mqtt_client.on_disconnect = self.on_disconnect
		self.mqtt_client.connect(broker_host, broker_port)
		self.mqtt_client.loop_forever()

	def on_connect(self, client, userdata, flags, rc):
		self.mqtt_client.subscribe(self.in_topic)
		print('connected')
		try:
			thread.start_new_thread(initialize, (self,))
		except:
			pass

	def on_disconnect(self, client, userdata, rc):
		self.mqtt_client.disconnect()

	def on_message(self, client, userdata, msg):
		try:
			self.msg_handler(json.loads(msg.payload), self)
		except Exception as e:
			print('CrosserPythonError: %s' % e)
			sys.exit()

	def debug(self, data):
		self.mqtt_client.publish(self.dbg_topic, data)

	def send(self, data):
		self.publish(data)

	def next(self, data):
		self.publish(data)

	def publish(self, data):
		self.mqtt_client.publish(self.out_topic, json.dumps(data))

def main():
	global module
	module = Client('127.0.0.1', 1883, '{this.InstanceId}')

if __name__== '__main__':
	main()
";
        }

        private string GetMainCode3()
        {
            return this.Settings.OnMessage;
        }
    }

}
