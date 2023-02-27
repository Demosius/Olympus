using Cadmus.ViewModels.Commands;

namespace Cadmus.Interfaces;

public interface IPrintable
{
    public PrintCommand PrintCommand { get; set; }
    void Print();
}