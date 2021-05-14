using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OlympusTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine(5+6);
            Assert.AreEqual(5 + 6, 11);
        }

        [TestMethod]
        public void TM2()
        {
            Assert.AreEqual(15, 16);
        }

    }
}
