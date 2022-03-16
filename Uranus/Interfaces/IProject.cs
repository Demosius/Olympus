using Uranus.Staff;

namespace Uranus.Interfaces;

public interface IProject
{
    // ReSharper disable once ConvertToConstant.Local
    private static readonly bool requiresUser = false;
    public static bool RequiresUser => requiresUser;

    EProject Project { get; }
    public void RefreshData();
}