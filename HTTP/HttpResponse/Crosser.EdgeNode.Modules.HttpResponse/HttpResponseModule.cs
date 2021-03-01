
namespace Crosser.EdgeNode.Modules.HttpResponse
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Crosser.EdgeNode.Common.Abstractions.Models;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Errors;
    using Crosser.EdgeNode.Common.Utilities.Binary;
    using Crosser.EdgeNode.Flows;
    using Google.Protobuf;
    using Grpc.Net.Client;

    public class UnixDomainSocketConnectionFactory
    {
        private readonly EndPoint _endPoint;

        public UnixDomainSocketConnectionFactory(EndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
            CancellationToken cancellationToken = default)
        {
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

            try
            {
                await socket.ConnectAsync(_endPoint, cancellationToken);//.ConfigureAwait(false);
                return new NetworkStream(socket, true);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }
    }

    public class HttpResponseModule : FlowModule<HttpResponseModuleSettings>
    {
        public override string UserFriendlyName => "Http Response";

        Crosser.Ipc.HttpModuleMessages.HttpModuleMessagesClient ipcClient;
        private GrpcChannel channel;
        public static readonly string SocketPath = Path.Combine(Path.GetTempPath(), "crosseripc.tmp");

        private static GrpcChannel CreateChannel()
        {
            var udsEndPoint = new UnixDomainSocketEndPoint(SocketPath);
            var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
            var socketsHttpHandler = new SocketsHttpHandler
            {
                ConnectCallback = connectionFactory.ConnectAsync,
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                EnableMultipleHttp2Connections = true
            };

            return GrpcChannel.ForAddress($"http://unix:/{SocketPath}", new GrpcChannelOptions
            {
                HttpHandler = socketsHttpHandler
            });
        }

        public HttpResponseModule():base(FlowModuleType.Output)
        {
            this.Categories.Add("http");
            this.Categories.Add("response");
        }

        protected override async Task MessageReceived(IFlowMessage msg)
        {
            Crosser.Ipc.HttpResponseMessage response = new ();
            response.RequestId = msg.GetValue<string>("data.requestId");
            response.Data = ByteString.CopyFrom(this.Ok(System.Text.Encoding.UTF8.GetBytes(msg.GetValue<string>("data.body")), "application/json"));

            var result = await this.ipcClient.HttpResponseAsync(response);

            if(result.Success)
            {
                msg.SetSuccess();
            }
            else
            {
                msg.SetError(result.Message);
            }

            await this.Next(msg);
        }

        public override async Task<IError> Initialize()
        {
            try
            {
                this.channel = CreateChannel();
                this.ipcClient = new Crosser.Ipc.HttpModuleMessages.HttpModuleMessagesClient(this.channel);
                return await base.Initialize();
            }
            catch (Exception ex)
            {
                return new Error(ex.Message);
            }
        }

        private byte[] Ok(byte[] data, string mimetype, string origin  = null)
        {
            var response = new StringBuilder();
            // Write headers
            response.Append($"HTTP/1.1 200 OK\r\n");
            response.Append("Date: " + DateTime.Now + "\r\n");
            response.Append("Connection: Keep-Alive\r\n");
            response.Append("Server: Crosser\r\n");
            response.Append("Content-Type: " + mimetype + "\r\n");
            if (origin is not null)
            {
                response.Append($"Access-Control-Allow-Origin: {origin}\r\n");
            }
            response.Append("Content-Length: " + data.Length + "\r\n\r\n");
            // Append body
            var payload = new List<byte>();
            payload.AddRange(response.ToString().ToUTF8Bytes());
            payload.AddRange(data);
            return payload.ToArray();
        }
    }

}
