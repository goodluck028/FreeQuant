using Microsoft.VisualStudio.TestTools.UnitTesting;
using FreeQuant.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Components.Tests {
    [TestClass()]
    public class BarGeneratorTests {
        [TestMethod()]
        public void addTickTest() {
            DateTime current = DateTime.Now;
            DateTime barTime = current.AddMinutes(-1);
            int barQuotient = (barTime.Day * 1440 + barTime.Hour * 60 + barTime.Minute) / ((int)BarSizeType.Min1);
            int tickQuotient = (current.Day * 1440 + current.Hour * 60 + current.Minute) / ((int)BarSizeType.Min1);
            Assert.AreNotEqual(barQuotient,tickQuotient);
        }
    }
}