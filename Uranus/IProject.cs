using Uranus.Staff;

namespace Uranus
{
    public interface IProject
    {
        EProject EProject { get; }
        public bool RequiresUser { get; }
        public void RefreshData();
    }
}
