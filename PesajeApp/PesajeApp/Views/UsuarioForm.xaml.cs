using PesajeApp.Models;
using PesajeApp.Services;
using System.Windows;
using System.Windows.Controls;

namespace PesajeApp.Views;

public partial class UsuarioForm : Window
{
    private readonly SedeService _sedeService;
    private readonly UsuarioService _usuarioService;
    private readonly Usuario? _usuarioEditar;

    public UsuarioForm(Usuario? usuario = null)
    {
        InitializeComponent();

        _sedeService = new SedeService();
        _usuarioService = new UsuarioService();

        _usuarioEditar = usuario;

        TurnoComboBox.SelectedIndex = 0;
        RolComboBox.SelectedIndex = 0;

        if (_usuarioEditar != null)
        {
            Title = "Editar usuario";
        }

        Loaded += UsuarioForm_Loaded;
    }

    private async void UsuarioForm_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var sedes = await _sedeService.ObtenerActivasAsync();

            SedeComboBox.ItemsSource = sedes;

            if (_usuarioEditar == null)
            {
                if (sedes.Count > 0)
                {
                    SedeComboBox.SelectedIndex = 0;
                }

                return;
            }

            NombreTextBox.Text = _usuarioEditar.Nombre;
            ClavePasswordBox.Password = _usuarioEditar.Clave;

            SedeComboBox.SelectedValue = _usuarioEditar.SedeId;

            foreach (ComboBoxItem item in RolComboBox.Items)
            {
                if (item.Content?.ToString() == _usuarioEditar.Rol)
                {
                    RolComboBox.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in TurnoComboBox.Items)
            {
                if (item.Content?.ToString() == _usuarioEditar.Turno)
                {
                    TurnoComboBox.SelectedItem = item;
                    break;
                }
            }
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

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private async void Guardar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NombreTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre de usuario.",
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

            if (SedeComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Debe seleccionar una sede.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (RolComboBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un rol.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (TurnoComboBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un turno.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var rol = (RolComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)
                ?.Content?.ToString();

            var turno = (TurnoComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)
                ?.Content?.ToString();

            var usuario = new Usuario
            {
                SedeId = Convert.ToInt32(SedeComboBox.SelectedValue),
                Nombre = NombreTextBox.Text.Trim(),
                Clave = ClavePasswordBox.Password,
                Rol = rol,
                Turno = turno ?? string.Empty,
                Estado = true
            };

            if (_usuarioEditar == null)
            {
                await _usuarioService.CrearAsync(usuario);

                MessageBox.Show(
                    "Usuario creado correctamente.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                usuario.Id = _usuarioEditar.Id;

                await _usuarioService.ActualizarAsync(usuario);

                MessageBox.Show(
                    "Usuario actualizado correctamente.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al crear el usuario:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

}