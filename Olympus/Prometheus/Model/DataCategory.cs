using Olympus.Prometheus.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Prometheus.Model
{
    class DataCategory
    {
        public EDataCategory Category { get; }
        public string Name { get; set; }
        public List<DataType> DataTypes { get; set; }

        public DataCategory() { }

        public DataCategory(EDataCategory category) : this()
        {
            Category = category;
            Name = EnumConverter.DataCategoryToString(category);

            DataTypes = new List<DataType>();
            foreach (var type in EnumConverter.GetTypeList(category))
            {
                DataTypes.Add(new DataType(this, type));
            }
        }
    }
}
