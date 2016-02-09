using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UsbLayer;

namespace TestConsole {
    class Program {
        static void Main(string[] args) {
            UsbManager manager = new UsbManager();

            Console.Write("Veuillez connecter votre appareil s'il vous plaît...");
            Console.ReadLine();
            try {
                if (manager.connect()) {
                    string msg = "";
                    Console.WriteLine("Connexion OK");

                    while(!string.Equals("STOP", msg)) {
                        Console.WriteLine("Entrez le message à envoyer :");
                        msg = Console.ReadLine();
                        manager.send(msg);
                        Console.WriteLine("Envoi du message : {0}", msg);
                        Thread.Sleep(100);
                    }
                    manager.stop();
                }
                else {
                    throw new Exception("Erreur lors de la connexion !");
                }
            }
            catch (Exception e) {
                Console.WriteLine("Exception occured : {0}", e);
            }
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
