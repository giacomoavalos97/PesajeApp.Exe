using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class SedeForm : Window
{
    private readonly SedeService _sedeService;
    private readonly Sede? _sedeEditar;

    public SedeForm(Sede? sede = null)
    {
        InitializeComponent();

        _sedeService = new SedeService();
        _sedeEditar = sede;

        if (_sedeEditar != null)
        {
            Title = "Editar Sede";

            CodigoTextBox.Text = _sedeEditar.Cod.ToString();
            NombreTextBox.Text = _sedeEditar.Nombre;
            DescripcionTextBox.Text = _sedeEditar.Descripcion;
        }
    }

    private async void Guardar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!long.TryParse(CodigoTextBox.Text, out long codigo))
            {
                MessageBox.Show(
                    "El código debe ser un número.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(NombreTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar un nombre.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (_sedeEditar == null)
            {
                // NUEVA SEDE

                var sede = new Sede
                {
                    Cod = codigo,
                    Nombre = NombreTextBox.Text.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(DescripcionTextBox.Text)
                        ? null
                        : DescripcionTextBox.Text.Trim(),
                    Estado = true
                };

                var id = await _sedeService.CrearAsync(sede);

                MessageBox.Show(
                    $"Sede creada correctamente.\n\nID generado: {id}",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                // EDITAR SEDE

                _sedeEditar.Cod = codigo;
                _sedeEditar.Nombre = NombreTextBox.Text.Trim();
                _sedeEditar.Descripcion = string.IsNullOrWhiteSpace(DescripcionTextBox.Text)
                    ? null
                    : DescripcionTextBox.Text.Trim();

                await _sedeService.ActualizarAsync(_sedeEditar);

                MessageBox.Show(
                    "Sede actualizada correctamente.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al guardar la sede:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
