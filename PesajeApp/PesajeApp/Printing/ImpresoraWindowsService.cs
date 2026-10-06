using PdfiumPrinter;
using System.IO;

namespace PesajeApp.Printing;

public class ImpresoraWindowsService : IImpresoraService
{
    public async Task ImprimirAsync(
        string rutaArchivo,
        string nombreImpresora)
    {
        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException(
                "No se encontró el archivo que se desea imprimir.",
                rutaArchivo);
        }

        if (string.IsNullOrWhiteSpace(nombreImpresora))
        {
            throw new ArgumentException(
                "No se indicó una impresora.",
                nameof(nombreImpresora));
        }

        await Task.Run(() =>
        {
            var impresora = new PdfPrinter(nombreImpresora);

            impresora.Print(rutaArchivo);
        });
    }
}