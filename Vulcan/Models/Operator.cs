using SQLite;
using Uranus.Staff.Models;

namespace Vulcan.Models;

internal class Operator
{
    [Ignore]
    public Employee Employee { get; set; }
    [Ignore]
    public int MyProperty { get; set; }


}