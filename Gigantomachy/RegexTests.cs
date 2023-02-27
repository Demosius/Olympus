using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gigantomachy;

[TestClass]
public class RegexTests
{
    [TestMethod]
    public void TrueOrderBins()
    {
        // Arrange
        const string pattern = @"^([\w]{2}|[\w]{4})(\d{2} \d{3})$";

        var string1 = "PR11 028";
        var string2 = "OS04 115";
        var string3 = "MCPR17 216";
        var string4 = "PRQ08 002";
        var string5 = "PR11 028B";

        // Act
        var replace1 = Regex.Replace(string1, pattern, "A$2");
        var replace2 = Regex.Replace(string2, pattern, "A$2");
        var replace3 = Regex.Replace(string3, pattern, "A$2");
        var replace4 = Regex.Replace(string4, pattern, "A$2");
        var replace5 = Regex.Replace(string5, pattern, "A$2");

        // Assert
        Assert.AreEqual("A11 028", replace1);
        Assert.AreEqual("A04 115", replace2);
        Assert.AreEqual("A17 216", replace3);
        Assert.AreEqual(string4, replace4);
        Assert.AreEqual(string5, replace5);

    }
}