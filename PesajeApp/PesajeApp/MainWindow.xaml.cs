using System.Windows;
using PesajeApp.Services;
using PesajeApp.Views;

namespace PesajeApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var usuario = SesionActual.Usuario;

        if (usuario == null)
        {
            MessageBox.Show(
                "No hay una sesión activa.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            Close();
            return;
        }

        UsuarioTextBlock.Text = $"Usuario: {usuario.Nombre}";
        SedeTextBlock.Text = $"Sede: {usuario.SedeNombre}";
        TurnoTextBlock.Text = $"Turno: {usuario.Turno}";
        RolTextBlock.Text = $"Rol: {usuario.Rol}";
    }

    private void AbrirSedes_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new SedesView();
        ventana.ShowDialog();
    }

    private void AbrirClientes_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new ClientesView();
        ventana.ShowDialog();
    }

    private void AbrirProductos_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new ProductosView();
        ventana.ShowDialog();
    }

    private void AbrirUsuarios_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new UsuariosView();
        ventana.Owner = this;
        ventana.ShowDialog();
    }

    private void Balanza_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new BalanzaView
        {
            Owner = this
        };

        ventana.ShowDialog();
    }

    private void Pesaje_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new PesajeForm
        {
            Owner = this
        };

        ventana.ShowDialog();
    }

    private void AbrirImpresora_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new ImpresoraView
        {
            Owner = this
        };

        ventana.ShowDialog();
    }

    private void AbrirEmpresa_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EmpresaView
        {
            Owner = this
        };

        ventana.ShowDialog();
    }

    private void BtnVerPesajes_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new PesajesView
        {
            Owner = this
        };

        ventana.ShowDialog();
    }
}