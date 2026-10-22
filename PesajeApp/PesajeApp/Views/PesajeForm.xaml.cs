using PesajeApp.Models;
using PesajeApp.Printing;
using PesajeApp.Services;
using System.Windows;
using System.Windows.Controls;

namespace PesajeApp.Views;

public partial class PesajeForm : Window
{
    private readonly ProductoService _productoService;
    private readonly PesajeService _pesajeService;
    private readonly BalanzaManager _balanzaManager;
    private readonly ImpresionService _impresionService;

    private readonly FastReportService _fastReportService;

    private Producto? _productoSeleccionado;
    private decimal? _pesoActual;

    public PesajeForm()
    {
        InitializeComponent();

        _productoService = new ProductoService();
        _pesajeService = new PesajeService();
        _balanzaManager = App.Balanza;
        _impresionService = new ImpresionService();
        _fastReportService = new FastReportService();

        Loaded += PesajeForm_Loaded;
        Closed += PesajeForm_Closed;
    }

    private async void PesajeForm_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await CargarProductosAsync();

            _balanzaManager.PesoRecibido += BalanzaManager_PesoRecibido;

        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al iniciar el pesaje:\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void PesajeForm_Closed(object? sender, EventArgs e)
    {
        _balanzaManager.PesoRecibido -= BalanzaManager_PesoRecibido;
    }

    private async Task CargarProductosAsync()
    {
        var productos = await _productoService.ObtenerTodosAsync();

        ProductosDataGrid.ItemsSource = productos;
    }

    private void ProductosDataGrid_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        _productoSeleccionado =
            ProductosDataGrid.SelectedItem as Producto;

        CalcularImporte();
    }

    private void BalanzaManager_PesoRecibido(decimal peso)
    {
        Dispatcher.Invoke(() =>
        {
            _pesoActual = peso;

            PesoTextBlock.Text = peso.ToString("N3");

            CalcularPesoNeto();
            CalcularImporte();
        });
    }

    private void CalcularPesoNeto()
    {
        if (!_pesoActual.HasValue)
        {
            PesoNetoTextBox.Text = "0.000";
            return;
        }

        decimal tara = ObtenerTara();

        decimal pesoNeto = _pesoActual.Value - tara;

        if (pesoNeto < 0)
            pesoNeto = 0;

        PesoNetoTextBox.Text = pesoNeto.ToString("N3");
    }

    private decimal ObtenerTara()
    {
        if (decimal.TryParse(
            PesoTaraTextBox.Text,
            out decimal tara))
        {
            return tara;
        }

        return 0;
    }

    private void CalcularImporte()
    {
        if (!_pesoActual.HasValue ||
            _productoSeleccionado == null)
        {
            ImporteConTaraTextBlock.Text = "S/ 0.000";
            ImporteTextBlock.Text = "S/ 0.000";
            return;
        }

        decimal peso = _pesoActual.Value;
        decimal tara = ObtenerTara();
        decimal precio = _productoSeleccionado.Precio;

        // ==========================================
        // IMPORTE SIN TARA
        // ==========================================

        decimal importeSinTara = peso * precio;

        ImporteTextBlock.Text =
            $"S/ {importeSinTara:N3}";


        // ==========================================
        // IMPORTE CON TARA
        // ==========================================

        decimal pesoNeto = peso - tara;

        if (pesoNeto < 0)
            pesoNeto = 0;

        decimal importeConTara = pesoNeto * precio;

        ImporteConTaraTextBlock.Text =
            $"S/ {importeConTara:N3}";
    }

    private async void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        await GuardarPesajeAsync(false);
    }

    private async void GuardarEImprimir_Click(
        object sender,
        RoutedEventArgs e)
    {
        await GuardarPesajeAsync(true);
    }

    private async Task GuardarPesajeAsync(bool imprimir)
    {
        try
        {
            var usuario = SesionActual.Usuario;

            if (usuario == null)
            {
                MessageBox.Show(
                    "No hay una sesión activa.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (_productoSeleccionado == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un producto.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (!_balanzaManager.PesoActual.HasValue)
            {
                MessageBox.Show(
                    "No se ha recibido un peso de la balanza.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            decimal peso = _balanzaManager.PesoActual.Value;

            // Tara ingresada manualmente.
            decimal tara = ObtenerTara();

            // Peso neto = peso - tara.
            decimal pesoNeto = peso - tara;

            if (pesoNeto < 0)
                pesoNeto = 0;

            // Precio del producto seleccionado.
            decimal precio = _productoSeleccionado.Precio;

            // Importe almacenado = peso neto × precio.
            decimal importe = pesoNeto * precio;

            var pesaje = new Pesaje
            {
                SedeId = usuario.SedeId,
                ProductoId = _productoSeleccionado.Id,
                Cantidad = _productoSeleccionado.Cantidad,
                UsuarioId = usuario.Id,
                Turno = usuario.Turno,

                FechaLocal = DateTime.Now,

                Peso = peso,
                Precio = precio,
                Importe = importe,

                Estado = true,

                Lote = string.IsNullOrWhiteSpace(LoteTextBox.Text)
                    ? null
                    : LoteTextBox.Text.Trim(),

                CodAlt = string.IsNullOrWhiteSpace(CodAltTextBox.Text)
                    ? null
                    : CodAltTextBox.Text.Trim(),

                OP = string.IsNullOrWhiteSpace(OperacionTextBox.Text)
                    ? null
                    : OperacionTextBox.Text.Trim(),

                Kanban = string.IsNullOrWhiteSpace(KanbanTextBox.Text)
                    ? null
                    : KanbanTextBox.Text.Trim(),

                PesoTara = tara,
                PesoNeto = pesoNeto,

                FlagPrecio = 0,

                FlagLote = 0,

                FlagBar = 0,

                FlagFechaVencimiento = 0,

                FlagFechaPeriodo = 0,

                FlagPesoNeto = 0,

                FlagPesoTara = 0,

                EmpresaNombre = App.EmpresaActual?.Nombre,
                EmpresaTitulo2 = App.EmpresaActual?.Titulo2,
                EmpresaTitulo3 = App.EmpresaActual?.Titulo3,
            };

            // 1. Guardar el pesaje.
            int id = await _pesajeService.CrearAsync(pesaje);

            await _fastReportService.ActualizarEstructuraReporteAsync(id);

            // 2. Si se solicitó imprimir, generar el PDF desde FastReport.
            if (imprimir)
            {
                await _impresionService.ImprimirPesajeAsync(id);
            }

            // 3. Confirmar que el pesaje fue guardado.
            MessageBox.Show(
                $"Pesaje guardado correctamente.\n\nID: {id}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // 4. Limpiar el formulario.
            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al guardar el pesaje:\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private void LimpiarFormulario()
    {
        ProductosDataGrid.SelectedItem = null;

        _productoSeleccionado = null;

        PesoTaraTextBox.Clear();
        PesoNetoTextBox.Text = "0.000";
        LoteTextBox.Clear();

        ImporteTextBlock.Text = "S/ 0.000";
    }

    private void PesoTaraTextBox_TextChanged(
    object sender,
    TextChangedEventArgs e)
    {
        CalcularPesoNeto();
        CalcularImporte();
    }
}
