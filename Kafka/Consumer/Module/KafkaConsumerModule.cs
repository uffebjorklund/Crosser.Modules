namespace Crosser.EdgeNode.Modules.Kafka.Consumer
{
    using System;
    using System.Threading.Tasks;
    using Crosser.EdgeNode.Common.Abstractions.Models;
    using Crosser.EdgeNode.Flows;
    public class KafkaConsumerModule : FlowModule<KafkaConsumerModuleSettings>
    {
        public override string UserFriendlyName => "Kafka Consumer";

        public KafkaConsumerModule() : base(FlowModuleType.Input) { }

        protected override Task MessageReceived(IFlowMessage msg)
        {
            throw new NotImplementedException();
        }

        // private void SetSecurity()
        // {
        //     if (this.Settings.OpcCredentials.HasValue && this.Settings.Credentials.ContainsKey(this.Settings.OpcCredentials.Value))
        //     {
        //         var credentials = this.Settings.Credentials[this.Settings.OpcCredentials.Value];
        //         switch (credentials.CredentialType)
        //         {
        //             case Credential.Types.UsernameAndPassword:
        //                 var usernamePasswordCredential = credentials.ToCredential<CredentialWithUsernamePassword>();
        //                 this.LoadUser(usernamePasswordCredential.Username, usernamePasswordCredential.Password);
        //                 break;
        //             case Credential.Types.Certificate:
        //                 var certificateCredential = credentials.ToCredential<CredentialWithCertificate>();
        //                 this.LoadCertificate(certificateCredential.X509Certificate2);
        //                 break;
        //         }
        //     }
        // }
    }
}
