using Microsoft.VisualStudio.TestTools.UnitTesting;
using FreeQuant.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Components.Tests {
    [TestClass()]
    public class RegexUtilsTests {
        [TestMethod()]
        public void TakeShortInstrumentIDTest() {
            string instId = "TA909";
            string resualt = RegexUtils.TakeShortInstrumentID(instId);
            Assert.AreEqual("TA09", resualt);
        }

        [TestMethod()]
        public void TakeProductNameTest() {
            string instId = "TA909";
            string resualt = RegexUtils.TakeProductName(instId);
            Assert.AreEqual("TA", resualt);
        }
    }
}