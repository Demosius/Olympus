using Olympus.Prometheus.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Prometheus.Model
{
    class DataType
    {
        public EDataType Type { get; set; }
        public string Name { get; set; }
        public DataCategory Category { get; set; }

        public DataType() { }

        public DataType(DataCategory category) : this() 
        {
            Category = category;
        }

        public DataType(DataCategory category, EDataType type) : this(category)
        {
            Type = type;
            Name = type.ToString();
        }
    }
}
