using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Projet
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application {
        /// <summary>
        /// Démarrage de l'application.
        /// </summary>
        /// <param name="e">Arguments du programme.</param>
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            // Ajoute le gestionnaire d'exceptions
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            try {
                MessageBox.Show("Une erreur est survenue, mais aucune crainte, elle a été gérée !",
                                Assembly.GetExecutingAssembly().GetName().Name,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                e.Handled = true;   // Sinon crash l'application
            }
            catch (Exception) {
                // Nothing
            }
        }
    }
}
