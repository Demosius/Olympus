namespace Pantheon.ViewModels.Interface;

public interface ISelector
{
    public bool CanCreate { get; }
    public bool CanDelete { get; }
    public bool CanConfirm { get; }

    void Create();
    void Delete();
}