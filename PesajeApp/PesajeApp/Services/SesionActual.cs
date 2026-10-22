using PesajeApp.Models;

namespace PesajeApp.Services;

public static class SesionActual
{
    public static Usuario? Usuario { get; private set; }

    public static void Iniciar(Usuario usuario)
    {
        Usuario = usuario;
    }

    public static void Cerrar()
    {
        Usuario = null;
    }
}