namespace PesajeApp.Printing;

public class ImpresionService
{
    private readonly FastReportService _fastReportService;
    private readonly IImpresoraService _impresoraService;

    public ImpresionService()
    {
        _fastReportService = new FastReportService();
        _impresoraService = new ImpresoraWindowsService();
    }

    public async Task ImprimirPesajeAsync(int pesajeId)
    {
        string rutaPdf =
            await _fastReportService.GenerarPdfAsync(pesajeId);

        var impresoraService =
            new PesajeApp.Services.ImpresoraService();

        var impresora =
            await impresoraService.ObtenerAsync();

        if (impresora == null)
        {
            throw new InvalidOperationException(
                "No hay una impresora configurada.");
        }

        await _impresoraService.ImprimirAsync(
            rutaPdf,
            impresora.Nombre);
    }
}