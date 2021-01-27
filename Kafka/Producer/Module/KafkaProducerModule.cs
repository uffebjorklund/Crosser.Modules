namespace Crosser.EdgeNode.Modules.Kafka.Producer
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Crosser.EdgeNode.Common.Abstractions.Models;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Errors;
    using Crosser.EdgeNode.Flows;
    using Crosser.EdgeNode.Flows.Abstractions;
    using Crosser.EdgeNode.Flows.Models.Abstractions.Models;

    public class KafkaProducerModule : FlowModule<KafkaProducerModuleSettings>
    {
        private IProducer<Null, string> producer;
        public override string UserFriendlyName => "Kafka Producer";

        public KafkaProducerModule() : base(FlowModuleType.Output) { }

        protected override async Task MessageReceived(IFlowMessage msg)
        {
            try
            {
                var result = await this.producer.ProduceAsync("s5d917ng-default", new Message<Null, string> { Value = FlowMessageExtensions.Serialize(msg) });
                if (result.Status == PersistenceStatus.Persisted)
                {
                    msg.SetSuccess();
                }
                else
                {
                    msg.SetError($"Kafka message was not persisted");
                }
            }
            catch (Exception ex)
            {
                msg.SetError(ex.Message);
            }
            finally
            {
                await this.Next(msg);
            }
        }

        public override Task<IError> Initialize()
        {
            // var conf = new ProducerConfig();
            // conf.BootstrapServers = "rocket-01.srvs.cloudkafka.com:9094,rocket-02.srvs.cloudkafka.com:9094,rocket-03.srvs.cloudkafka.com:9094";
            // conf.SaslUsername = "s5d917ng";
            // conf.SaslPassword = "hiP17smX9Cv_cfc9QLScNK9PwVC9jZpI";
            // conf.SaslMechanism = SaslMechanism.ScramSha512;
            // conf.SecurityProtocol = SecurityProtocol.SaslSsl;

            this.producer = new ProducerBuilder<Null, string>(this.GetProducerConfiguration()).Build();
            return base.Initialize();
        }

        private ProducerConfig GetProducerConfiguration()
        {
            var conf = new ProducerConfig();
            conf.BootstrapServers = this.Settings.Servers;
            conf.SaslMechanism = this.Settings.SaslMechanism;
            conf.SecurityProtocol = this.Settings.SecurityProtocol;
            if (this.Settings.KafkaCredentials.HasValue && this.Settings.Credentials.ContainsKey(this.Settings.KafkaCredentials.Value))
            {
                var credentials = this.Settings.Credentials[this.Settings.KafkaCredentials.Value];
                switch (credentials.CredentialType)
                {
                    case Credential.Types.UsernameAndPassword:
                        var usernamePasswordCredential = credentials.ToCredential<CredentialWithUsernamePassword>();
                        conf.SaslUsername = usernamePasswordCredential.Username;
                        conf.SaslPassword = usernamePasswordCredential.Password;
                        break;
                    case Credential.Types.Certificate:
                        throw new NotImplementedException();
                }
            }
            return conf;
        }
    }
}
