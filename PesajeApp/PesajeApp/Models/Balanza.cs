namespace PesajeApp.Models;

public class Balanza
{
    public int Id { get; set; }

    public string Modelo { get; set; } = string.Empty;

    public string Modo { get; set; } = string.Empty;

    public string PortName { get; set; } = string.Empty;

    public int BaudRate { get; set; }

    public int DataBits { get; set; }

    public string Parity { get; set; } = string.Empty;
}