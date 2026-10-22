using PesajeApp.Services;
using System.Windows;
using System.Windows.Controls;

namespace PesajeApp.Views;

public partial class UsuariosView : Window
{
    private readonly UsuarioService _usuarioService;

    public UsuariosView()
    {
        InitializeComponent();

        _usuarioService = new UsuarioService();

        Loaded += UsuariosView_Loaded;
    }

    private async void UsuariosView_Loaded(object sender, RoutedEventArgs e)
    {
        await CargarUsuariosAsync();
    }

    private async Task CargarUsuariosAsync()
    {
        try
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();

            UsuariosDataGrid.ItemsSource = usuarios;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al cargar los usuarios:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void NuevoUsuario_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new UsuarioForm();

        ventana.Owner = this;

        var resultado = ventana.ShowDialog();

        if (resultado == true)
        {
            _ = CargarUsuariosAsync();
        }
    }

    private void EditarUsuario_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not Models.Usuario usuario)
            return;

        var ventana = new UsuarioForm(usuario);

        ventana.Owner = this;

        var resultado = ventana.ShowDialog();

        if (resultado == true)
        {
            _ = CargarUsuariosAsync();
        }
    }

    private async void DesactivarUsuario_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not Models.Usuario usuario)
            return;

        var respuesta = MessageBox.Show(
            $"¿Desea desactivar al usuario '{usuario.Nombre}'?",
            "PesajeApp",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (respuesta != MessageBoxResult.Yes)
            return;

        try
        {
            await _usuarioService.DesactivarAsync(usuario.Id);

            MessageBox.Show(
                "Usuario desactivado correctamente.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await CargarUsuariosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al desactivar el usuario:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}