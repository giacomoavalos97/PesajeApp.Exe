using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class ClienteForm : Window
{
    private readonly ClienteService _clienteService;
    private readonly SedeService _sedeService;
    private readonly Cliente? _clienteEditar;

    public ClienteForm(Cliente? cliente = null)
    {
        InitializeComponent();

        _clienteService = new ClienteService();
        _sedeService = new SedeService();
        _clienteEditar = cliente;

        if (_clienteEditar != null)
        {
            Title = "Editar Cliente";

            CodigoTextBox.Text = _clienteEditar.Cod?.ToString();
            NombresTextBox.Text = _clienteEditar.Nombres;
        }

        Loaded += ClienteForm_Loaded;
    }

    private async void ClienteForm_Loaded(object sender, RoutedEventArgs e)
    {
        await CargarSedesAsync();

        if (_clienteEditar != null)
        {
            SedeComboBox.SelectedValue = _clienteEditar.SedeId;
        }
    }

    private async Task CargarSedesAsync()
    {
        try
        {
            var sedes = await _sedeService.ObtenerActivasAsync();

            SedeComboBox.ItemsSource = sedes;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al cargar las sedes:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
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

            if (string.IsNullOrWhiteSpace(NombresTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar los nombres.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (SedeComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Debe seleccionar una sede.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (_clienteEditar == null)
            {
                // NUEVO CLIENTE

                var cliente = new Cliente
                {
                    Cod = codigo,
                    Nombres = NombresTextBox.Text.Trim(),
                    SedeId = Convert.ToInt32(SedeComboBox.SelectedValue),
                    Estado = true
                };

                var id = await _clienteService.CrearAsync(cliente);

                MessageBox.Show(
                    $"Cliente creado correctamente.\n\nID generado: {id}",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                // EDITAR CLIENTE

                _clienteEditar.Cod = codigo;
                _clienteEditar.Nombres = NombresTextBox.Text.Trim();
                _clienteEditar.SedeId =
                    Convert.ToInt32(SedeComboBox.SelectedValue);

                await _clienteService.ActualizarAsync(_clienteEditar);

                MessageBox.Show(
                    "Cliente actualizado correctamente.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al guardar el cliente:\n\n{ex.Message}",
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