using System.Globalization;
using System.Text;

namespace PesajeApp.Services;

public class BalanzaParser
{
    private string _buffer = string.Empty;

    public event Action<decimal>? PesoRecibido;

    public void ProcesarDatos(string datos)
    {
        if (string.IsNullOrEmpty(datos))
            return;

        _buffer += datos;

        ProcesarBuffer();
    }

    private void ProcesarBuffer()
    {
        while (true)
        {
            int posicionG = _buffer.IndexOf(
                'g',
                StringComparison.OrdinalIgnoreCase);

            if (posicionG < 0)
                return;

            string lectura = _buffer[..posicionG];

            _buffer = _buffer[(posicionG + 1)..];

            string numero = ExtraerNumero(lectura);

            if (string.IsNullOrWhiteSpace(numero))
                continue;

            if (!decimal.TryParse(
                    numero,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal peso))
            {
                continue;
            }

            PesoRecibido?.Invoke(peso);
        }
    }

    private static string ExtraerNumero(string texto)
    {
        var resultado = new StringBuilder();

        foreach (char caracter in texto)
        {
            if (char.IsDigit(caracter) || caracter == '.')
            {
                resultado.Append(caracter);
            }
        }

        return resultado.ToString();
    }
}