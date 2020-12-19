using Confluent.Kafka;
using Crosser.EdgeNode.Flows.Models.Abstractions.Models;
using Crosser.EdgeNode.Modules.Kafka.Producer;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Crosser.EdgeNode.Modules.KafkaProducer.UnitTests
{
    [CollectionDefinition("KafkaProducerTests Collection")]
    [Collection("KafkaProducerTests Collection")]
    public class KafkaProducerTests
    {
        private IConsumer<Ignore, string> consumer;
        private void SetupConsumer()
        {
            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            this.consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
        }


        [Fact]
        public async Task CanProduceToKafka()
        {
            SetupConsumer();
            var module = new KafkaProducerModule();

            dynamic msg = new FlowMessage();
            msg.say = "Hello World";

            var initError = await module.Initialize();
            Assert.Null(initError);

            var startError = await module.Start();
            Assert.Null(startError);

            this.consumer.Subscribe("foo");

            await module.Receive(msg);

            Assert.True(msg.GetValue<bool>("crosser.success"));

            var cr = this.consumer.Consume(10000);
            Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");

            Assert.Equal("{\"say\":\"Hello World\"}", cr.Message.Value);
        }
    }
}