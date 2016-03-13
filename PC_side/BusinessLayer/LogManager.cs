using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer {
    public class LogManager {
        public static void WriteToFile(string errorMessage, string className) {

            string filename = "error.log";

            // Vérifier le fichier
            if (!System.IO.File.Exists(filename)) {
                // Création du fichier log du jour
                System.IO.FileStream f = System.IO.File.Create(filename);
                f.Close();
            }

            using (StreamWriter writer = new StreamWriter(filename, true)) {
                //écriture dans le fichier log du jour
                writer.WriteLine(string.Format(
                                       "[{0} - {1}] : {2}",
                                       className,
                                       DateTime.Now,
                                       errorMessage));
            }
        }
    }
}
