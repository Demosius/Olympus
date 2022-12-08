using Cadmus.ViewModels.Commands;

namespace Cadmus;

public interface IPrintable
{
    public PrintCommand PrintCommand { get; set; }
    public ShowPrinterCommand ShowPrinterCommand { get; set; }
    void Print();
    void ShowPrinter();
}