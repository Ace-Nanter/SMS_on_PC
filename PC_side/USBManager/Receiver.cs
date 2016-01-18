using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace USBLayer {
    class Receiver {

        private static readonly ReadEndpointID readEndpoint = ReadEndpointID.Ep04;  // 0x84 ==> 132 en décimal ==> Correspond à l'endpoint 4 dans l'enum (qui démarre à 129)
        private UsbEndpointReader m_reader = null;                                  // Endpoint de lecture

        private Thread readThread = null;
        private bool m_engaged = false;                                             // Définit l'arrêt/marche du thread de lecture
        private USBManager m_manager;                                               // Référence au manager global

        public Receiver(USBManager manager, UsbDevice device) {
            //m_manager = manager;
            //m_reader = device.OpenEndpointReader(readEndpoint);                     // On ouvre le canal de lecture
            //m_engaged = true;
            //readThread = new Thread(this.doRead);

            //// Start to read
            //readThread.Start();
        }

        //public void doRead() {
        //    ErrorCode ec = ErrorCode.None;
        //    UsbTransfer bytesRead;
        //    string code = "";

        //    while (m_engaged) {
        //        try {
        //            byte[] codeBuffer = new byte[2];                                // Buffer pour contenir la taille des buffers suivants
        //            // Lecture de la taille d'un paquet à venir
        //            ec = m_reader.SubmitAsyncTransfer(codeBuffer, 0, codeBuffer.Length, 100, out bytesRead);

        //            checkErrors(ec, bytesRead, codeBuffer.Length);                  // Vérification des erreurs
        //            code = Encoding.UTF8.GetString(codeBuffer);                     // Désencodage

        //            if (string.Equals(code, UsbCode.STOP) {                         // Demande de stop

        //            }
        //            else if (string.Equals(code, UsbCode.CONTACT) {                 // Arrivée d'un contact

        //            }

        //            sizeRead = int.Parse(Encoding.Default.GetString(codeBuffer));   // On convertit la chaîne en int
        //            bytesRead.Dispose();                                            // On se prépare pour une nouvelle lecture
        //            byte[] buffer = new byte[sizeRead];                             // Nouveau buffer
        //            ec = m_reader.SubmitAsyncTransfer(buffer, 0, sizeRead, 100, out bytesRead);

        //            checkErrors(ec, bytesRead, sizeRead);                           // On vérifie les erreurs

        //            bytesRead.Dispose();
        //            code = Encoding.UTF8.GetString(buffer);                          // On récupère le message
        //            readEvent(code);                                                 // On émet l'évènement

        //            #region Réinitialisation des variables

        //            sizeRead = -1;
        //            codeBuffer = null;
        //            buffer = null;
        //            code = "";


        //            //sizeRead = int.Parse(Encoding.Default.GetString(sizeBuffer));   // On convertit la chaîne en int
        //            //bytesRead.Dispose();                                            // On se prépare pour une nouvelle lecture
        //            //byte[] buffer = new byte[sizeRead];                             // Nouveau buffer
        //            //ec = m_reader.SubmitAsyncTransfer(buffer, 0, sizeRead, 100, out bytesRead);

        //            //checkErrors(ec, bytesRead, sizeRead);                           // On vérifie les erreurs

        //            //bytesRead.Dispose();
        //            //msg = Encoding.UTF8.GetString(buffer);                          // On récupère le message
        //            //readEvent(msg);                                                 // On émet l'évènement

        //            //#region Réinitialisation des variables

        //            //sizeRead = -1;
        //            //sizeBuffer = null;
        //            //buffer = null;
        //            //msg = "";

        //            #endregion
        //        }
        //        catch (Exception e) {
        //            this.stop();                                                    // On arrête la lecture
        //            // TODO : envoyer un message
        //        }
        //    }
        //}

        ///// <summary>
        ///// Fonction raccourci permettant de vérifier qu'il n'y ait pas d'erreurs
        ///// </summary>
        ///// <param name="ec">Code d'erreur utilisé</param>
        ///// <param name="bytesRead">Bits lus</param>
        ///// <param name="theoricLength">Longueur théorique du buffer</param>
        //public void checkErrors(ErrorCode ec, UsbTransfer bytesRead, int theoricLength) {
        //    if (bytesRead.Transmitted != theoricLength && ec != ErrorCode.None) {
        //        throw new Exception("Erreur de lecture !");
        //    }
        //}

        //public bool is_running() {
        //    return m_engaged;
        //}

        ///// <summary>
        ///// Fonction qui émet les évènements lors qu'un message a été reçu
        ///// </summary>
        ///// <param name="msg">Chaîne de caractères lues par le flux USB</param>
        //public void readEvent(string msg) {
        //    // TODO : fonction qui émet un evenement avec le message à l'intérieur
        //}

        ///// <summary>
        ///// Fonction qui arrête la lecture en envoyant un évènement contenant la raison de l'arrêt
        ///// </summary>
        ///// <param name="e">Exception qui peut avoir causer l'arrêt</param>
        //public void stop() {
        //    m_engaged = false;
        //}


    }
}
