using Uranus.Staff;

namespace Uranus
{
    public interface IProject
    {
        EProject EProject { get; }
        public void RefreshData();
    }
}
