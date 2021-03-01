namespace Crosser.EdgeNode.Modules.Kafka.Producer
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Confluent.Kafka;
    using Crosser.EdgeNode.Flows.Abstractions;
    using NJsonSchema.Annotations;

    // public class KafkaSaslSettings
    // {
    //     [JsonSchemaExtensionData("x-sortOrder", 1)]
    //     [Display(Name = "SASL Mechanism", Description = "The SASL Mechanism to use")]
    //     [Required, DefaultValue(SaslMechanism.ScramSha256)]
    //     public SaslMechanism Mechanism { get; set; }

    //     [JsonSchemaExtensionData("x-sortOrder", 2)]
    //     [Display(Name = "Credentials", Description = "The SASL credentials to use")]
    //     [JsonSchemaExtensionData(Credential.ATTRIBUTE, Credential.Types.UsernameAndPassword)]
    //     public Guid? SaslCredentials { get; set; }
    // }

    public class KafkaProducerModuleSettings : FlowModuleSettings
    {
        [JsonSchemaExtensionData("x-sortOrder", 1)]
        [Display(Name = "Brokers", Description = "The Kafka brokers to use separated by comma")]
        [Required, DefaultValue(""), MinLength(1), MaxLength(256)]
        public string Brokers {get;set;}

        // [JsonSchemaExtensionData("x-sortOrder", 2)]
        // [Display(Name = "Sasl Settings", Description = "The Sasl settings for your Kafka server")]
        // [Required]
        // public KafkaSaslSettings KafkaSaslSettings {get;set;} = new();

        [JsonSchemaExtensionData("x-sortOrder", 2)]
        [Display(Name = "SASL Mechanism", Description = "The SASL Mechanism to use")]
        [Required, DefaultValue(SaslMechanism.Plain)]
        public SaslMechanism Mechanism { get; set; }

        // [JsonSchemaExtensionData("x-sortOrder", 3)]
        // [Display(Name = "Security Protocol", Description = "The security protocol configured on the broker endpoint")]
        // [Required, DefaultValue(SecurityProtocol.SaslSsl)]
        // public SecurityProtocol SecurityProtocol { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 3)]
        [Display(Name = "Credentials", Description = "The SASL credentials to use")]
        [JsonSchemaExtensionData(Credential.ATTRIBUTE, Credential.Types.UsernameAndPassword)]
        public Guid? SaslCredentials { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 4)]
        [Display(Name = "Use SSL?", Description = "Check to enable SSL")]
        [Required, DefaultValue(true)]
        public bool UseSSL { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 5)]
        [Display(Name = "CA Certificate Location", Description = "If you want to verify the server certificate you need the CA Certificate. If the certificate is in the store, or if you do not want to validate the server certificate, you do not need to set this")]
        [Required, DefaultValue(""), MinLength(0), MaxLength(256)]
        public string CACertificateLocation { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 6)]
        [Display(Name = "Verify server cetificate?", Description = "If your brokers are using a self signed certificate you will need to untick `Verify server certificate?`")]
        [Required, DefaultValue(true)]
        public bool VerifyServerCertificate { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 7)]
        [Display(Name = "Source Property", Description = "The pattern to locate the value to send to Kafka")]
        [Required, MinLength(1), MaxLength(64), DefaultValue("data")]
        public string SourceProperty { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 8)]
        [Display(Name = "Topic", Description = "The topic/pattern to use when publishing")]
        [Required, MinLength(1), MaxLength(64), DefaultValue("topic")]
        public string Topic { get; set; }
    }
}