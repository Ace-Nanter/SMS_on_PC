using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using BusinessLayer;

namespace UsbLayer {

    class Sender {

        // Constantes
        private readonly WriteEndpointID writeEndpoint = WriteEndpointID.Ep02;      // 0x02 ==> 2 en décimal ==> Endpoint 2
        //private readonly WriteEndpointID writeEndpoint = WriteEndpointID.Ep05;      // 0x05 ==> 5 en décimal ==> Endpoint 5
        private readonly int timeout = int.Parse(Properties.Resources.timeout);     // timeout : 100ms

        private UsbEndpointWriter m_writer = null;                                  // Endpoint d'écriture
        private Queue<string> m_toSend;                                             // FIFO des messages à envoyer
        private UsbManager m_manager = null;                                        // Référence au manager
        private Thread m_writeThread = null;                                        // Thread d'envoi
        private bool m_stop = false;                                                // Variable d'arrêt du thread d'envoi

        public Sender(UsbManager manager) {
            if (manager != null) {
                m_manager = manager;
                m_writer = m_manager.getDevice().OpenEndpointWriter(writeEndpoint);
                m_writer.Flush();
                m_toSend = new Queue<string>();                                     // Initialisation de la pile

                m_writeThread = new Thread(this.doSend);                            // Création du thread
                m_writeThread.Start();                                              // Lancement du thread
            }
        }

        public void send(string msg) {
            m_toSend.Enqueue(msg);
        }

        public bool active() {
            return (!m_stop);
        }

        /// <summary>
        /// Processus principal du thread d'envoi : lit la pile et envoie le sommet de la pile
        /// Si la pile est vide ne fait rien
        /// </summary>
        public void doSend() {
            ErrorCode ec = ErrorCode.None;
            UsbTransfer bytesWritten;
            
            int tryLeft = 5;
            string msg = null;
            string length = "";
            byte[] sizeBuffer;
            byte[] buffer;

            while (!m_stop) {
                if (m_toSend.Count > 0) {                                                   // Si la file n'est pas vide
                    msg = m_toSend.Peek();                                                  // Récupération de la chaîne à envoyer
                    if (!string.IsNullOrEmpty(msg)) {                                       // Si cette chaîne est viable
                        tryLeft = 5;

                        // Tant qu'on a pas réussi à envoyer
                        while (m_toSend.Count > 0 && string.Equals(msg, m_toSend.Peek()) && tryLeft > 0) {
                            try {
                                #region Envoi de la longueur

                                length = "" + Encoding.UTF8.GetByteCount(msg);
                                sizeBuffer = Encoding.UTF8.GetBytes(length);
                                ec = m_writer.SubmitAsyncTransfer(sizeBuffer, 0, sizeBuffer.Length, timeout, out bytesWritten);
                                checkErrors(ec, bytesWritten, sizeBuffer.Length);           // Vérification des erreurs
                                bytesWritten.Dispose();

                                #endregion
                                #region Encodage du message

                                buffer = new byte[int.Parse(length)];                       // Création d'un buffer de taille appropriée
                                buffer = Encoding.UTF8.GetBytes(msg);
                                ec = m_writer.SubmitAsyncTransfer(buffer, 0, buffer.Length, timeout, out bytesWritten);
                                checkErrors(ec, bytesWritten, buffer.Length);
                                bytesWritten.Dispose();

                                #endregion
                            }
                            catch (Exception e) {
                                LogManager.WriteToFile(e.Message, "Sender");
                            }

                            // Si on a un ACK à envoyer, on déqueue
                            if(string.Equals(msg, "ACK")) {
                                pop();
                                break;
                            }

                            Thread.Sleep(600);                                              // Timeout de 600 ms
                            tryLeft--;
                        }

                        if(m_toSend.Count > 0 && string.Equals(msg, m_toSend.Peek())) {
                            LogManager.WriteToFile("Fail to Send : " + msg, "Sender");
                            pop();
                        }
                    }
                }
                Thread.Sleep(100);                                                          // Envoi toutes les 100 ms
            }
        }

        /// <summary>
        /// Supprime l'élément de la pile du dessus
        /// </summary>
        public void pop() {
            if(m_toSend.Count > 0)
                m_toSend.Dequeue();
        }

        /// <summary>
        /// Fonction arrêtant le thread d'envoi
        /// </summary>
        /// <returns></returns>
        public bool stop() {
            m_stop = true;                                  // Arrêt du thread
            m_writer.Dispose();
            return (m_toSend.Count > 0);
        }

        /// <summary>
        /// Fonction raccourci permettant de vérifier qu'il n'y ait pas d'erreurs
        /// </summary>
        /// <param name="ec">Code d'erreur utilisé</param>
        /// <param name="bytes">Bits lus</param>
        /// <param name="theoricLength">Longueur théorique du buffer</param>
        public void checkErrors(ErrorCode ec, UsbTransfer bytes, int theoricLength) {
            if (bytes.Transmitted != theoricLength && ec != ErrorCode.None) {
                throw new Exception("Erreur de lecture !");
            }
        }
    }
}
