namespace Crosser.EdgeNode.Modules.Kafka.Producer
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Confluent.Kafka;
    using Crosser.EdgeNode.Flows.Abstractions;
    using NJsonSchema.Annotations;

    public class KafkaProducerModuleSettings : FlowModuleSettings
    {
        [JsonSchemaExtensionData("x-sortOrder", 0)]
        [Display(Name = "Source Property", Description = "The property that contains the data to send")]
        [Required]
        [MinLength(1)]
        [MaxLength(64)]
        [DefaultValue("data")]
        public string SourceProperty { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 1)]
        [Display(Name = "Servers", Description = "A CSV separated list servers")]
        [Required]
        [MinLength(6)]
        [MaxLength(1024)]
        [DefaultValue("rocket-01.srvs.cloudkafka.com:9094,rocket-02.srvs.cloudkafka.com:9094,rocket-03.srvs.cloudkafka.com:9094")]
        public string Servers { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 2)]
        [Display(Name = "Credentials", Description = "The Kafka credentials to use")]
        [JsonSchemaExtensionData(Credential.ATTRIBUTE, "UsernameAndPassword")] // ,Certificate <= to be added
        public Guid? KafkaCredentials { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 3)]
        [Display(Name = "SASL Mechanism")]
        public SaslMechanism SaslMechanism { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 4)]
        [Display(Name = "Security Protocol")]
        public SecurityProtocol SecurityProtocol { get; set; }
    }
}