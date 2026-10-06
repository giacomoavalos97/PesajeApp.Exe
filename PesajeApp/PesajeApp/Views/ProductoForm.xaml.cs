using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class ProductoForm : Window
{
    private readonly ProductoService _productoService;
    private readonly SedeService _sedeService;

    private readonly Producto? _productoEditar;

    public ProductoForm(Producto? producto = null)
    {
        InitializeComponent();

        _productoService = new ProductoService();
        _sedeService = new SedeService();
        _productoEditar = producto;

        if (_productoEditar != null)
        {
            Title = "Editar Producto";

            CodigoTextBox.Text = _productoEditar.Cod?.ToString();
            DescripcionTextBox.Text = _productoEditar.Descripcion;
            PrecioTextBox.Text = _productoEditar.Precio.ToString();
            CaducidadTextBox.Text = _productoEditar.Caducidad.ToString();
            CantidadTextBox.Text = _productoEditar.Cantidad?.ToString();
        }

        Loaded += ProductoForm_Loaded;
    }

    private async void ProductoForm_Loaded(object sender, RoutedEventArgs e)
    {
        await CargarSedesAsync();

        if (_productoEditar != null)
        {
            SedeComboBox.SelectedValue = _productoEditar.SedeId;
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
            if (!int.TryParse(CodigoTextBox.Text, out int codigo))
            {
                MessageBox.Show(
                    "El código debe ser un número.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(DescripcionTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar una descripción.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!decimal.TryParse(PrecioTextBox.Text, out decimal precio))
            {
                MessageBox.Show(
                    "El precio debe ser un número válido.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!int.TryParse(CaducidadTextBox.Text, out int caducidad))
            {
                MessageBox.Show(
                    "La caducidad debe ser un número.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!int.TryParse(CantidadTextBox.Text, out int cantidad))
            {
                MessageBox.Show(
                    "La cantidad debe ser un número.",
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

            if (_productoEditar == null)
            {
                var producto = new Producto
                {
                    Cod = codigo,
                    Descripcion = DescripcionTextBox.Text.Trim(),
                    Precio = precio,
                    Caducidad = caducidad,
                    Cantidad = cantidad,
                    SedeId = Convert.ToInt32(SedeComboBox.SelectedValue),
                    Estado = true
                };

                var id = await _productoService.CrearAsync(producto);

                MessageBox.Show(
                    $"Producto creado correctamente.\n\nID generado: {id}",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                _productoEditar.Cod = codigo;
                _productoEditar.Descripcion = DescripcionTextBox.Text.Trim();
                _productoEditar.Precio = precio;
                _productoEditar.Caducidad = caducidad;
                _productoEditar.Cantidad = cantidad;
                _productoEditar.SedeId =
                    Convert.ToInt32(SedeComboBox.SelectedValue);

                await _productoService.ActualizarAsync(_productoEditar);

                MessageBox.Show(
                    "Producto actualizado correctamente.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al guardar el producto:\n\n{ex.Message}",
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