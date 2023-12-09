namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HomeAssistant.Events;
    using Loupedeck.HomeAssistant.Json;

    using Newtonsoft.Json.Linq;

    using WebSocketSharp;

    public class HaPlugin : Plugin
    {
        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;

        private String Token;
        private WebSocket WebSocket;

        public HaPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
            var Config = HaConfig.Read();

            if (Config == null)
            {
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Configuration could not be read.", "https://github.com/schmic/Loupedeck-HomeAssistant/wiki", "Help");
                return;
            }

            this.Token = Config.Token;
            this.SetupWebsocket(Config.Uri.ToString());
        }

        public override void Unload() => this.WebSocket.Close();

        private enum Events : Int32
        {
            get_states = 1001,
            subscribe_events = 1002,
        }

        public Dictionary<String, HaState> States = new Dictionary<String, HaState>();
        public event EventHandler<EventArgs> StatesReady;
        public event EventHandler<StateChangedEventArgs> StateChanged;

        private void SetupWebsocket(String url)
        {
            PluginLog.Verbose($"SetupWebsocket(): [url: {url}]");
            this.WebSocket = new WebSocket(url);
            this.WebSocket.Log.Level = LogLevel.Info;
            this.WebSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            this.WebSocket.SslConfiguration.ServerCertificateValidationCallback = null;

            this.WebSocket.OnError += this.OnErrorHdl;
            this.WebSocket.OnMessage += this.OnMessageHdl;

            this.WebSocket.Connect();
        }

        private void OnErrorHdl(Object sender, ErrorEventArgs e) => PluginLog.Error($"### Error: {e.Message}\n{e.Exception}");

        private void OnMessageHdl(Object sender, MessageEventArgs e)
        {
            var data = JObject.Parse(e.Data);
            var type = (String)data["type"];

            if (type.Equals("event"))
            {
                var haEventType = (String)data["event"]["event_type"];

                if (haEventType.Equals("state_changed"))
                {
                    var haEvent = data["event"]["data"].ToObject<HaEvent>();
                    var haState = haEvent.State;
                    //PluginLog.Verbose($"state_changed [{haState.Entity_Id}: {haState.State}]");
                    this.States[haState.Entity_Id] = haState;
                    this.StateChanged?.Invoke(null, StateChangedEventArgs.Create(haState.Entity_Id));
                }
                //else
                //{
                //    PluginLog.Verbose($"Unhandled HaEventType: {haEventType}\n{data}");
                //}
            }
            else if (type.Equals("result"))
            {
                var result_id = Int32.Parse(data["id"].ToString());
                var result_event = (Events)result_id;
                var result_status = (Boolean)data["success"];

                if (result_event.Equals(Events.get_states))
                {
                    PluginLog.Info("States Update Success: " + result_status);

                    foreach (JToken t in data["result"].Children())
                    {
                        var haState = t.ToObject<HaState>();
                        //PluginLog.Verbose("state.ToString(): " + haState.ToString());
                        this.States[haState.Entity_Id] = haState;
                    }

                    PluginLog.Info($"Received {this.States.Count} states");
                    StatesReady.Invoke(null, null);
                }
                else if (result_event.Equals(Events.subscribe_events))
                {
                    PluginLog.Info("Event Subscription Success: " + result_status);
                }
                //else
                //{
                //    PluginLog.Warning($"Unknown ResultID: {result_id}\n{data}");
                //}
            }
            else if (type.Equals("auth_required"))
            {
                var auth = new JObject
                {
                    ["type"] = "auth",
                    ["access_token"] = this.Token
                };
                PluginLog.Info("Sending auth token ..");
                this.WebSocket.Send(auth.ToString());
            }
            else if (type.Equals("auth_ok"))
            {
                PluginLog.Info("Socket Auth: OK (HA Version: " + data["ha_version"] + ")");

                var get_states = new JObject
                {
                    ["id"] = (Int32)Events.get_states,
                    ["type"] = Events.get_states.ToString()
                };
                this.WebSocket.Send(get_states.ToString());

                var subscribe = new JObject
                {
                    ["id"] = (Int32)Events.subscribe_events,
                    ["type"] = Events.subscribe_events.ToString()
                };
                this.WebSocket.Send(subscribe.ToString());
            }
            else
            {
                PluginLog.Warning($"### Unknown Msg Type {type}\n{data}");
            }
        }

        private Int32 id = 2000;

        public void LightToggle(String entity_id)
        {
            var serivce_req = new JObject {
                { "id", ++this.id },
                { "type", "call_service" },
                { "domain", "light" },
                { "service", "toggle" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            PluginLog.Verbose($"LightToggle: [entity_id: {entity_id}] [id: {this.id}]");

            this.WebSocket.Send(serivce_req.ToString());
        }
        public void LightBrightness(String entity_id, Int32 brightness)
        {
            var serivce_req = new JObject {
                { "id", ++this.id },
                { "type", "call_service" },
                { "domain", "light" },
                { "service", "turn_on" },
                { "service_data", new JObject { { "brightness", brightness} } },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            PluginLog.Verbose($"LightBrightness: [entity_id: {entity_id}] [id: {this.id}] [brightness: {brightness}]");

            this.WebSocket.Send(serivce_req.ToString());
        }

        public void SwitchToggle(String entity_id)
        {
            var serivce_req = new JObject {
                { "id", ++this.id },
                { "type", "call_service" },
                { "domain", "switch" },
                { "service", "toggle" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            PluginLog.Verbose($"SwitchToggle: [entity_id: {entity_id}] [id: {this.id}]");

            this.WebSocket.Send(serivce_req.ToString());
        }
        public void ClimateTemperature(String entity_id, Int32 temperature)
        {
            var serivce_req = new JObject {
                { "id", ++this.id },
                { "type", "call_service" },
                { "domain", "climate" },
                { "service", "set_temperature" },
                { "service_data", new JObject { { "temperature", temperature} } },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            PluginLog.Verbose($"ClimateTemperature: [entity_id: {entity_id}] [id: {this.id}] [temperature: {temperature}]");

            this.WebSocket.Send(serivce_req.ToString());
        }
    }
}
