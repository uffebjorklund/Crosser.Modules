namespace Crosser.EdgeNode.Modules.Kafka.Producer
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Crosser.EdgeNode.Common.Abstractions.Models;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Errors;
    using Crosser.EdgeNode.Flows;
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
                // Console.WriteLine(!r.Error.IsError
                //     ? $"Delivered message to {r.TopicPartitionOffset}"
                //     : $"Delivery Error: {r.Error.Reason}");

                var result = await this.producer.ProduceAsync("foo", new Message<Null, string> { Value = FlowMessageExtensions.Serialize(msg) });
                msg.SetSuccess();
                //this.producer.Flush(TimeSpan.FromSeconds(10));
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
            var conf = new ProducerConfig { BootstrapServers = "localhost:9092" };
            this.producer = new ProducerBuilder<Null, string>(conf).Build();
            return base.Initialize();
        }
    }
}
