using Cadmus.ViewModels.Commands;

namespace Cadmus;

public interface IPrintable
{
    public PrintCommand PrintCommand { get; set; }
    void Print();
}