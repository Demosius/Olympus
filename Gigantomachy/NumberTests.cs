using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gigantomachy;

[TestClass]
public class NumberTests
{
    [TestMethod]
    public void HighestCommonFactorTesting()
    {
        // Arrange
        var numbers = new List<int> { 12, 20, 24 };
        var n1 = 24;
        var n2 = 72;
        var bigNumbers = new List<int> { 1500, 54128, 90000 };

        // Act
        var res1 = Morpheus.Utility.HCF(numbers);
        var res2 = Morpheus.Utility.HCF(n1, n2);
        var res3 = Morpheus.Utility.HCF(bigNumbers);

        // Assert
        Assert.AreEqual(4, res1);
        Assert.AreEqual(24, res2);
        Assert.AreEqual(4, res3);

    }
}