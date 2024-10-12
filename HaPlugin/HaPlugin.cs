namespace Loupedeck.HaPlugin
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HaPlugin.Events;
    using Loupedeck.HaPlugin.Json;
    using Loupedeck.HaPlugin.Helpers;

    using Newtonsoft.Json.Linq;

    using Websocket.Client;
    using System.Reactive.Linq;
    using Newtonsoft.Json;

    public class HaPlugin : Plugin
    {
        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;

        private String Token;
        private WebsocketClient WebSocket;

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
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Configuration could not be read.", "https://github.com/schmic/Loupedeck-HomeAssistant", "Help");
                return;
            }
            else if (Config.Token.IsNullOrEmpty())
            {
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Configuration is missing token.", "https://github.com/schmic/Loupedeck-HomeAssistant", "Help");
                return;
            }
            else if (Config.Url.IsNullOrEmpty())
            {
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Configuration is missing url.", "https://github.com/schmic/Loupedeck-HomeAssistant", "Help");
                return;
            }

            this.Token = Config.Token;
            this.SetupWebsocket(Config.ApiUrl);
        }

        public override void Unload() => this.WebSocket.Dispose();

        private enum Events : Int32
        {
            get_states = 1001,
            subscribe_events = 1002,
        }

        private Int32 Event_Id = 2000;

        public Dictionary<String, HaState> States = new Dictionary<String, HaState>();
        public event EventHandler<EventArgs> StatesReady;
        public event EventHandler<StateChangedEventArgs> StateChanged;

        private void SetupWebsocket(Uri url)
        {
            PluginLog.Verbose($"SetupWebsocket(): [url: {url}] [token: {this.Token.Substring(0, 16)}...]");
            this.WebSocket = new WebsocketClient(url);

            this.WebSocket.ReconnectTimeout = TimeSpan.FromSeconds(15);
            this.WebSocket.ErrorReconnectTimeout = TimeSpan.FromSeconds(15);
            
            this.WebSocket.ReconnectionHappened.Subscribe(info =>
                PluginLog.Info($"Reconnection happened, type: {info.Type}"));
            this.WebSocket.DisconnectionHappened.Subscribe(info => 
                PluginLog.Info(info.Exception, $"Disconnect happened, type: {info.Type}"));

            this.WebSocket.MessageReceived.Subscribe(this.OnMessageHdl);

            this.WebSocket.Start();
        }

        private void OnMessageHdl(ResponseMessage msg)
        {
            var respData = JObject.Parse(msg.Text);
            var respType = (String)respData["type"];

            if (respType.Equals("event"))
            {
                var evtType = (String)respData["event"]["event_type"];

                if (evtType.Equals("state_changed"))
                {
                    try
                    {
                        var haEvent = respData["event"]["data"].ToObject<HaEvent>();
                        var haState = haEvent.State;
                        this.States[haState.Entity_Id] = haState;
                        this.StateChanged?.Invoke(null, StateChangedEventArgs.Create(haState.Entity_Id));
                    }
                    catch (JsonSerializationException)
                    {
                        PluginLog.Warning($"JsonSerializationException HaEvent: {respData["event"]["data"]}");
                    }
                }
            }
            else if (respType.Equals("result"))
            {
                var result_id = Int32.Parse(respData["id"].ToString());
                var result_event = (Events)result_id;
                var result_status = (Boolean)respData["success"];

                if (result_event.Equals(Events.get_states))
                {
                    PluginLog.Info("States Update Success: " + result_status);

                    foreach (JToken t in respData["result"].Children())
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
            else if (respType.Equals("auth_required"))
            {
                var auth = new JObject
                {
                    ["type"] = "auth",
                    ["access_token"] = this.Token
                };
                PluginLog.Info("Sending auth token ..");
                this.WebSocket.Send(auth.ToString());
            }
            else if (respType.Equals("auth_ok"))
            {
                PluginLog.Info("Socket Auth: OK (HA Version: " + respData["ha_version"] + ")");

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
                PluginLog.Warning($"### HA unknown response type {respType}\n{respData}");
            }
        }

        public void CallService(JObject data)
        {
            if (!data.ContainsKey("type"))
            {
                data.Add("type", "call_service");
            }
            PluginLog.Verbose(data.ToString());
            this.Send(data);
        }

        private void Send(JObject data)
        {
            data.Add("id", ++this.Event_Id);
            this.WebSocket.Send(data.ToString());
        }
    }
}
