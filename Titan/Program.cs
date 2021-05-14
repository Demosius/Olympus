using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympus.Helios.Inventory;

namespace Titan
{
    class Program
    {
        static void Main(string[] args)
        {
            Bay bay = new Bay();
            bay.Name = "Francis";
            Console.WriteLine(bay.Name);
            Console.ReadLine();
        }
    }
}
