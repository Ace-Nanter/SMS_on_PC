using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer;
using UsbLayer;

namespace TestConsole {
    public class Utilities : UsbInterface {

        /// <summary>
        /// Occure when USB connexion is up
        /// </summary>
        public void hasBeenConnected() {
            Console.WriteLine("Connecté !");
        }

        /// <summary>
        /// Occure when USB connexion has been stopped
        /// </summary>
        public void hasBeenStopped() {
            Console.WriteLine("Connexion USB arrêtée.");
        }

        /// <summary>
        /// Display an incoming message from USB
        /// </summary>
        /// <param name="msg"></param>
        public void hasRead(string msg) {
            Console.WriteLine("Message reçu : " + msg);
        }

        public void smsReceived(SMS sms) {
            Console.WriteLine("Nouveau message reçu : " + sms.Body);
        }
    }
}
