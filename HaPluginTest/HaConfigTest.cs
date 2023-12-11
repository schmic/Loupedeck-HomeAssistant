namespace HaPluginTest
{
    using System;

    using Loupedeck.HomeAssistant;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json.Linq;

    [TestClass]
    public class HaConfigTest
    {
        [TestMethod]
        public void TestLocalhostAndPortConfig()
        {
            var Config = this.CreateConfig(url: "http://localhost:8123");

            Assert.AreEqual(Config.ApiUrl, "ws://localhost:8123/api/websocket");
        }

        [TestMethod]
        public void TestIpAndPortConfig()
        {
            var Config = this.CreateConfig(url: "http://127.0.0.1:8123");

            Assert.AreEqual(Config.ApiUrl, "ws://127.0.0.1:8123/api/websocket");
        }

        [TestMethod]
        public void TestHostnameAndPortConfig()
        {
            var Config = this.CreateConfig(url: "http://somehostname:8123");

            Assert.AreEqual(Config.ApiUrl, "ws://somehostname:8123/api/websocket");
        }

        [TestMethod]
        public void TestHostnameConfig()
        {
            var Config = this.CreateConfig(url: "http://somehostname");

            Assert.AreEqual(Config.ApiUrl, "ws://somehostname/api/websocket");
        }

        [TestMethod]
        public void TestHostnameWithHttpsConfig()
        {
            var Config = this.CreateConfig(url: "https://somehostname");

            Assert.AreEqual(Config.ApiUrl, "wss://somehostname/api/websocket");
        }

        [TestMethod]
        public void TestHostnameWithPathConfig()
        {
            var Config = this.CreateConfig(url: "https://somehostname/api/");

            Assert.AreEqual(Config.ApiUrl, "wss://somehostname/api/websocket");
        }

        private HaConfig CreateConfig(String url = "http://localhost:8123", String token = "aTokenString")
        {
            var configValid = new JObject
            {
                { "token", token },
                { "url", url}
            }.ToString();

            return HaConfig.FromString(configValid);
        }
    }
}
