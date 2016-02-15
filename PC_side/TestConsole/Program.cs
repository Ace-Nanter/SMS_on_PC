using EntityLayer;
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
                    /*string msg = "";
                    Console.WriteLine("Connexion OK");

                    while(true) {
                        Console.WriteLine("Entrez le message à envoyer :");
                        msg = Console.ReadLine();
                        if (!string.Equals("STOP", msg)) {
                            manager.send(msg);
                            Console.WriteLine("Envoi du message : {0}", msg);
                            Thread.Sleep(100);
                        }
                        else {
                            break;
                        }
                    }
                    manager.stop();*/

                    Console.WriteLine("Envoi du message ?");
                    Console.ReadLine();                                 // Petite pause

                    Contact c = new Contact("Adrien", "0647657049");
                    string msg = Console.ReadLine();
                    if(!string.IsNullOrEmpty(msg)) {
                        SMS sms = new SMS(msg, c);
                        sms.send(manager);
                    }
                }
                else {
                    Console.WriteLine("Aucun téléphone connecté !");
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
