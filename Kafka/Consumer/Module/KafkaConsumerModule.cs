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
    }
}
