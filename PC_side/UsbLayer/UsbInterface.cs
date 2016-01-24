using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsbLayer {
    public interface UsbInterface {
        /// <summary>
        /// Se déclenche lors d'une lecture sur le flux USB
        /// </summary>
        /// <param name="msg">Message lu sur le flux USB</param>
        void hasRead(string msg);

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
