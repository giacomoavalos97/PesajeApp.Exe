using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PesajeApp.Licensing;

/// <summary>
/// Valida licencias PAL1 de PesajeApp usando únicamente la clave pública
/// embebida en el ejecutable.
/// </summary>
public sealed class LicenciaValidator
{
    private const string LicensePrefix = "PAL1.";
    private const int MaxLicenseLength = 8192;

    /// <summary>
    /// Valida una licencia completa contra la instalación actual.
    /// </summary>
    /// <param name="licenseText">Código de licencia PAL1.</param>
    /// <param name="installationIdEsperado">
    /// InstallationId correspondiente a esta instalación de PesajeApp.
    /// </param>
    public LicenciaValidationResult Validate(
        string licenseText,
        string installationIdEsperado)
    {
        if (licenseText is null)
        {
            return LicenciaValidationResult.Invalid(
                "La licencia no puede ser nula.");
        }

        // 1. Trim exterior
        licenseText = licenseText.Trim();

        // 2. Saltos de línea internos no permitidos
        if (licenseText.Contains('\n') || licenseText.Contains('\r'))
        {
            return LicenciaValidationResult.Invalid(
                "La licencia contiene saltos de línea no permitidos.");
        }

        // 3. Longitud máxima
        if (licenseText.Length > MaxLicenseLength)
        {
            return LicenciaValidationResult.Invalid(
                "La licencia excede la longitud máxima permitida.");
        }

        // 4. Prefijo
        if (!licenseText.StartsWith(
                LicensePrefix,
                StringComparison.Ordinal))
        {
            return LicenciaValidationResult.Invalid(
                "La licencia no tiene un prefijo PAL1 válido.");
        }

        // 5. Deben existir exactamente 3 partes
        var parts = licenseText.Split('.');

        if (parts.Length != 3 ||
            !string.Equals(parts[0], "PAL1", StringComparison.Ordinal) ||
            string.IsNullOrEmpty(parts[1]) ||
            string.IsNullOrEmpty(parts[2]))
        {
            return LicenciaValidationResult.Invalid(
                "El formato de la licencia no es válido.");
        }

        byte[] payloadBytes;
        byte[] signatureBytes;

        // 6 y 7. Decodificar Base64URL
        try
        {
            payloadBytes = DecodeBase64Url(parts[1]);
            signatureBytes = DecodeBase64Url(parts[2]);
        }
        catch (FormatException ex)
        {
            return LicenciaValidationResult.Invalid(
                $"La licencia contiene Base64URL inválido: {ex.Message}");
        }

        // 8 y 9. Parsear y comprobar representación canónica
        LicenciaPayload payload;

        try
        {
            payload = ParseCanonicalPayload(payloadBytes);
        }
        catch (Exception ex)
        {
            return LicenciaValidationResult.Invalid(
                $"Payload de licencia no válido: {ex.Message}");
        }

        // 10. Cargar clave pública embebida
        using var publicKey = LoadPublicKey();

        if (publicKey is null)
        {
            return LicenciaValidationResult.Invalid(
                "No se pudo cargar la clave pública de PesajeApp.");
        }

        // 11. Verificar firma RSA-SHA256 PKCS#1 v1.5
        bool signatureValid;

        try
        {
            signatureValid = publicKey.VerifyData(
                payloadBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch (CryptographicException)
        {
            signatureValid = false;
        }

        if (!signatureValid)
        {
            return LicenciaValidationResult.Invalid(
                "Firma de licencia inválida.",
                payload);
        }

        // 12. Producto
        if (!string.Equals(
                payload.Product,
                LicenciaPayload.ProductPesajeApp,
                StringComparison.Ordinal))
        {
            return LicenciaValidationResult.Invalid(
                $"Producto no soportado: '{payload.Product}'.",
                payload);
        }

        // 13. InstallationId
        if (!string.Equals(
                payload.InstallationId,
                installationIdEsperado,
                StringComparison.Ordinal))
        {
            return LicenciaValidationResult.Invalid(
                "La licencia no corresponde a esta instalación.",
                payload);
        }

        return LicenciaValidationResult.Valid(payload);
    }

    /// <summary>
    /// Verifica únicamente la firma criptográfica de una licencia.
    /// No comprueba InstallationId.
    /// </summary>
    public bool VerifySignatureOnly(string licenseText)
    {
        try
        {
            licenseText = licenseText.Trim();

            if (licenseText.Length > MaxLicenseLength)
                return false;

            if (licenseText.Contains('\n') ||
                licenseText.Contains('\r'))
            {
                return false;
            }

            if (!licenseText.StartsWith(
                    LicensePrefix,
                    StringComparison.Ordinal))
            {
                return false;
            }

            var parts = licenseText.Split('.');

            if (parts.Length != 3)
                return false;

            var payloadBytes = DecodeBase64Url(parts[1]);
            var signatureBytes = DecodeBase64Url(parts[2]);

            // También exigimos payload canónico.
            ParseCanonicalPayload(payloadBytes);

            using var publicKey = LoadPublicKey();

            return publicKey.VerifyData(
                payloadBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Reconstruye el payload y exige coincidencia byte por byte
    /// con la representación canónica definida por LICENSE_FORMAT.md.
    /// </summary>
    private static LicenciaPayload ParseCanonicalPayload(
        byte[] payloadBytes)
    {
        string json;

        try
        {
            var utf8 = new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: false,
                throwOnInvalidBytes: true);

            json = utf8.GetString(payloadBytes);
        }
        catch (Exception ex)
        {
            throw new FormatException(
                "El payload no es UTF-8 válido.",
                ex);
        }

        using var document = JsonDocument.Parse(json);

        var root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new FormatException(
                "El payload debe ser un objeto JSON.");
        }

        // El contrato exige exactamente estas cuatro propiedades.
        var propertyNames = root
            .EnumerateObject()
            .Select(p => p.Name)
            .ToArray();

        if (propertyNames.Length != 4 ||
            !string.Equals(
                propertyNames[0],
                "Version",
                StringComparison.Ordinal) ||
            !string.Equals(
                propertyNames[1],
                "Product",
                StringComparison.Ordinal) ||
            !string.Equals(
                propertyNames[2],
                "InstallationId",
                StringComparison.Ordinal) ||
            !string.Equals(
                propertyNames[3],
                "LicenseId",
                StringComparison.Ordinal))
        {
            throw new FormatException(
                "El payload no contiene exactamente las cuatro propiedades " +
                "esperadas en el orden requerido.");
        }

        var versionElement = root.GetProperty("Version");
        var productElement = root.GetProperty("Product");
        var installationElement = root.GetProperty("InstallationId");
        var licenseElement = root.GetProperty("LicenseId");

        if (versionElement.ValueKind != JsonValueKind.Number ||
            !versionElement.TryGetInt32(out var version))
        {
            throw new FormatException(
                "Version no es un entero válido.");
        }

        if (productElement.ValueKind != JsonValueKind.String ||
            installationElement.ValueKind != JsonValueKind.String ||
            licenseElement.ValueKind != JsonValueKind.String)
        {
            throw new FormatException(
                "Product, InstallationId y LicenseId deben ser cadenas.");
        }

        var product = productElement.GetString() ?? string.Empty;
        var installationId =
            installationElement.GetString() ?? string.Empty;
        var licenseId =
            licenseElement.GetString() ?? string.Empty;

        var payload = new LicenciaPayload
        {
            Version = version,
            Product = product,
            InstallationId = installationId,
            LicenseId = licenseId
        };

        var canonicalBytes = payload.ToCanonicalBytes();

        if (!payloadBytes.AsSpan().SequenceEqual(canonicalBytes))
        {
            throw new FormatException(
                "El payload no está en la representación canónica esperada.");
        }

        return payload;
    }

    /// <summary>
    /// Carga la clave pública PEM embebida en el ensamblado de PesajeApp.
    /// </summary>
    private static RSA LoadPublicKey()
    {
        const string resourceName =
            "PesajeApp.Assets.public-key.pem";

        var assembly = typeof(LicenciaValidator).Assembly;

        using var stream =
            assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"No se encontró el recurso embebido '{resourceName}'.");
        }

        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true);

        var pem = reader.ReadToEnd();

        var rsa = RSA.Create();
        rsa.ImportFromPem(pem);

        return rsa;
    }

    /// <summary>
    /// Decodifica Base64URL sin padding.
    /// </summary>
    private static byte[] DecodeBase64Url(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new FormatException(
                "La parte Base64URL está vacía.");
        }

        // El contrato usa Base64URL:
        // '-' en lugar de '+'
        // '_' en lugar de '/'
        // sin '=' al final.
        if (value.Contains('=') ||
            value.Contains('+') ||
            value.Contains('/') ||
            value.Any(char.IsWhiteSpace))
        {
            throw new FormatException(
                "La cadena no tiene formato Base64URL válido.");
        }

        var base64 = value
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 0:
                break;

            case 2:
                base64 += "==";
                break;

            case 3:
                base64 += "=";
                break;

            default:
                throw new FormatException(
                    "Longitud Base64URL inválida.");
        }

        try
        {
            return Convert.FromBase64String(base64);
        }
        catch (FormatException ex)
        {
            throw new FormatException(
                "Base64URL inválido.",
                ex);
        }
    }
}