using Microsoft.VisualStudio.TestTools.UnitTesting;
using FreeQuant.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework.Tests {
    [TestClass()]
    public class RegexUtilsTests {
        [TestMethod()]
        public void TakeShortInstrumentIDTest() {
            string instID = "TA909";
            string resualt = RegexUtils.TakeShortInstrumentID(instID);
            Assert.AreEqual("TA09", resualt);
        }

        [TestMethod()]
        public void TakeProductNameTest() {
            string instID = "TA909";
            string resualt = RegexUtils.TakeProductName(instID);
            Assert.AreEqual("TA", resualt);
        }
    }
}