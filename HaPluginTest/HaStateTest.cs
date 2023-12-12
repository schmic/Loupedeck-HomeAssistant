namespace HaPluginTest
{
    using System.Security.Policy;

    using Loupedeck.HomeAssistant.Json;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json.Linq;

    [TestClass]
    public class HaStateTest
    {
        [TestMethod]
        public void TestHaStateNothingNull()
        {
            var aState = new JObject
            {
                { "entity_id", "placeholder" },
                { "Attributes", new JObject {  } }
            }.ToString();

            var haState = HaState.FromString(aState);

            Assert.IsNotNull(haState.FriendlyName);
            Assert.AreEqual(haState.FriendlyName,"#placeholder");

            Assert.IsNotNull(haState.Icon);
            Assert.AreEqual(haState.Icon, "");
        }
    }
}
