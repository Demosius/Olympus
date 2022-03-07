using Prometheus.ViewModel.Helpers;

namespace Prometheus.Model;

public class DataType
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