using System.Windows;
using PesajeApp.Models;
using PesajeApp.Services;

namespace PesajeApp.Views;

public partial class BalanzaView : Window
{
    private readonly BalanzaService _balanzaService;
    private Balanza? _balanza;

    protected override void OnClosed(EventArgs e)
    {
        App.Balanza.PesoRecibido -= BalanzaManager_PesoRecibido;
        base.OnClosed(e);
    }

    public BalanzaView()
    {
        InitializeComponent();

        _balanzaService = new BalanzaService();

        App.Balanza.PesoRecibido += BalanzaManager_PesoRecibido;

        Loaded += BalanzaView_Loaded;
    }

    private void BalanzaManager_PesoRecibido(decimal peso)
    {
        Dispatcher.Invoke(() =>
        {
            TerminalTextBox.AppendText(
                $"{peso:0.###} KG\r\n");

            TerminalTextBox.ScrollToEnd();
        });
    }

    private async void BalanzaView_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _balanza = await _balanzaService.ObtenerAsync();

            if (_balanza == null)
            {
                ModeloTextBox.Text = "CAS CI-201A";

                ModoComboBox.SelectedIndex = 0;

                PortNameTextBox.Text = "COM3";

                BaudRateTextBox.Text = "9600";

                DataBitsTextBox.Text = "8";

                ParityComboBox.SelectedIndex = 0;

                return;
            }

            ModeloTextBox.Text = _balanza.Modelo;

            SeleccionarComboBox(
                ModoComboBox,
                _balanza.Modo);

            PortNameTextBox.Text = _balanza.PortName;

            BaudRateTextBox.Text = _balanza.BaudRate.ToString();

            DataBitsTextBox.Text = _balanza.DataBits.ToString();

            SeleccionarComboBox(
                ParityComboBox,
                _balanza.Parity);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al cargar la configuración de la balanza:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static void SeleccionarComboBox(
        System.Windows.Controls.ComboBox comboBox,
        string valor)
    {
        foreach (System.Windows.Controls.ComboBoxItem item in comboBox.Items)
        {
            if (item.Content?.ToString() == valor)
            {
                comboBox.SelectedItem = item;
                return;
            }
        }
    }

    private async void Guardar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ModeloTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el modelo.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PortNameTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el puerto.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(BaudRateTextBox.Text, out var baudRate))
            {
                MessageBox.Show(
                    "BaudRate debe ser un número entero.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(DataBitsTextBox.Text, out var dataBits))
            {
                MessageBox.Show(
                    "DataBits debe ser un número entero.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (ModoComboBox.SelectedItem is not System.Windows.Controls.ComboBoxItem modoItem)
            {
                MessageBox.Show(
                    "Debe seleccionar un modo.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (ParityComboBox.SelectedItem is not System.Windows.Controls.ComboBoxItem parityItem)
            {
                MessageBox.Show(
                    "Debe seleccionar la paridad.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var balanza = new Balanza
            {
                Id = _balanza?.Id ?? 0,
                Modelo = ModeloTextBox.Text.Trim(),
                Modo = modoItem.Content?.ToString() ?? string.Empty,
                PortName = PortNameTextBox.Text.Trim(),
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parityItem.Content?.ToString() ?? string.Empty
            };

            if (_balanza == null)
            {
                await _balanzaService.GuardarAsync(balanza);

                _balanza = await _balanzaService.ObtenerAsync();
            }
            else
            {
                await _balanzaService.ActualizarAsync(balanza);

                _balanza = balanza;
            }

            MessageBox.Show(
                "Configuración guardada correctamente.",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al guardar la configuración:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ProbarConexion_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(PortNameTextBox.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el puerto.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!int.TryParse(BaudRateTextBox.Text, out var baudRate))
            {
                MessageBox.Show(
                    "BaudRate debe ser un número entero.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!int.TryParse(DataBitsTextBox.Text, out var dataBits))
            {
                MessageBox.Show(
                    "DataBits debe ser un número entero.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (ParityComboBox.SelectedItem
                is not System.Windows.Controls.ComboBoxItem parityItem)
            {
                MessageBox.Show(
                    "Debe seleccionar la paridad.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            string parity = parityItem.Content?.ToString() ?? "None";

            TerminalTextBox.Clear();

            var balanzaPrueba = new Balanza
            {
                Modelo = ModeloTextBox.Text.Trim(),
                Modo = ModoComboBox.Text,
                PortName = PortNameTextBox.Text.Trim(),
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity
            };

            App.Balanza.Iniciar(balanzaPrueba);

            TerminalTextBox.AppendText(
                $"Conectado a {PortNameTextBox.Text.Trim()}.\r\n");

            TerminalTextBox.AppendText(
                "Esperando datos de la balanza...\r\n");

            TerminalTextBox.ScrollToEnd();
        }
        catch (Exception ex)
        {
            TerminalTextBox.AppendText(
                $"ERROR: {ex.Message}\r\n");

            TerminalTextBox.ScrollToEnd();

            MessageBox.Show(
                $"No se pudo conectar con la balanza:\n\n{ex.Message}",
                "PesajeApp",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Cerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}