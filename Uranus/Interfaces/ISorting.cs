using Uranus.Commands;

namespace Uranus.Interfaces;

public interface ISorting
{
    public ApplySortingCommand ApplySortingCommand { get; set; }
    
    void ApplySorting();
}