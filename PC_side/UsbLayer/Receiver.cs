using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace UsbLayer {
    class Receiver {

        private static readonly ReadEndpointID readEndpoint = ReadEndpointID.Ep04;  // 0x84 ==> 132 en décimal ==> Correspond à l'endpoint 4 dans l'enum (qui démarre à 129)
        private readonly int timeout = int.Parse(Properties.Resources.timeout);     // timeout : 100ms
        private UsbEndpointReader m_reader = null;                                  // Endpoint de lecture

        private Thread m_readThread = null;
        private EndpointDataEventArgs m_args = null;                                // Permet d'avoir accès aux arguments globalement
        private bool m_stop = false;                                                // Définit l'arrêt/marche de la lecture
        private UsbManager m_manager;                                               // Référence au manager global

        public Receiver(UsbManager manager) {
            m_manager = manager;
            m_reader = manager.getDevice().OpenEndpointReader(readEndpoint);        // On ouvre le canal de lecture
            m_reader.Flush();

            m_reader.DataReceived += (createThread);                                // Activation de l'event de lecture
            m_reader.DataReceivedEnabled = true;
        }

        public bool active() {
            return (!m_stop);
        }

        private void createThread(object sender, EndpointDataEventArgs e) {

            // Get the arguments
            m_args = e;
            
            // Start the thread
            m_readThread = new Thread(this.doRead);
            m_readThread.Start();
        }

        private void doRead() {
            ErrorCode ec = ErrorCode.None;
            
            int length = 0;
            int received = 0;
            byte[] buffer;
            string msg = "";
            string tmp = "";
            
            try {

                // Desactivate the event reading
                m_reader.DataReceived -= (createThread);
                m_reader.DataReceivedEnabled = false;

                #region Get the length
                tmp = Encoding.UTF8.GetString(m_args.Buffer, 0, m_args.Count);
                if (string.IsNullOrEmpty(tmp))                                      // Is it exploitable ?
                    throw new Exception("Anormal data received !");

                length = int.Parse(tmp);                                            // Get int from string
                #endregion

                #region Récupération du message

                buffer = new byte[length];                                          // Instanciate a buffer at the good size        
                ec = m_reader.Read(buffer, 0, buffer.Length, timeout, out received);
                checkErrors(ec, received, buffer.Length);
                msg = Encoding.UTF8.GetString(buffer);
                if (!string.IsNullOrEmpty(msg)) {                                   // Is the message exploitable ?
                                                                            // TODO : event, interface ?
                    Console.WriteLine(">>> {0}", msg);                   // TODO : to delete
                }
                #endregion
            }
            catch (Exception e) {
                Console.WriteLine("An exception occurred : " + e);
                stop();
                // TODO : traitement pour récupérer l'erreur ?
            }

            // Reactivate the event
            m_reader.DataReceived += (createThread);
            m_reader.DataReceivedEnabled = true;
        }

        /// <summary>
        /// Fonction raccourci permettant de vérifier qu'il n'y ait pas d'erreurs
        /// </summary>
        /// <param name="ec">Code d'erreur utilisé</param>
        /// <param name="bytesRead">Bits lus</param>
        /// <param name="theoricLength">Longueur théorique du buffer</param>
        public void checkErrors(ErrorCode ec, int received, int theoricLength) {
            if (received != theoricLength && ec != ErrorCode.None) {
                throw new Exception("Erreur de lecture !");
            }
        }
        
        /// <summary>
        /// Fonction qui émet les évènements lors qu'un message a été reçu
        /// </summary>
        /// <param name="msg">Chaîne de caractères lues par le flux USB</param>
        public void readEvent(string msg) {
            // TODO : fonction qui émet un evenement avec le message à l'intérieur
        }

        /// <summary>
        /// Fonction qui arrête la lecture en envoyant un évènement contenant la raison de l'arrêt
        /// </summary>
        /// <param name="e">Exception qui peut avoir causer l'arrêt</param>
        public void stop() {

            Console.WriteLine("Désactivation du receiver...");
            // Desactivate the event reading
            m_reader.DataReceived -= (createThread);
            m_reader.DataReceivedEnabled = false;
            m_stop = true;                                  // Arrêt du thread
            //m_readThread.Abort();
            m_reader.Abort();                               // Stop reading
            m_reader.Dispose();                             // On libère les ressources de lecture
        }
    }
}
