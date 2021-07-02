using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Olympus.Helios.Equipment
{
    public class Checklist
    {
        public string Name { get; set; }
        public string TypeCode { get; set; }
        public List<Check> Checks { get; set; }

        public Checklist() { }
        
        public Checklist(string name, string typeCode, string checklist)
        {
            Name = name;
            TypeCode = typeCode;
            Checks = JsonSerializer.Deserialize<List<Check>>(checklist);
        }

        public Checklist(string name, string typeCode, List<Check> checklist)
        {
            Name = name;
            TypeCode = typeCode;
            Checks = checklist;
        }
    }

    public class Check
    {
        public string Description { get; set; }
        public bool DesiredResponse { get; set; }
        public bool? Response { get; set; }
        public bool FaultFails { get; set; }

        public bool? IsFault()
        {
            return Response == null ? Response : Response != DesiredResponse;
        }

        public string FaultString()
        {
            return $"{Description}: {Response}";
        }
    }

    public class CompletedChecklist : Checklist
    {
        public int MachinSerialNumber { get; set; }
        public int EmployeeNumber { get; set; }

        public string Comment { get; set; }

        public bool Pass { get; set; }

        public CompletedChecklist () { }

        public CompletedChecklist(Checklist checklist, int machineSerialNumber, int employeeNumber) 
            : base(checklist.Name, checklist.TypeCode, checklist.Checks)
        {
            MachinSerialNumber = machineSerialNumber;
            EmployeeNumber = employeeNumber;
        }

        public int Faults()
        {
            Pass = true;
            int count = 0;
            foreach (Check check in Checks)
            {
                if (check.IsFault() ?? false)
                {
                    count++;
                    if (Pass) Pass = !check.FaultFails;
                }
            }
            return count;
        }
    }
}
