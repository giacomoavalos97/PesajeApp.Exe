using System.Windows;
using System.Windows.Controls;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class PesajesView : Window
{
    private readonly PesajeService _pesajeService;
    private readonly SedeService _sedeService;

    public PesajesView()
    {
        InitializeComponent();

        _pesajeService = new PesajeService();
        _sedeService = new SedeService();

        Loaded += PesajesView_Loaded;
    }

    private async void PesajesView_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var sedes = await _sedeService.ObtenerActivasAsync();
            sedes.Insert(0, new Sede { Id = 0, Nombre = "Todas" });
            SedeComboBox.ItemsSource = sedes;
            SedeComboBox.SelectedValue = 0;

            await CargarPesajesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudieron cargar los filtros de pesajes.\n\n{ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Buscar_Click(object sender, RoutedEventArgs e)
    {
        await CargarPesajesAsync();
    }

    private async void Limpiar_Click(object sender, RoutedEventArgs e)
    {
        FechaDesdePicker.SelectedDate = null;
        FechaHastaPicker.SelectedDate = null;
        TurnoComboBox.SelectedIndex = 0;
        SedeComboBox.SelectedValue = 0;
        BuscarTextBox.Clear();

        await CargarPesajesAsync();
    }

    private async Task CargarPesajesAsync()
    {
        try
        {
            var fechaDesde = FechaDesdePicker.SelectedDate?.Date;
            var fechaHastaExclusiva = FechaHastaPicker.SelectedDate?.Date.AddDays(1);
            var turno = (TurnoComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var busqueda = string.IsNullOrWhiteSpace(BuscarTextBox.Text)
                ? null
                : BuscarTextBox.Text.Trim();
            var sedeId = SedeComboBox.SelectedValue is int selectedSedeId
                && selectedSedeId > 0
                    ? selectedSedeId
                    : (int?)null;

            var pesajes = await _pesajeService.ObtenerFiltradosAsync(
                fechaDesde,
                fechaHastaExclusiva,
                turno == "Todos" ? null : turno,
                busqueda,
                sedeId);

            PesajesDataGrid.ItemsSource = pesajes;

            var pesoNetoTotal = pesajes.Sum(p => p.PesoNeto ?? 0m);
            var importeTotal = pesajes.Sum(p => p.Importe ?? 0m);
            ResumenTextBlock.Text =
                $"Registros: {pesajes.Count}    Peso neto total: {pesoNetoTotal:N3}    Importe total: S/ {importeTotal:N3}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudieron cargar los pesajes.\n\n{ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
