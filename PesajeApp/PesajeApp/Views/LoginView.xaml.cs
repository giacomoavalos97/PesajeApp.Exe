using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class LoginView : Window
{
    private readonly UsuarioService _usuarioService;

    public LoginView()
    {
        InitializeComponent();

        _usuarioService = new UsuarioService();
    }

    private async void Ingresar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(UsuarioTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el usuario.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(ClavePasswordBox.Password))
            {
                MessageBox.Show(
                    "Debe ingresar la clave.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var usuario = await _usuarioService.AutenticarAsync(
                UsuarioTextBox.Text.Trim(),
                ClavePasswordBox.Password);

            if (usuario == null)
            {
                MessageBox.Show(
                    "Usuario o clave incorrectos.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show(
                $"Bienvenido, {usuario.Nombre}.\n\n" +
                $"Sede: {usuario.SedeNombre}\n" +
                $"Turno: {usuario.Turno}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            SesionActual.Iniciar(usuario);

            var mainWindow = new MainWindow();

            Application.Current.MainWindow = mainWindow;

            mainWindow.Show();

            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al iniciar sesión:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}