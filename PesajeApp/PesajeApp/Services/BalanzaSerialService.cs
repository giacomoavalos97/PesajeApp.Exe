using System.IO.Ports;

namespace PesajeApp.Services;

public class BalanzaSerialService : IDisposable
{
    private SerialPort? _serialPort;

    private readonly BalanzaParser _parser;

    public event Action<decimal>? PesoRecibido;

    public bool EstaConectada => _serialPort?.IsOpen == true;

    public BalanzaSerialService()
    {
        _parser = new BalanzaParser();

        _parser.PesoRecibido += peso =>
        {
            PesoRecibido?.Invoke(peso);
        };
    }

    public void Conectar(
        string portName,
        int baudRate,
        int dataBits,
        string parity)
    {
        Desconectar();

        var paridad = parity switch
        {
            "Odd" => Parity.Odd,
            "Even" => Parity.Even,
            _ => Parity.None
        };

        _serialPort = new SerialPort(
            portName,
            baudRate,
            paridad,
            dataBits);

        _serialPort.DataReceived += SerialPort_DataReceived;

        _serialPort.Open();
    }

    private void SerialPort_DataReceived(
    object sender,
    SerialDataReceivedEventArgs e)
    {
        if (_serialPort == null || !_serialPort.IsOpen)
            return;

        string datos = _serialPort.ReadExisting();

        if (string.IsNullOrEmpty(datos))
            return;

        _parser.ProcesarDatos(datos);
    }

    public void Desconectar()
    {
        if (_serialPort == null)
            return;

        try
        {
            _serialPort.DataReceived -= SerialPort_DataReceived;

            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _serialPort.Dispose();
        }
        finally
        {
            _serialPort = null;
        }
    }

    public void Dispose()
    {
        Desconectar();
    }
}