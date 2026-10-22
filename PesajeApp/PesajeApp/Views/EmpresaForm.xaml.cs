using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class EmpresaForm : Window
{
    private readonly EmpresaService _empresaService;

    private Empresa? _empresa;

    public EmpresaForm()
    {
        InitializeComponent();

        _empresaService = new EmpresaService();

        Loaded += EmpresaForm_Loaded;
    }

    private async void EmpresaForm_Loaded(object sender, RoutedEventArgs e)
    {
        await CargarEmpresaAsync();
    }

    private async Task CargarEmpresaAsync()
    {
        try
        {
            _empresa = await _empresaService.ObtenerAsync();

            if (_empresa == null)
                return;

            NombreTextBox.Text = _empresa.Nombre;
            Titulo2TextBox.Text = _empresa.Titulo2;
            Titulo3TextBox.Text = _empresa.Titulo3;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al cargar los datos de la empresa:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Guardar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NombreTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre de la empresa.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (_empresa == null)
            {
                var empresa = new Empresa
                {
                    Nombre = NombreTextBox.Text.Trim(),

                    Titulo2 = string.IsNullOrWhiteSpace(Titulo2TextBox.Text)
                        ? null
                        : Titulo2TextBox.Text.Trim(),

                    Titulo3 = string.IsNullOrWhiteSpace(Titulo3TextBox.Text)
                        ? null
                        : Titulo3TextBox.Text.Trim()
                };

                var id = await _empresaService.CrearAsync(empresa);

                empresa.Id = id;
                _empresa = empresa;

                // Actualizar empresa global de la aplicación
                App.EmpresaActual = empresa;

                MessageBox.Show(
                    "Datos de la empresa guardados correctamente.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                _empresa.Nombre = NombreTextBox.Text.Trim();

                _empresa.Titulo2 =
                    string.IsNullOrWhiteSpace(Titulo2TextBox.Text)
                        ? null
                        : Titulo2TextBox.Text.Trim();

                _empresa.Titulo3 =
                    string.IsNullOrWhiteSpace(Titulo3TextBox.Text)
                        ? null
                        : Titulo3TextBox.Text.Trim();

                await _empresaService.ActualizarAsync(_empresa);

                // Actualizar empresa global de la aplicación
                App.EmpresaActual = _empresa;

                MessageBox.Show(
                    "Datos de la empresa actualizados correctamente.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al guardar los datos de la empresa:\n\n{ex.Message}",
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