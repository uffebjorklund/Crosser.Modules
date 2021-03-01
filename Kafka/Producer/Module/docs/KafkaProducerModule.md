# Kafka Producer

Will send messages to Kafka

## Settings

| Name                       | Requirements                 | Purpose                                                                                                  | Default |
| -------------------------- | ---------------------------- | -------------------------------------------------------------------------------------------------------- | ------  |
| Brokers                    | String with length `1 to 256`| The Kafka brokers to use separated by comma                                                              | ``      |
| SASL Mechanism             | See SASL Mechanism section   | Simple Authentication and Security Layer (SASL) Mechanism                                                | `Plain` |
| SASL Credential            | Username/Password            | The credential to use for broker authentication                                                          | ``      |
| Use SSL                    | Boolean                      | If true the security protocol will be SaslSSL                                                            | ``      |
| CA Certificate Location    | string with length `0 to 256`| If you want to verify the server certificate you need the CA Certificate. If the certificate is in the store, or if you do not want to validate the server certificate, you do not need to set this | `` |
| Verify server certificate  | Boolean                      | If your brokers are using a self signed certificate you will need to untick `Verify server certificate?` | `true`  |
| Source Property            | string with length `1 to 64` | The pattern to locate the value to send to Kafka                                                         | `data`  |
| Topic                      | string with length `1 to 64` | The topic/pattern to use when publishing                                                                 | `data`  |

### SALS Mechanism

 - Gssapi
 - Plain
 - ScramSha256
 - ScramSha512
 - OAuthBearer = 4