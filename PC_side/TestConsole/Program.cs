using EntityLayer;
using System;

using UsbLayer;

namespace TestConsole {
    class Program {
        static void Main(string[] args) {
            UsbManager manager = UsbManager.getInstance(new Utilities());
            
            try {
                if (manager.connect()) {
                    while(true) {
                        Console.WriteLine("Ecrivez votre message :");
                        string msg = Console.ReadLine();

                        Contact c = new Contact("Adrien", "0647657049");
                        if (!string.IsNullOrEmpty(msg)) {
                            SMS sms = new SMS(msg, c);
                            manager.send(sms);
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
