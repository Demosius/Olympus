using Uranus.Staff;

namespace Uranus
{
    public interface IProject
    {
        EProject Project { get; }
        public bool RequiresUser { get; }
        public void RefreshData();
    }
}
