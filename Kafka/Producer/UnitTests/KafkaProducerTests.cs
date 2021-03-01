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
        //private IConsumer<Ignore, string> consumer;
        // private void SetupConsumer()
        // {
        //     var conf = new ConsumerConfig
        //     {
        //         GroupId = "test-consumer-group",
        //         BootstrapServers = "kafka-2abf04ee-crossertech-3ec3.aivencloud.com:11642",
        //         AutoOffsetReset = AutoOffsetReset.Earliest,

        //     };
        //     this.consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
        // }

        [Fact]
        public async Task A()
        {
            var kafka = new ProducerConfig();
            kafka.BootstrapServers = "kafka-2abf04ee-crossertech-3ec3.aivencloud.com:11631";
            kafka.EnableSslCertificateVerification = true;
            kafka.SecurityProtocol = SecurityProtocol.Ssl;
            kafka.SslCertificatePem = @"-----BEGIN CERTIFICATE-----
MIIEQDCCAqigAwIBAgIUSOihjlKGSN6rvkyFwb+LZni7zBYwDQYJKoZIhvcNAQEM
BQAwOjE4MDYGA1UEAwwvYzhhMmYyMzYtNzAzZS00ODAxLWJiYmYtZmY5ZmViYjc1
MzI2IFByb2plY3QgQ0EwHhcNMjEwMjI3MDkyODI4WhcNMjMwNTI4MDkyODI4WjA/
MRcwFQYDVQQKDA5rYWZrYS0yYWJmMDRlZTERMA8GA1UECwwIdTNzM2c5dDcxETAP
BgNVBAMMCGF2bmFkbWluMIIBojANBgkqhkiG9w0BAQEFAAOCAY8AMIIBigKCAYEA
6Twk25ORgr7BKtxz60463COeRxAsmjaao5YUCmuwE4cpzLo9w2VwDLnZ8sLr0UGm
QDSuIMIuYHUTtxNPjPmfUByyQf7HOHxdUnOX1FBymWP919SkAXg8vZhS+XuFn73D
s69jpqE5TyyUiomgPBL41dlOFR8wD+qMjBJw+9sANsnGgID7HQeiin9Ye64TeYJj
X4o3m+nDXUJxGYn92ivowUKftn1wLKcHr8QTDHB+hnseNmZh78AhrzLalZW0qcv3
bf6NOnvmwfgs1zmF/ffKJM9ILWTpVlk04mxY+YgYXOx+iBg2zOp+mnEwXLjrs0xe
Bf8L2iBAXJglp2yzTQRgIll+NRNqMDQiEzki8WhF95z8m3klUczAxjcMhsEAHd0k
6Iny7SNb/Pv6B93uzFT9+GMji81v/meAu7yhLPfZVOCWGNkc6g8E8P30FQMLRHFz
L4c0eD4Qc/QLqiA60Kbxjg6y5GN/ntuufUNewCVvVDPrAwm9qV6BBdDqK4q7lad7
AgMBAAGjOTA3MB0GA1UdDgQWBBTaBYO3uiTFOuZ9WUU0+I+raPWnyTAJBgNVHRME
AjAAMAsGA1UdDwQEAwIFoDANBgkqhkiG9w0BAQwFAAOCAYEAA084isryGPfhNH/g
lWPdpDi+4ff2XKbAVT+1rUlssoZ97LN0oSApKrOQZoJo2DPmWnhsOULzqVE+JEhT
gJYWCgR4RkSic2up5dI3IoUutBfeScY8PWuwQu96MQGNlmQWYSeJvJ7RuRgr/810
LCFdmBIs2XyViqwxSxwfkSBeHsy+lu2YnxuFTKSzaI+T2vop2Ipll+KIBR/z88Iu
+epdvbwanSQZIgZXaxWl5DBw8lt1B5WAFH3hE1wP/7g+1McpI2wdoJkrrQa0eRhk
YO0ZdSe78vJoSMCB1djdT25bY6dpwchB76FvHOjR7cS7ikl3MSpaeqXZSODSSuyM
rcboAalxkcOgzzaHM8q85pZ95819Wz0TgXYnzKC4DVM71VORiVcMu8SgJDGofzkY
ydgSJe5Qq9lkBzoPNA6IjE6dab/aq/DoFN2i/x+3DxIHhfdQH/hL4YYo/ZjIFaSN
DWWCpyBW11I2ijD9jOQJgQV4tGsKVjC5LXoTPq3S+yHqLxus
-----END CERTIFICATE-----";
            kafka.SslKeyPem = @"-----BEGIN PRIVATE KEY-----
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
ydLtzeh+Kcz3mTxg5VStObjr
-----END PRIVATE KEY-----";

var caCert = @"-----BEGIN CERTIFICATE-----
MIIEQTCCAqmgAwIBAgIUUC+tdnNM+FIyvGPY5V4F5Y5duGgwDQYJKoZIhvcNAQEM
BQAwOjE4MDYGA1UEAwwvYzhhMmYyMzYtNzAzZS00ODAxLWJiYmYtZmY5ZmViYjc1
MzI2IFByb2plY3QgQ0EwHhcNMjEwMjI3MDkyNzMwWhcNMzEwMjI1MDkyNzMwWjA6
MTgwNgYDVQQDDC9jOGEyZjIzNi03MDNlLTQ4MDEtYmJiZi1mZjlmZWJiNzUzMjYg
UHJvamVjdCBDQTCCAaIwDQYJKoZIhvcNAQEBBQADggGPADCCAYoCggGBANhiTOdu
m6aboE2TdL8yoetBjI5NSqcf6gwePRRdvRh4jZuuZJF71P9qHjaqrf6VZkIOE8Sy
5AXg2Fs6iWCCOGLVFrReVanMvEXJU4lVulceiZthlUS/GhtWzciaVzFDqJGOkUnX
vLhNVDdcR5fb5H6CsDlkDfgd6zKaAc/kSirMG6GsZIh8dWx4+V6r2IOMRCF1tAi8
jAyJeiwK3xAG0sG/Ku59M6D49YpAJxybn52+Pt1DgFzXSc+ilztza74vTPz3ZrOe
FVwIadvro1BdvsNcDYNHe6X+xz4Uxij8CSZGxnzJHvKdT5oE0+gDQC42S6jS5vi5
o5J5+MRmVqC/nDnKsWlHquwEekxA5cJIA4YhKeC6SYIJKjYfGxUNK+TWiJF4Bv+W
/wNt57ZejJHq9bJppC24vlYfsgGiATxjCkPUTI1BIs3/3qLwhpSw4FqPoofQ1sUO
fe20ne/nhgVWMmQZERFi63n6H07zXdN5XarMW53b658WAspzsrXXMo9ZMwIDAQAB
oz8wPTAdBgNVHQ4EFgQUp5yuq8RZcddTAsajfBkzvZNZi+8wDwYDVR0TBAgwBgEB
/wIBADALBgNVHQ8EBAMCAQYwDQYJKoZIhvcNAQEMBQADggGBAKD+U+zi/u1pAaLq
hceHI3Pg4IQQIERP8gMVCGfKP1DbMmksfb0Udy2rng/WbzuvsyaIYtm64cU4+whB
b2BpAue0KNR7Jq99G9XRGtRo9msGay1Dq36EwagkSr21P2yWE+cIp7fLyQ/BWGbM
99RIuBbFhMJndVfEXe9YyHUhBqGr+6umDqzNNZbzT+/FatSpE4dMmj9dZY5psDdK
i/7SaDJO7s0LF+o80DfCnIunBFkcD//D0xXUDajppPv8jrle9EOnVtHFIN7gOovZ
18q/+h6WWgPbrpJaEQfUbyroJ0ElIkPfWmrfyb3S5MPkRkEKZMOaVfbwfClEiDN7
ppAqigddk0Apyb9OFmphYQayGxO2BnEuBi7dHQYYfNLo3HhbpBQP/sUkk3ZWnPTa
+fSFZ9vvfPy6mj5bgex4u6HC/oChrZXSmfncy4Utf3YWcOLlsDu+WIBQwJghj2PR
Jp/t9rs9JAghFJM+Dsy6qun2SD2/qcx8a8E07HuGbs4DoybwUg==
-----END CERTIFICATE-----";

System.IO.File.WriteAllText("./foo.crt", caCert);
kafka.SslCaLocation = "./foo.crt";

            var publisher = new ProducerBuilder<Null, string>(kafka).Build();

            var result = await publisher.ProduceAsync("foo",new Message<Null, string>(){Value = "Hello"});

            Assert.True(result.Status == PersistenceStatus.Persisted);
        }


        [Fact]
        public async Task CanProduceToKafka()
        {
            //SetupConsumer();
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

            //this.consumer.Subscribe("foo");

            await module.Receive(msg);

            Assert.True(msg.GetValue<bool>("crosser.success"));

            // var cr = this.consumer.Consume(10000);
            // Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");

            // Assert.Equal("{\"say\":\"Hello World\"}", cr.Message.Value);
        }
    }
}