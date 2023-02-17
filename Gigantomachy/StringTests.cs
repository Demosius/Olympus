using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uranus;

// ReSharper disable StringLiteralTypo

namespace Gigantomachy;

[TestClass]
public class StringTests
{
    [TestMethod]
    public void TestIntersect()
    {
        // Arrange
        var s1 = "TS SW MANDO DARKSABER U XS";

        var s2 = "TS SW MANDO DARKSABER U S";

        var s3 = "TS SW MANDO DARKSABER U M";

        var s4 = "TS SW MANDO DARKSABER U L";

        var s5 = "TS SW MANDO DARKSABER U XL";

        var s6 = "TS SW MANDO DARKSABER U XXL";

        // Act
        var common = Utility.LongestCommonSubstring(new List<string> { s1, s2, s3, s4, s5, s6 }).Trim();

        // Assert
        Assert.AreEqual("TS SW MANDO DARKSABER U", common);
    }
}