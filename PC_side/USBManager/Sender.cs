﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace USBLayer {

    class Sender {

        // Constantes
        private readonly WriteEndpointID writeEndpoint = WriteEndpointID.Ep05;      // 0x05 ==> 5 en décimal ==> Endpoint 5
        private readonly int timeout = 100;                                         // timeout : 100ms

        private UsbEndpointWriter m_writer = null;                                  // Endpoint d'écriture
        private Stack<string> m_toSend;                                             // Pile de messages à envoyer
        private USBManager m_manager = null;                                        // Référence au manager
        private Thread m_writeThread = null;                                        // Thread d'envoi
        private bool m_stop = false;                                                // Variable d'arrêt du thread d'envoi

        public Sender(USBManager manager) {
            if (manager != null) {
                m_manager = manager;
                m_writer = m_manager.getDevice().OpenEndpointWriter(writeEndpoint);
                m_toSend = new Stack<string>();                                     // Initialisation de la pile
                m_writeThread = new Thread(this.doSend);                            // Création du thread
                m_writeThread.Start();                                              // Lancement du thread
            }
        }

        public void send(string msg) {
            m_toSend.Push(msg);
        }

        /// <summary>
        /// Processus principal du thread d'envoi : lit la pile et envoie le sommet de la pile
        /// Si la pile est vide ne fait rien
        /// </summary>
        public void doSend() {
            ErrorCode ec = ErrorCode.None;
            UsbTransfer bytesWritten;
            string msg = null;

            string length = "";
            byte[] sizeBuffer;
            byte[] buffer;

            while (!m_stop) {
                if(m_toSend.Count > 0) { 
                    msg = m_toSend.Pop();                                               // Récupération de la chaîne à envoyer
                    if (!string.IsNullOrEmpty(msg)) {
                        try {
                            #region Envoi de la longueur

                            length += Encoding.UTF8.GetByteCount(msg);
                            sizeBuffer = new byte[5];                                       // Réinitialisation du buffer
                            sizeBuffer = Encoding.UTF8.GetBytes(length);
                            ec = m_writer.SubmitAsyncTransfer(sizeBuffer, 0, sizeBuffer.Length, timeout, out bytesWritten);
                            checkErrors(ec, bytesWritten, sizeBuffer.Length);               // Vérification des erreurs
                            bytesWritten.Dispose();

                            #endregion

                            #region Encodage du message

                            buffer = new byte[int.Parse(length)];                           // Création d'un buffer de taille appropriée
                            buffer = Encoding.UTF8.GetBytes(msg);
                            ec = m_writer.SubmitAsyncTransfer(buffer, 0, buffer.Length, timeout, out bytesWritten);
                            checkErrors(ec, bytesWritten, sizeBuffer.Length);
                            bytesWritten.Dispose();

                            #endregion
                        }
                        catch (Exception e) {
                            m_stop = true;
                            // TODO : traitement pour récupérer l'erreur ?
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Fonction arrêtant le thread d'envoi
        /// </summary>
        /// <returns></returns>
        public bool stop() {
            m_stop = true;                                  // Arrêt du thread
            m_writer.Dispose();                             // On libère les ressources d'écriture
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
