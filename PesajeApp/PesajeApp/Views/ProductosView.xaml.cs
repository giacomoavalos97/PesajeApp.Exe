using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class ProductosView : Window
{
    private readonly ProductoService _productoService;

    public ProductosView()
    {
        InitializeComponent();

        _productoService = new ProductoService();

        Loaded += ProductosView_Loaded;
    }

    private async void ProductosView_Loaded(object sender, RoutedEventArgs e)
    {
        await CargarProductosAsync();
    }

    private async Task CargarProductosAsync()
    {
        try
        {
            var productos = await _productoService.ObtenerTodosAsync();

            ProductosDataGrid.ItemsSource = productos;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al cargar los productos:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Nuevo_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new ProductoForm
        {
            Owner = this
        };

        var resultado = ventana.ShowDialog();

        if (resultado == true)
        {
            await CargarProductosAsync();
        }
    }

    private async void Editar_Click(object sender, RoutedEventArgs e)
    {
        if (ProductosDataGrid.SelectedItem is not Producto productoSeleccionado)
        {
            MessageBox.Show(
                "Debe seleccionar un producto.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var ventana = new ProductoForm(productoSeleccionado)
        {
            Owner = this
        };

        var resultado = ventana.ShowDialog();

        if (resultado == true)
        {
            await CargarProductosAsync();
        }
    }

    private async void Desactivar_Click(object sender, RoutedEventArgs e)
    {
        if (ProductosDataGrid.SelectedItem is not Producto productoSeleccionado)
        {
            MessageBox.Show(
                "Debe seleccionar un producto.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var resultado = MessageBox.Show(
            $"¿Está seguro de desactivar el producto \"{productoSeleccionado.Descripcion}\"?",
            "Confirmar desactivación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await _productoService.DesactivarAsync(productoSeleccionado.Id);

            MessageBox.Show(
                "Producto desactivado correctamente.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await CargarProductosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al desactivar el producto:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}