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
                { "Attributes", new JObject {  } }
            }.ToString();

            var haState = HaState.FromString(aState);

            Assert.IsNotNull(haState.FriendlyName);
            Assert.IsNotNull(haState.Icon);
        }
    }
}
