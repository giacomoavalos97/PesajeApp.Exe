namespace PesajeApp.Licensing;

/// <summary>
/// Resultado de la validación de una licencia.
/// </summary>
public sealed record LicenciaValidationResult(
    bool EsValida,
    string Motivo,
    LicenciaPayload? Payload = null)
{
    public static LicenciaValidationResult Valid(
        LicenciaPayload payload)
    {
        return new LicenciaValidationResult(
            true,
            "Licencia válida.",
            payload);
    }

    public static LicenciaValidationResult Invalid(
        string motivo,
        LicenciaPayload? payload = null)
    {
        return new LicenciaValidationResult(
            false,
            motivo,
            payload);
    }
}