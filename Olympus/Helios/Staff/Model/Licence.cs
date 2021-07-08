using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Olympus.Helios.Staff.Model
{
    public class Licence
    {
        public string Number { get; set; } 
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool LF { get; set; }
        public bool LO { get; set; }
        public bool WP { get; set; }
        public Image Front { get; set; }
        public Image Back { get; set; }
        public Employee Employee { get; set; }

        public Licence() { }

        public Licence(string number, DateTime issueDate, DateTime expiryDate, bool lf, bool lo, bool wp, Image front, Image back, Employee employee)
        {
            Number = number;
            IssueDate = issueDate;
            ExpiryDate = expiryDate;
            LF = lf;
            LO = lo;
            WP = wp;
            Front = front;
            Back = back;
            Employee = employee;
        }
    }
}
