using FastReport;
using FastReport.Export.PdfSimple;
using PesajeApp.Services;
using System.IO;
using System.Windows;

namespace PesajeApp.Printing;

public class FastReportService
{
    private readonly PesajeService _pesajeService;

    public FastReportService()
    {
        _pesajeService = new PesajeService();
    }

    public async Task<string> GenerarPdfAsync(int pesajeId)
    {
        string rutaReporte = Path.Combine(
            AppContext.BaseDirectory,
            "Reports",
            "Pesaje.frx");

        if (!File.Exists(rutaReporte))
        {
            throw new FileNotFoundException(
                "No se encontró el archivo de reporte.",
                rutaReporte);
        }

        var datos =
            await _pesajeService.ObtenerDatosReporteAsync(pesajeId);

        if (datos == null)
        {
            throw new InvalidOperationException(
                $"No se encontró el pesaje con ID {pesajeId}.");
        }

        using var reporte = new Report();

        reporte.Load(rutaReporte);

        reporte.RegisterData(
            new[] { datos },
            "Pesaje");

        reporte.Prepare();

        string carpetaTemporal =
            Path.Combine(
                Path.GetTempPath(),
                "PesajeApp");

        Directory.CreateDirectory(carpetaTemporal);

        string rutaPdf = Path.Combine(
            carpetaTemporal,
            $"Pesaje_{pesajeId}.pdf");

        using var exportador = new PDFSimpleExport();

        reporte.Export(
            exportador,
            rutaPdf);

        return rutaPdf;
    }

    public async Task ActualizarEstructuraReporteAsync(int pesajeId)
    {
        string rutaReporte = Path.Combine(
            AppContext.BaseDirectory,
            "Reports",
            "Pesaje.frx");

        var datos = await _pesajeService.ObtenerDatosReporteAsync(pesajeId);

        if (datos == null)
        {
            throw new InvalidOperationException(
                $"No se encontró el pesaje con ID {pesajeId}.");
        }

        using var reporte = new Report();

        reporte.Load(rutaReporte);

        reporte.RegisterData(
            new[] { datos },
            "Pesaje");

        string rutaNueva = Path.Combine(
            AppContext.BaseDirectory,
            "Reports",
            "Pesaje_con_datos.frx");

        reporte.Save(rutaNueva);

        /*MessageBox.Show(
            $"El archivo se creó correctamente en:\n\n{rutaNueva}",
            "PesajeApp",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        */
    }


}