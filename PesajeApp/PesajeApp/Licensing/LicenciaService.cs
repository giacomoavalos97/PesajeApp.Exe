namespace PesajeApp.Licensing;

/// <summary>
/// Servicio principal de licenciamiento de PesajeApp.
/// Coordina la lectura, validación y activación de la licencia.
/// </summary>
public sealed class LicenciaService
{
    private readonly LicenciaStorage _storage;
    private readonly InstallationIdService _installationIdService;
    private readonly LicenciaValidator _validator;

    public LicenciaService()
    {
        _storage = new LicenciaStorage();
        _installationIdService = new InstallationIdService();
        _validator = new LicenciaValidator();
    }

    /// <summary>
    /// Obtiene el InstallationId de esta PC.
    /// </summary>
    public string ObtenerInstallationId()
    {
        return _installationIdService.ObtenerInstallationId();
    }

    /// <summary>
    /// Indica si existe una licencia almacenada.
    /// </summary>
    public bool ExisteLicencia()
    {
        return _storage.ExisteLicencia();
    }

    /// <summary>
    /// Valida la licencia almacenada para esta instalación.
    /// </summary>
    public LicenciaValidationResult ValidarLicenciaInstalada()
    {
        var licencia = _storage.Obtener();

        if (string.IsNullOrWhiteSpace(licencia))
        {
            return LicenciaValidationResult.Invalid(
                "No existe una licencia instalada.");
        }

        var installationId =
            _installationIdService.ObtenerInstallationId();

        return _validator.Validate(
            licencia,
            installationId);
    }

    /// <summary>
    /// Intenta activar PesajeApp utilizando una licencia.
    /// </summary>
    public LicenciaValidationResult Activar(string licencia)
    {
        if (string.IsNullOrWhiteSpace(licencia))
        {
            return LicenciaValidationResult.Invalid(
                "La licencia no puede estar vacía.");
        }

        var installationId =
            _installationIdService.ObtenerInstallationId();

        var resultado = _validator.Validate(
            licencia,
            installationId);

        if (!resultado.EsValida)
        {
            return resultado;
        }

        _storage.Guardar(licencia);

        return resultado;
    }
}