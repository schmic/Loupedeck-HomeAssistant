namespace Loupedeck.HaPlugin.Commands
{
    using System;

    using Loupedeck.HaPlugin;
    using Loupedeck.HaPlugin.Helpers;

    using Newtonsoft.Json.Linq;

    class ScriptAdvancedCommand : ActionEditorCommand
    {
        public ScriptAdvancedCommand()
        {
            this.Name = "ScriptAdvancedCommand";
            this.DisplayName = "Execute Script with parameters";

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox("script", "Script").SetPlaceholder("Name of script").SetRequired()
            );
            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox("data", "Data", "Must be valid JSON").SetPlaceholder("{ \"message\":  \"foobar\" }")
            );
        }

        protected HaPlugin GetPlugin() => (HaPlugin)base.Plugin;

        protected override String GetCommandDisplayName(ActionEditorActionParameters actionParameters)
        {
            var scriptName = actionParameters.GetString("script");

            PluginLog.Info(scriptName);
            return base.GetCommandDisplayName(actionParameters);
        }

        //protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth, Int32 imageHeight) => base.GetCommandImage(actionParameters, imageWidth, imageHeight);

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var scriptName = actionParameters.GetString("script");

            if (scriptName.IsNullOrEmpty())
            {
                return false;
            }

            var scriptData = actionParameters.GetString("data");
            PluginLog.Info($"Executing script: {scriptName} with \"{scriptData}\"");

            JObject scriptJson = null;
            if (!scriptData.IsNullOrEmpty())
            {
                scriptJson = JObject.Parse(scriptData);
            }


            var data = new JObject {
                { "domain", "script" },
                { "service", "turn_on" },
                { "target", new JObject { { "entity_id", $"script.{scriptName}"} } }
            };

            if (scriptJson != null)
            {
                data["service_data"] = new JObject { { "variables", scriptJson} };
            }

            this.GetPlugin().CallService(data);

            return true;
        }
    }
}
