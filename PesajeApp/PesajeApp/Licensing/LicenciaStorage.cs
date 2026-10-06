using System.IO;
using System.Text;

namespace PesajeApp.Licensing;

/// <summary>
/// Guarda y recupera la licencia de PesajeApp
/// en el perfil local del usuario de Windows.
/// </summary>
public sealed class LicenciaStorage
{
    private const string AppFolderName = "PesajeApp";
    private const string LicenseFileName = "license.lic";

    private readonly string _licenseDirectory;
    private readonly string _licenseFilePath;

    public LicenciaStorage()
    {
        var localAppData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        _licenseDirectory = Path.Combine(
            localAppData,
            AppFolderName);

        _licenseFilePath = Path.Combine(
            _licenseDirectory,
            LicenseFileName);
    }

    /// <summary>
    /// Ruta donde se almacena la licencia.
    /// </summary>
    public string LicenseFilePath => _licenseFilePath;

    /// <summary>
    /// Indica si existe una licencia almacenada.
    /// </summary>
    public bool ExisteLicencia()
    {
        return File.Exists(_licenseFilePath);
    }

    /// <summary>
    /// Guarda una licencia en la PC.
    /// </summary>
    public void Guardar(string licencia)
    {
        if (string.IsNullOrWhiteSpace(licencia))
            throw new ArgumentException(
                "La licencia no puede estar vacía.",
                nameof(licencia));

        Directory.CreateDirectory(_licenseDirectory);

        File.WriteAllText(
            _licenseFilePath,
            licencia.Trim(),
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    /// <summary>
    /// Recupera la licencia almacenada.
    /// </summary>
    public string? Obtener()
    {
        if (!File.Exists(_licenseFilePath))
            return null;

        return File.ReadAllText(
            _licenseFilePath,
            Encoding.UTF8);
    }

    /// <summary>
    /// Elimina la licencia almacenada.
    /// </summary>
    public void Eliminar()
    {
        if (File.Exists(_licenseFilePath))
            File.Delete(_licenseFilePath);
    }
}