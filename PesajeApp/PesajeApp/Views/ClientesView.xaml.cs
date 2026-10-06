using PesajeApp.Models;
using PesajeApp.Services;
using System.Windows;

namespace PesajeApp.Views;

public partial class ClientesView : Window
{
    private readonly ClienteService _clienteService;

    public ClientesView()
    {
        InitializeComponent();

        _clienteService = new ClienteService();

        Loaded += ClientesView_Loaded;
    }

    private async void ClientesView_Loaded(object sender, RoutedEventArgs e)
    {
        await CargarClientesAsync();
    }

    private async Task CargarClientesAsync()
    {
        try
        {
            var clientes = await _clienteService.ObtenerTodosAsync();

            ClientesDataGrid.ItemsSource = clientes;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al cargar los clientes:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Nuevo_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new ClienteForm
        {
            Owner = this
        };

        var resultado = ventana.ShowDialog();

        if (resultado == true)
        {
            await CargarClientesAsync();
        }
    }

    private async void Editar_Click(object sender, RoutedEventArgs e)
    {
        if (ClientesDataGrid.SelectedItem is not Cliente clienteSeleccionado)
        {
            MessageBox.Show(
                "Debe seleccionar un cliente.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var ventana = new ClienteForm(clienteSeleccionado)
        {
            Owner = this
        };

        var resultado = ventana.ShowDialog();

        if (resultado == true)
        {
            await CargarClientesAsync();
        }
    }

    private async void Desactivar_Click(object sender, RoutedEventArgs e)
    {
        if (ClientesDataGrid.SelectedItem is not Cliente clienteSeleccionado)
        {
            MessageBox.Show(
                "Debe seleccionar un cliente.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var resultado = MessageBox.Show(
            $"¿Está seguro de desactivar el cliente \"{clienteSeleccionado.Nombres}\"?",
            "Confirmar desactivación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await _clienteService.DesactivarAsync(clienteSeleccionado.Id);

            MessageBox.Show(
                "Cliente desactivado correctamente.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al desactivar el cliente:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
