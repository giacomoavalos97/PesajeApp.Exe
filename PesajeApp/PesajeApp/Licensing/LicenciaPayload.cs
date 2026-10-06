namespace PesajeApp.Licensing;

/// <summary>
/// Datos firmados de una licencia de PesajeApp.
/// Debe mantenerse compatible con el contrato LICENSE_FORMAT.md.
/// </summary>
public sealed class LicenciaPayload
{
    public const int CurrentVersion = 1;
    public const string ProductPesajeApp = "PesajeApp";

    public int Version { get; init; }

    public required string Product { get; init; }

    public required string InstallationId { get; init; }

    public required string LicenseId { get; init; }

    /// <summary>
    /// Reconstruye exactamente los bytes canónicos definidos por
    /// LICENSE_FORMAT.md.
    /// </summary>
    public byte[] ToCanonicalBytes()
    {
        var json =
            "{\"Version\":" + Version +
            ",\"Product\":\"" + EscapeJson(Product) + "\"" +
            ",\"InstallationId\":\"" + EscapeJson(InstallationId) + "\"" +
            ",\"LicenseId\":\"" + EscapeJson(LicenseId) + "\"}";

        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    public string ToCanonicalString() =>
        System.Text.Encoding.UTF8.GetString(ToCanonicalBytes());

    private static string EscapeJson(string value) =>
        value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
}