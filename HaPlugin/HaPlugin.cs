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

        private readonly String Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI5ZTQ5N2VhNzE3MzU0ODNhYWU2ZWY4ZGNmNTgyMWIwYSIsImlhdCI6MTY5OTU2MzczNywiZXhwIjoyMDE0OTIzNzM3fQ.gROeLs3aW9RhDVB4gCwJTosKoSoYZ6SwK5yj2ccLx0s";
        private readonly WebSocket WebSocket = new WebSocket("wss://ha.home.schmic.eu/api/websocket");

        public HaPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load() => this.SetupWebsocket();

        private enum Events : Int32
        {
            get_states = 1001,
            subscribe_events = 1002,
        }

        public Dictionary<String, HaState> States = new Dictionary<String, HaState>();
        public event EventHandler<EventArgs> StatesReady;
        public event EventHandler<StateChangedEventArgs> StateChanged;

        public void SetupWebsocket()
        {
            PluginLog.Verbose("SetupWebsocket()");

            this.WebSocket.Log.Level = LogLevel.Info;
            this.WebSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            this.WebSocket.SslConfiguration.ServerCertificateValidationCallback = null;

            this.WebSocket.OnError += this.OnErrorHdl;
            this.WebSocket.OnMessage += this.OnMessageHdl;

            this.WebSocket.Connect();
        }

        private void OnErrorHdl(Object sender, ErrorEventArgs e) => PluginLog.Error("### error:" + e);

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
                    PluginLog.Verbose($"state_changed [{haState.Entity_Id}: {haState.State}]");
                    this.States[haState.Entity_Id] = haState;
                    this.StateChanged?.Invoke(null, StateChangedEventArgs.Create(haState.Entity_Id));
                }
                else
                {
                    PluginLog.Verbose($"Unhandled haEventType: {haEventType}");
                }
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
                else
                {
                    PluginLog.Warning("Unknown ResultID: " + result_id);
                    PluginLog.Warning("Event Data:\n" + data);
                }
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
                PluginLog.Info("### data with unknown type:" + data);
            }
        }

        // This method is called when the plugin is unloaded during the Loupedeck service shutdown.
        public override void Unload() => this.WebSocket.Close();

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
    }
}
