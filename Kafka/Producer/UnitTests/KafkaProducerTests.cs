using Confluent.Kafka;
using Crosser.EdgeNode.Flows.Abstractions;
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
            var conf = new ConsumerConfig();
            conf.BootstrapServers = "rocket-01.srvs.cloudkafka.com:9094";
            conf.SaslUsername = "s5d917ng";
            conf.SaslPassword = "hiP17smX9Cv_cfc9QLScNK9PwVC9jZpI";
            conf.AutoOffsetReset = AutoOffsetReset.Earliest;
            conf.SaslMechanism = SaslMechanism.ScramSha512;
            conf.SecurityProtocol = SecurityProtocol.SaslSsl;
            conf.GroupId = "test-consumer-group";
            this.consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
        }


        [Fact]
        public async Task CanProduceToKafka()
        {
            SetupConsumer();
            var module = new KafkaProducerModule();
            module.Settings.SecurityProtocol = SecurityProtocol.SaslSsl;
            module.Settings.SaslMechanism = SaslMechanism.ScramSha256;
            var credentialId = Guid.NewGuid();
            var credential = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { Username = "s5d917ng", Password = "hiP17smX9Cv_cfc9QLScNK9PwVC9jZpI" }));
            module.Settings.Credentials.Add(credentialId, new Credential(credentialId, credential, "", null, Credential.Types.UsernameAndPassword));
            module.Settings.KafkaCredentials = credentialId;

            dynamic msg = new FlowMessage();
            msg.say = "Hello World";

            var initError = await module.Initialize();
            Assert.Null(initError);

            var startError = await module.Start();
            Assert.Null(startError);

            this.consumer.Subscribe("s5d917ng-default");

            await module.Receive(msg);

            Assert.True(msg.GetValue<bool>("crosser.success"));

            var cr = this.consumer.Consume(10000);
            Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");

            Assert.Equal("{\"say\":\"Hello World\"}", cr.Message.Value);
        }
    }
}