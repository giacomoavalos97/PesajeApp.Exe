namespace PesajeApp.Printing;

public interface IImpresoraService
{
    Task ImprimirAsync(
        string rutaArchivo,
        string nombreImpresora);
}