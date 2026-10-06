using System.Printing;
using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class ImpresoraView : Window
{
    private readonly ImpresoraService _impresoraService;

    public ImpresoraView()
    {
        InitializeComponent();

        _impresoraService = new ImpresoraService();

        CargarImpresoras();
    }

    private async void CargarImpresoras()
    {
        try
        {
            // Obtener impresoras instaladas en Windows
            LocalPrintServer servidor = new LocalPrintServer();

            var impresorasWindows = servidor
                .GetPrintQueues()
                .Select(p => p.Name)
                .ToList();

            // Convertirlas a nuestro modelo
            var impresoras = impresorasWindows
                .Select(nombre => new Impresora
                {
                    Nombre = nombre,
                    Estado = true
                })
                .ToList();

            ImpresoraComboBox.ItemsSource = impresoras;

            // Obtener la impresora guardada en PostgreSQL
            var impresoraGuardada = await _impresoraService.ObtenerAsync();

            if (impresoraGuardada != null)
            {
                var seleccionada = impresoras
                    .FirstOrDefault(x =>
                        x.Nombre.Equals(
                            impresoraGuardada.Nombre,
                            StringComparison.OrdinalIgnoreCase));

                if (seleccionada != null)
                {
                    ImpresoraComboBox.SelectedItem = seleccionada;
                }

                DescripcionTextBox.Text =
                    impresoraGuardada.Descripcion ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudieron cargar las impresoras:\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            if (ImpresoraComboBox.SelectedItem is not Impresora impresora)
            {
                MessageBox.Show(
                    "Debe seleccionar una impresora.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var impresoraGuardada =
                await _impresoraService.ObtenerAsync();

            if (impresoraGuardada == null)
            {
                await _impresoraService.CrearAsync(
                    new Impresora
                    {
                        Nombre = impresora.Nombre,
                        Descripcion =
                            string.IsNullOrWhiteSpace(
                                DescripcionTextBox.Text)
                                ? null
                                : DescripcionTextBox.Text.Trim(),
                        Estado = true
                    });
            }
            else
            {
                impresoraGuardada.Nombre = impresora.Nombre;

                impresoraGuardada.Descripcion =
                    string.IsNullOrWhiteSpace(
                        DescripcionTextBox.Text)
                        ? null
                        : DescripcionTextBox.Text.Trim();

                await _impresoraService.ActualizarAsync(
                    impresoraGuardada);
            }

            MessageBox.Show(
                $"Impresora guardada correctamente.\n\n{impresora.Nombre}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al guardar la impresora:\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Cancelar_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}
