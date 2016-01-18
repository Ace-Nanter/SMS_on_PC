using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using USBLayer;

namespace TestConsole {
    class Program {
        static void Main(string[] args) {
            /*USBManager manager = new USBManager();

            Console.Write("Veuillez connecter votre appareil s'il vous plaît...");
            Console.ReadLine();

            if(manager.connect()) {
                while (!manager.read()) ;
                if(!string.Equals(manager.last_msg(), "OK")) {
                    throw new Exception("Erreur : aucun message reçu de l'appareil !");
                }
                Console.WriteLine("Connecté");
                while (true) {
                    manager.read();
                    Console.WriteLine(manager.last_msg());
                }
            }
            else {
                throw new Exception("Erreur lors de la connexion !");
            }
            */
            /*
            int test = 35;
            string fmt = "00000";

            Console.WriteLine(test.ToString(fmt));
            */

            Stack<string> test = new Stack<string>();

            test.Push("yolo");
            test.Push("yolo2");

            Console.WriteLine(test.Pop() + " - taille : " + test.Count);
            Console.WriteLine(test.Pop() + " - taille : " + test.Count);
            Console.WriteLine(test.Count);


        }
        /*
        void USBInterface.hasBeenConnected() {
            throw new NotImplementedException();
        }

        void USBInterface.hasBeenStopped() {
            throw new NotImplementedException();
        }

        void USBInterface.hasRead(string msg) {
            throw new NotImplementedException();
        }*/
    }

    
}
