using Prometheus.ViewModels.Helpers;

namespace Prometheus.Model;

public class DataType
{
    public EDataType Type { get; set; }
    public string Name { get; set; }
    public DataCategory Category { get; set; }

    public DataType(DataCategory category, EDataType type)
    {
        Category = category;
        Type = type;
        Name = type.ToString();
    }
}