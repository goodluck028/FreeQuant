using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FreeQuant.Framework;

namespace UnitTest {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            string name = Exchange.CFFEX.ToString();
            Assert.AreEqual(1, 1);
        }
    }
}
