using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestData.Part1;
using TestData.Part1.Share;
using TestData.Part2;
using TestData.Part2.Share;
using static SimpleHostContainer.Test.net472.Global;

namespace SimpleHostContainer.Test.net472
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            Global.Init();
        }

        [TestMethod]
        public void TestMethod1()
        {
            IInterface_0 interface_0 = Get<IInterface_0>();

            Assert.IsNotNull(interface_0);
            Assert.AreEqual(typeof(Class_0), interface_0.GetType());
        }

        [TestMethod]
        public void TestMethod2()
        {
            Class_0 class_0 = Get<Class_0>();

            Assert.IsNotNull(class_0);
            Assert.AreEqual(typeof(Class_0), class_0.GetType());
        }

        [TestMethod]
        public void TestMethod3()
        {
            Class_1 class_1 = Get<Class_1>();

            Assert.IsNotNull(class_1);
            Assert.AreEqual(typeof(Class_1), class_1.GetType());
        }

        [TestMethod]
        public void TestMethod4()
        {
            IInterface_1 interface2 = Get<IInterface_1>();

            Assert.IsNotNull(interface2);
            Assert.AreEqual(typeof(Class_2), interface2.GetType());
        }

        [TestMethod]
        public void TestMethod5()
        {
            Class_2 class_2 = Get<Class_2>();

            Assert.IsNull(class_2);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Class_3 class_3 = Get<Class_3>();

            Assert.IsNull(class_3);
        }

        [TestMethod]
        public void TestMethod13()
        {
            Part1Options part1Options = Get<IOptions<Part1Options>>().Value;

            Assert.IsNotNull(part1Options);
        }

        [TestMethod]
        public void TestMethod15()
        {
            Part1Options part1Options = Get<IOptions<Part1Options>>().Value;

            Assert.IsNotNull(part1Options);

            Assert.AreEqual(5, part1Options.Value);
        }
    }
}
