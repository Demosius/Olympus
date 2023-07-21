using System.Threading.Tasks;
using Cadmus.ViewModels.Commands;

namespace Cadmus.Interfaces;

public interface IExport
{
    bool CanExport { get; }

    public ExportToCSVCommand ExportToCSVCommand { get; set; }
    public ExportToExcelCommand ExportToExcelCommand { get; set; }
    public ExportToLabelsCommand ExportToLabelsCommand { get; set; }
    public ExportToPDFCommand ExportToPDFCommand { get; set; }

    public Task ExportToPDF();
    public Task ExportToCSV();
    public Task ExportToExcel();
    public Task ExportToLabels();
}