using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            string name = new ClassB().metherdA();
            Assert.AreEqual("ClassB",name);
        }
    }

    public class ClassA
    {
        public string metherdA()
        {
            return GetType().Name;
        }
    }

    public class ClassB : ClassA
    {

    }
}
