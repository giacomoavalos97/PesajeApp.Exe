using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace PesajeApp.Licensing;

/// <summary>
/// Obtiene un identificador estable asociado a la instalación de Windows.
/// </summary>
public sealed class InstallationIdService
{
    private const string RegistryPath =
        @"SOFTWARE\Microsoft\Cryptography";

    private const string RegistryValueName =
        "MachineGuid";

    /// <summary>
    /// Obtiene el InstallationId de esta instalación de Windows.
    /// </summary>
    public string ObtenerInstallationId()
    {
        var machineGuid = ObtenerMachineGuid();

        if (string.IsNullOrWhiteSpace(machineGuid))
        {
            throw new InvalidOperationException(
                "No se pudo obtener el identificador de instalación de Windows.");
        }

        return GenerarInstallationId(machineGuid);
    }

    private static string ObtenerMachineGuid()
    {
        using var key = Registry.LocalMachine.OpenSubKey(RegistryPath);

        var value = key?.GetValue(RegistryValueName);

        return value?.ToString()?.Trim() ?? string.Empty;
    }

    private static string GenerarInstallationId(string machineGuid)
    {
        var bytes = Encoding.UTF8.GetBytes(machineGuid);

        var hash = SHA256.HashData(bytes);

        // Utilizamos los primeros 16 bytes del SHA-256.
        // 16 bytes = 32 caracteres hexadecimales.
        var hex = Convert.ToHexString(hash[..16]);

        // Convertimos los 32 caracteres en 8 grupos de 4.
        var grupos = Enumerable
            .Range(0, 8)
            .Select(i => hex.Substring(i * 4, 4));

        return string.Join("-", grupos);
    }
}