using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsbLayer {
    public interface UsbInterface {
        /// <summary>
        /// Se déclenche lors d'une lecture sur le flux USB.
        /// </summary>
        /// <param name="msg">Message lu sur le flux USB</param>
        void hasRead(string msg);

        /// <summary>
        /// Se déclenche lors de la réception d'un SMS sur le flux USB.
        /// </summary>
        /// <param name="sms">SMS reçu</param>
        void smsReceived(SMS sms);

        /// <summary>
        /// Permet de savoir quand la connexion est faite
        /// </summary>
        void hasBeenConnected();

        /// <summary>
        /// Permet de savoir quand la connexion a été coupée
        /// </summary>
        void hasBeenStopped();
    }
}
