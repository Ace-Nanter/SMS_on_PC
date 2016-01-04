/*-----------------------------------------------------------------------*/
/* AVANT TOUT USAGE DU PROGRAMME UTILISER FILTER SUR LE TELEPHONE UTILISE*/
/*             INSTALLER LES FILTRES                                     */
/*-----------------------------------------------------------------------*/

using System;
using System.Text;

namespace TestConsole {

    public class Synchroniser {       
        static readonly bool DEBUG = true;                      // Lance le mode débug
 
        /// <summary>
        /// Fonction principale
        /// </summary>
        /// <param name="args">Rien à donner pour le moment</param>
        public static void Main(string[] args) {
            USBManager m = new USBManager();

            string s;

            if(m.connect()) {
                Console.WriteLine("Connexion OK !");
                Console.WriteLine("Que voulez-vous écrire ?");
                s = Console.ReadLine();
                m.write(s);
                m.read();

            }

            m.stop();
        }   // Fin Fonction main
    }       // Fin public class
}           // Fin namespace