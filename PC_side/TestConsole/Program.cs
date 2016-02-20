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
                    while(true) {
                        Console.WriteLine("Ecrivez votre message :");
                        string msg = Console.ReadLine();

                        Contact c = new Contact("Adrien", "0647657049");
                        if (!string.IsNullOrEmpty(msg)) {
                            SMS sms = new SMS(msg, c);
                            sms.send(manager);
                        }
                        Console.WriteLine("Message envoyé !");
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
    }

    
}
