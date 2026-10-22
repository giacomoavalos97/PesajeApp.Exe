using PesajeApp.Models;
using PesajeApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PesajeApp.Views
{
    /// <summary>
    /// Lógica de interacción para SedesView.xaml
    /// </summary>
    public partial class SedesView : Window
    {
        private readonly SedeService _sedeService;
        public SedesView()
        {
            InitializeComponent();

            _sedeService = new SedeService();

            Loaded += SedesView_Loaded;
        }

        private async void SedesView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarSedesAsync();
        }

        private async Task CargarSedesAsync()
        {
            try
            {
                var sedes = await _sedeService.ObtenerTodosAsync();

                SedesDataGrid.ItemsSource = sedes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar las sedes:\n\n{ex.Message}",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new SedeForm
            {
                Owner = this
            };

            var resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                await CargarSedesAsync();
            }
        }

        private async void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (SedesDataGrid.SelectedItem is not Sede sedeSeleccionada)
            {
                MessageBox.Show(
                    "Debe seleccionar una sede.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var ventana = new SedeForm(sedeSeleccionada)
            {
                Owner = this
            };

            var resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                await CargarSedesAsync();
            }
        }

        private async void Desactivar_Click(object sender, RoutedEventArgs e)
        {
            if (SedesDataGrid.SelectedItem is not Sede sedeSeleccionada)
            {
                MessageBox.Show(
                    "Debe seleccionar una sede.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var resultado = MessageBox.Show(
                $"¿Está seguro de desactivar la sede \"{sedeSeleccionada.Nombre}\"?",
                "Confirmar desactivación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                await _sedeService.DesactivarAsync(sedeSeleccionada.Id);

                MessageBox.Show(
                    "Sede desactivada correctamente.",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                await CargarSedesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al desactivar la sede:\n\n{ex.Message}",
                    "PesajeApp",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
