using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestData.Part1.Share;
using TestData.Part2.Share;
using static SimpleHostContainer.Test.net472.Plugin.Global;

namespace SimpleHostContainer.Test.net472.Plugin
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            Global.Init();
        }

        [TestMethod]
        public void TestMethod16()
        {
            IInterface_0 interface_0 = Get<IInterface_0>();

            Assert.IsNotNull(interface_0);
        }

        [TestMethod]
        public void TestMethod17()
        {
            IInterface_1 interface2 = Get<IInterface_1>();

            Assert.IsNotNull(interface2);
        }

        [TestMethod]
        public void TestMethod18()
        {
            Part1Options part1Options = Get<IOptions<Part1Options>>().Value;

            Assert.IsNotNull(part1Options);
        }

        [TestMethod]
        public void TestMethod19()
        {
            Part1Options part1Options = Get<IOptions<Part1Options>>().Value;

            Assert.IsNotNull(part1Options);

            Assert.AreEqual(5, part1Options.Value);
        }
    }
}
