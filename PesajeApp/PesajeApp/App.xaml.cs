using Microsoft.Extensions.Configuration;
using PesajeApp.Data;
using PesajeApp.Licensing;
using PesajeApp.Models;
using PesajeApp.Services;
using PesajeApp.Views;
using System.Windows;

namespace PesajeApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; } = null!;
        public static DatabaseConnection Database { get; private set; } = null!;
        public static BalanzaManager Balanza { get; private set; } = null!;

        // Empresa actualmente configurada
        public static Empresa? EmpresaActual { get; set; }

        public App()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Database = new DatabaseConnection(Configuration);
            Balanza = new BalanzaManager();
        }


        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // ==========================================
                // 1. VERIFICAR LICENCIA
                // ==========================================

                var licenciaService = new LicenciaService();

                var resultadoLicencia =
                    licenciaService.ValidarLicenciaInstalada();

                if (!resultadoLicencia.EsValida)
                {
                    var licenciaView = new LicenciaView();

                    var resultadoActivacion =
                        licenciaView.ShowDialog();

                    if (resultadoActivacion != true)
                    {
                        Shutdown();
                        return;
                    }

                    // Volvemos a comprobar la licencia después
                    // de la activación.
                    resultadoLicencia =
                        licenciaService.ValidarLicenciaInstalada();

                    if (!resultadoLicencia.EsValida)
                    {
                        MessageBox.Show(
                            "La licencia no pudo ser validada después de la activación.\n\n" +
                            resultadoLicencia.Motivo,
                            "PesajeApp",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        Shutdown();
                        return;
                    }
                }

                // ==========================================
                // 2. CARGAR CONFIGURACIÓN DE EMPRESA
                // ==========================================

                // Cargar configuración de empresa
                var empresaService = new EmpresaService();
                EmpresaActual = await empresaService.ObtenerAsync();

                // Iniciar balanza
                await Balanza.IniciarDesdeConfiguracionAsync();

                var login = new LoginView();
                MainWindow = login;
                login.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al iniciar PesajeApp:\n\n{ex.Message}",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
            }
        }



    }
}