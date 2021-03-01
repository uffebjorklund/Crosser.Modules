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


    [Serializable]
    public class KafkaModuleException : Exception
    {
        public KafkaModuleException(string message):base(message){}
    }

    public class KafkaProducerModule : FlowModule<KafkaProducerModuleSettings>
    {
        private IProducer<Null, string> producer;
        public override string UserFriendlyName => "Kafka Producer";

        private string username;
        private string password;

        public KafkaProducerModule() : base(FlowModuleType.Output) { }

        protected override async Task MessageReceived(IFlowMessage msg)
        {
            try
            {
                if(!msg.HasOwnProperty(this.Settings.SourceProperty))
                {
                    msg.SetError($"Could not locate the `Source Property` {this.Settings.SourceProperty} on the incoming message");
                    return;
                }

                var topic = this.Settings.Topic;
                if(msg.HasOwnProperty<string>(topic))
                {
                    topic = msg.GetValue<string>(topic);
                }
                var kafkaMessage = new Message<Null, string> { Value = FlowMessageExtensions.Serialize(msg.GetValue<dynamic>(this.Settings.SourceProperty)) };
                var result = await this.producer.ProduceAsync(topic, kafkaMessage);

                if(result.Status == PersistenceStatus.Persisted)
                {
                    msg.SetSuccess();
                }
                else
                {
                    msg.SetError($"Kafka producer message was not confirmed as persisted: Status = {result.Status}");
                }
            }
            catch (Exception ex)
            {
                msg.SetError($"Kafka producer error: {ex.Message}");
            }
            finally
            {
                await this.Next(msg);
            }
        }

        private void SetSecurity()
        {
            if (this.Settings.SaslCredentials.HasValue && this.Settings.Credentials.ContainsKey(this.Settings.SaslCredentials.Value))
            {
                var credentials = this.Settings.Credentials[this.Settings.SaslCredentials.Value];
                switch (credentials.CredentialType)
                {
                    case Credential.Types.UsernameAndPassword:
                        this.SetSaslUsernamePassword(credentials.ToCredential<CredentialWithUsernamePassword>());
                        return;
                    // TODO: Add support for certificate here in the next version
                    default:
                        throw new KafkaModuleException($"CredentialsType {credentials.CredentialType} not supported");
                }
            }
        }

        public void SetSaslUsernamePassword(CredentialWithUsernamePassword credentialWithUsernamePassword)
        {
            this.SetSaslUsernamePassword(credentialWithUsernamePassword.Username, credentialWithUsernamePassword.Password);
        }
        public void SetSaslUsernamePassword(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public override Task<IError> Initialize()
        {
            this.SetSecurity();

            var conf = new ProducerConfig { BootstrapServers = this.Settings.Brokers };
            conf.SaslMechanism = this.Settings.Mechanism;
            conf.SaslUsername = this.username;
            conf.SaslPassword = this.password;
            if(this.Settings.UseSSL)
            {
                conf.SecurityProtocol = SecurityProtocol.SaslSsl;
            }
            conf.EnableSslCertificateVerification = this.Settings.VerifyServerCertificate;
            if(!string.IsNullOrEmpty(this.Settings.CACertificateLocation))
            {
                conf.SslCaLocation = this.Settings.CACertificateLocation;
            }

            this.producer = new ProducerBuilder<Null, string>(conf).Build();
            return base.Initialize();
        }
    }
}
