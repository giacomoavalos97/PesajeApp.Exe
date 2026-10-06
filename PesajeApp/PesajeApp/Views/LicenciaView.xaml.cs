
using System.Windows;
using PesajeApp.Licensing;

namespace PesajeApp.Views;

public partial class LicenciaView : Window
{
    private readonly LicenciaService _licenciaService;

    public LicenciaView()
    {
        InitializeComponent();

        _licenciaService = new LicenciaService();

        Loaded += LicenciaView_Loaded;
    }

    private void LicenciaView_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            TxtInstallationId.Text =
                _licenciaService.ObtenerInstallationId();

            TxtLicencia.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo obtener el identificador de instalación.\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            BtnActivar.IsEnabled = false;
            BtnCopiarId.IsEnabled = false;
        }
    }

    private void BtnCopiarId_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.SetText(TxtInstallationId.Text);

            MessageBox.Show(
                "Identificador copiado al portapapeles.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo copiar el identificador.\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void BtnActivar_Click(object sender, RoutedEventArgs e)
    {
        var licencia = TxtLicencia.Text.Trim();

        if (string.IsNullOrWhiteSpace(licencia))
        {
            MessageBox.Show(
                "Pega el código de licencia antes de continuar.",
                "Licencia requerida",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtLicencia.Focus();
            return;
        }

        BtnActivar.IsEnabled = false;

        try
        {
            var resultado = _licenciaService.Activar(licencia);

            if (!resultado.EsValida)
            {
                MessageBox.Show(
                    resultado.Motivo,
                    "No se pudo activar PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                TxtLicencia.Focus();
                return;
            }

            MessageBox.Show(
                "PesajeApp se ha activado correctamente.",
                "Activación completada",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ocurrió un error al guardar o validar la licencia.\n\n{ex.Message}",
                "Error de activación",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            BtnActivar.IsEnabled = true;
        }
    }
}