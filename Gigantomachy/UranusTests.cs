using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Uranus.Extensions;

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
        while (date < new DateTime(2022, 2, 1))
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

    [TestMethod]
    public void WeekFindTesting()
    {
        const int tests = 5000;
        const string list = "2,4,6,8,10";
        var values = new List<(int, bool)>
        {
            (1, false),
            (2, true),
            (3, false),
            (4, true),
            (5, false),
            (6, true),
            (7, false),
            (8, true),
            (9, false),
            (10, true)
        };

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        for (var i = 0; i < tests; i++)
        {
            foreach (var (num, value) in values)
            {
                var regex = new Regex($"^{num}$|^({num}),|,({num})$|,({num}),");
                var match = regex.IsMatch(list);
                Assert.IsTrue(match == value);
            }
        }
        stopwatch.Stop();
        var regexTime = stopwatch.ElapsedMilliseconds;

        stopwatch = new Stopwatch();
        stopwatch.Start();
        for (var i = 0; i < tests; i++)
        {
            foreach (var (num, value) in values)
            {
                var contains = list.Split(",").Contains(num.ToString());
                Assert.IsTrue(contains == value);
            }
        }
        stopwatch.Stop();
        var splitTime = stopwatch.ElapsedMilliseconds;

        Console.WriteLine($"Regex Method Time: {regexTime} ms\n" +
                          $"Split Method Time: {splitTime} ms");

    }

    private class A
    {
        public virtual string Value => "This is A";
    }

    private class B : A
    {
        public override string Value => "This is B";
    }

    private class C : B
    {
        public override string Value => "This is C";
    }

    [TestMethod]
    public void TestInheritance()
    {
        var list = new List<A>();

        for (var i = 0; i < 10; i++)
        {
            list.Add(new A());
            list.Add(new B());
            list.Add(new C());
        }

        foreach (var a in list)
        {
            Console.WriteLine(a.Value);
        }
    }
}