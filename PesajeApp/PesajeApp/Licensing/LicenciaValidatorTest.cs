using System.Windows;

namespace PesajeApp.Licensing;

public static class LicenciaValidatorTest
{
    public static void Ejecutar(string licencia)
    {
        const string installationId = "A8F2-71C4-93D1-6B82";

        var validator = new LicenciaValidator();

        var resultado = validator.Validate(
            licencia,
            installationId);

        MessageBox.Show(
            $"¿Licencia válida?: {resultado.EsValida}\n\n" +
            $"Motivo: {resultado.Motivo}\n\n" +
            $"Product: {resultado.Payload?.Product}\n" +
            $"InstallationId: {resultado.Payload?.InstallationId}\n" +
            $"LicenseId: {resultado.Payload?.LicenseId}",
            "Prueba de licencia",
            MessageBoxButton.OK,
            resultado.EsValida
                ? MessageBoxImage.Information
                : MessageBoxImage.Error);
    }
}