using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class EmpresaView : Window
{
    private readonly EmpresaService _empresaService;

public EmpresaView()
    {
        InitializeComponent();

        _empresaService = new EmpresaService();

        Loaded += EmpresaView_Loaded;
    }

    private async void EmpresaView_Loaded(object sender, RoutedEventArgs e)
    {
        await CargarEmpresaAsync();
    }

    private async Task CargarEmpresaAsync()
    {
        try
        {
            var empresa = await _empresaService.ObtenerAsync();

            if (empresa == null)
            {
                NombreTextBlock.Text = "Sin configurar";
                TitulosTextBlock.Text = "Sin títulos configurados";
                return;
            }

            NombreTextBlock.Text =
                string.IsNullOrWhiteSpace(empresa.Nombre)
                    ? "Sin configurar"
                    : empresa.Nombre;

            var titulos = new List<string>();

            if (!string.IsNullOrWhiteSpace(empresa.Titulo2))
                titulos.Add(empresa.Titulo2);

            if (!string.IsNullOrWhiteSpace(empresa.Titulo3))
                titulos.Add(empresa.Titulo3);

            TitulosTextBlock.Text = titulos.Count > 0
                ? string.Join("\n", titulos)
                : "Sin títulos configurados";
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

    private async void Configurar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EmpresaForm
        {
            Owner = this
        };

        var resultado = ventana.ShowDialog();

        if (resultado == true)
        {
            await CargarEmpresaAsync();
        }
    }

    private void Cerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

}
