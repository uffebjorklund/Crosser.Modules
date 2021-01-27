namespace Crosser.EdgeNode.Modules
{
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Validation;
    using Crosser.EdgeNode.Flows.Abstractions;
    using System.ComponentModel;

    public class NodeJsBridgeModuleSettings : FlowModuleSettings
    {
        public string NodeJsPath { get; set; } = "/home/uffe/.nvm/versions/node/v14.15.4/bin/node";
        public string NpmPath { get; set; } = "/home/uffe/.nvm/versions/node/v14.15.4/bin/npm";

        private const string INIT = @"
";
        private const string MAIN = @"
import json
# feel free to alter/add, but do not remove the definition `msg_handler`
def msg_handler(msg, module):
    print(json.dumps(msg))
    # pass the message to the next module
    module.next(msg)
    ";

        [DefaultValue(true)]
        public bool CatchStandardOutput { get; set; }

        [DefaultValue(INIT)]
        public string Packages { get; set; }

        [DefaultValue(MAIN)]
        public string OnMessage { get; set; }

        [DefaultValue("data")]
        public string SourceProperty { get; set; }

        [DefaultValue("data")]
        public string TargetProperty { get; set; }

        public override void Validate(SettingsValidator validator)
        {
            validator.Validate(nameof(this.TargetProperty), this.TargetProperty).MinLength(0).MaxLength(64).NotNull();
            validator.Validate(nameof(this.SourceProperty), this.SourceProperty).MinLength(0).MaxLength(64).NotNull();
            validator.Validate(nameof(this.OnMessage), this.OnMessage).NotNull();
        }
    }

}
