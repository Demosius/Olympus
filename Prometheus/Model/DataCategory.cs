using Prometheus.ViewModels.Helpers;
using System.Collections.Generic;

namespace Prometheus.Model;

public class DataCategory
{
    public EDataCategory Category { get; }
    public string Name { get; set; }
    public List<DataType> DataTypes { get; set; }

    public DataCategory()
    {
        Name = string.Empty;
        DataTypes = new List<DataType>();
    }

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