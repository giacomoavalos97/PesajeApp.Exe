using PesajeApp.Models;

namespace PesajeApp.Services;

public class BalanzaManager : IDisposable
{
    private BalanzaSerialService? _serialService;

    public decimal? PesoActual { get; private set; }

    public event Action<decimal>? PesoRecibido;

    public void Iniciar(Balanza balanza)
    {
        Detener();

        _serialService = new BalanzaSerialService();

        _serialService.PesoRecibido += peso =>
        {
            PesoActual = peso;
            PesoRecibido?.Invoke(peso);
        };

        _serialService.Conectar(
            balanza.PortName,
            balanza.BaudRate,
            balanza.DataBits,
            balanza.Parity);
    }

    public async Task IniciarDesdeConfiguracionAsync()
    {
        var servicio = new BalanzaService();

        var balanza = await servicio.ObtenerAsync();

        if (balanza == null)
            return;

        try
        {
            Iniciar(balanza);
        }
        catch
        {
            Detener();
        }
    }

    public void Detener()
    {
        if (_serialService == null)
            return;

        _serialService.Dispose();
        _serialService = null;

        PesoActual = null;
    }

    public void Dispose()
    {
        Detener();
    }
}