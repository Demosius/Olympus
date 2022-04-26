using Uranus;

namespace Styx.Interfaces;

public interface IDataSource
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public void SetDataSources(Helios helios, Charon charon);
}