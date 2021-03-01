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
        [Fact]
        public async Task CanProduceToKafka()
        {
            var module = new KafkaProducerModule();
            module.Settings.Brokers = "kafka-2abf04ee-crossertech-3ec3.aivencloud.com:11642";
            module.Settings.VerifyServerCertificate = false;

            module.Settings.UseSSL = true;
            module.Settings.Mechanism = SaslMechanism.Plain;
            module.SetSaslUsernamePassword("avnadmin", "tmxw3flnyohta3jo");
            module.Settings.Topic = "foo";
            module.Settings.SourceProperty = "say";

            dynamic msg = new FlowMessage();
            msg.say = "Hello World";

            var initError = await module.Initialize();
            Assert.Null(initError);

            var startError = await module.Start();
            Assert.Null(startError);

            await module.Receive(msg);

            Assert.True(msg.GetValue<bool>("crosser.success"));
        }
    }
}