using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uranus.Extension;

namespace Gigantomachy;

[TestClass]
public class UranusTests
{
    [TestMethod]
    public void FiscalWeekTesting()
    {
        // Arrange
        var date1 = new DateTime(2021, 1, 13);
        var date2 = new DateTime(2021, 4, 17);
        var date3 = new DateTime(2021, 5, 1);
        var date4 = new DateTime(2021, 8, 8);
        var date5 = new DateTime(2021, 9, 4);
        var date6 = new DateTime(2021, 12, 31);

        // Act
        var date = new DateTime(2020, 1, 1);
        while (date < new DateTime(2022,2,1))
        {
            Console.WriteLine($"{date:d} = {date.FiscalWeek()}");
            date = date.AddDays(1);
        }


        // Assert
        Assert.AreEqual("Jan-Wk2", date1.FiscalWeek(), $"{date1} did not give the correct result.");
        Assert.AreEqual("Apr-Wk2", date2.FiscalWeek(), $"{date2} did not give the correct result.");
        Assert.AreEqual("Apr-Wk4", date3.FiscalWeek(), $"{date3} did not give the correct result.");
        Assert.AreEqual("Aug-Wk1", date4.FiscalWeek(), $"{date4} did not give the correct result.");
        Assert.AreEqual("Sep-Wk1", date5.FiscalWeek(), $"{date5} did not give the correct result.");
        Assert.AreEqual("Dec-Wk5", date6.FiscalWeek(), $"{date6} did not give the correct result.");

    }
}