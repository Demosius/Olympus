using System;
using System.Collections.Generic;
using System.Linq;
using Uranus.Staff.Models;

namespace Deimos.Models;

public class QASession
{
    public static TimeSpan QABreakSpan => new(0, 5, 0);

    public Employee Operator { get; set; }
    public List<QACarton> Cartons { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan AverageCartonTime { get; set; }

    public TimeSpan Duration => EndTime.Subtract(StartTime);
    public int CartonCount => Cartons.Count;
    public int Scans => Cartons.Sum(c => c.QAScans);
    public int Units => Cartons.Sum(c => c.QAUnits);

    // Assume all are the correctly set, same operator, and same date.
    // Use DateTime instead of TimeSpan for Start and End in case they are not the same date.
    // Should also be separated into appropriate groups on the same date based on time between. 
    //      - Assume this too, has been done.
    public QASession(Employee employee, IEnumerable<QACarton> cartons)
    {
        Operator = employee;
        Cartons = cartons.OrderBy(c => c.DateTime).ToList();

        var times = new List<TimeSpan>();
        // Connect cartons.
        for (var i = 0; i < Cartons.Count - 1; i++)
        {
            Cartons[i].NextCarton = Cartons[i + 1];
            times.Add(Cartons[i].QATime);
        }

        AverageCartonTime =  new TimeSpan((long) times.Average(t => t.Ticks));
        
        StartTime = Cartons.First().DateTime;
        EndTime = Cartons.Last().DateTime.AddTicks(AverageCartonTime.Ticks);
    }

    public static Dictionary<Employee, List<QASession>> GetSessions(List<QACarton> cartons, TimeSpan? breakSpan = null)
    {
        breakSpan ??= QABreakSpan;

        var returnDict = new Dictionary<Employee, List<QASession>>();

        var dateGroups = cartons.Where(c => c.QAOperator is not null).GroupBy(c => (c.QAOperator, c.Date)).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var ((employee, _), groupCartons) in dateGroups)
        {
            var ctns = groupCartons.OrderBy(c => c.Time).ToList();
            var sessions = new List<QASession>();
            var sessionCartons = new List<QACarton> {ctns.First()};

            for (var i = 0; i < ctns.Count; i++)
            {
                var carton = ctns[i];
                var nextCarton = ctns[i + 1];
                if (nextCarton.Time.Subtract(carton.Time) >= breakSpan)
                {
                    if (sessionCartons.Count > 1) 
                        sessions.Add(new QASession(employee!, sessionCartons));
                    sessionCartons.Clear();
                }
                sessionCartons.Add(ctns[i+1]);
            }
            // Add to dict.
            if (returnDict.ContainsKey(employee!))
                returnDict[employee!].AddRange(sessions);
            else
                returnDict.Add(employee!, sessions);
        }

        return returnDict;
    }
}