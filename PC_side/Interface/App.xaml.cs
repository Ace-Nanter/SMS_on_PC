using BusinessLayer;
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
                LogManager.WriteToFile(e.Exception.Message, "App");
            }
            catch (Exception) {
                // Nothing
            }








        }
    }
}
