using BusinessLayer;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UsbLayer {
    class ReceiveHandler {

        private bool m_stop;
        private Thread m_handleThread = null;
        private UsbManager m_manager;

        public ReceiveHandler(UsbManager manager) {
            m_stop = false;
            m_manager = manager;
            m_handleThread = new Thread(this.handle);
            m_handleThread.Start();
        }

        /// <summary>
        /// Manage what was received
        /// </summary>
        private void handle() {
            int index = 0;
            int nbComs = 0;
            SMS sms = null;
            string toHandle;

            ConversationManager business = new ConversationManager();

            while (!m_stop) {
                try {
                    toHandle = m_manager.getLastReceived();
                    if (!string.IsNullOrEmpty(toHandle)) {
                        if (toHandle.StartsWith("ACK")) {
                            m_manager.popSending();
                        }
                        else {
                            m_manager.send("ACK");                          // Acquittal
                            #region Connexion
                            if (toHandle.StartsWith("OK")) {
                                m_manager.hasBeenConnected();
                            }
                            #endregion
                            #region SMS HEADER
                            if (toHandle.StartsWith("SMSHEADER")) {         // SMS Header
                                string[] args = toHandle.Split(':');        // Get arguments

                                if (checkHeader(args)) {                    // Check
                                    // Get contact
                                    Contact c = business.getContactFromString(args[1]);         // Get contact

                                    // Get a phone number which begin by 0
                                    if (c != null) {
                                        if (args[1].StartsWith("+33")) {
                                            args[1] = "0" + args[1].Substring(3);
                                        }
                                        c = new Contact(args[1]);
                                    }

                                    // Get the date
                                    string pattern = "dd-MM-yyyy HH.mm.ss";
                                    DateTime date = DateTime.ParseExact(args[2], pattern,
                                        CultureInfo.InvariantCulture);

                                    sms = new SMS(c, date, true);
                                    nbComs = int.Parse(args[3]);
                                    index = 1;
                                }
                                else {                                      // If problem
                                    throw new Exception("Incorrect Header !");
                                }
                            }
                            #endregion
                            #region SMS BODY
                            else if (toHandle.StartsWith("SMSBODY")) {      // SMS Body
                                if (index != 0 && nbComs != 0) {

                                    toHandle = toHandle.Substring(8);       // Delete "SMSBODY:"

                                    // Get the number of the part of the message
                                    int i = int.Parse(toHandle.Split(':')[0]);
                                    toHandle = toHandle.Substring(2);       // Delete number

                                    // Get a part
                                    if (i == index && index <= nbComs) {
                                        sms.appendBody(toHandle);
                                    }
                                    else if (nbComs > 0 && i != index) {
                                        throw new Exception("Incorrect part of the message received !");
                                    }

                                    // Is the message complete ?
                                    if (index == nbComs) {                   // Yes
                                        // Add the message in the conversation
                                        Conversation c = business.getConversationsFromContact(sms.Contact.Num);
                                        business.AddMessageToConv(sms, c);

                                        m_manager.smsReceived(sms);         // Notify the interface
                                        nbComs = 0;
                                        index = 0;
                                    }
                                    else {                                  // No
                                        index++;
                                    }
                                }
                                else {
                                    throw new Exception("Body unexpected !");
                                }
                            }
                            #endregion
                            #region CONTACT
                            else if (toHandle.StartsWith("CONTACT")) {
                                // TODO : to implement
                            }
                            #endregion
                        }

                    }   // End if
                }
                catch (Exception e) {
                    LogManager.WriteToFile(e.Message, "ReceiveHandler");
                }

                Thread.Sleep(50);                                       // For scheduling
            }   // End while
        }       // End method

        private bool checkHeader(string[] args) {
            bool flag = true;

            flag &= string.Equals(args[0], "SMSHEADER");

            try {
                flag &= args[2].Length == 19;
                flag &= (int.Parse(args[3]) > 0);                       // Verify Nb coms
            }
            catch (Exception e) {
                flag = false;
            }

            flag &= (args[1].Length >= 10);                             // Phone number size is ok ?

            return flag;
        }

        public void stop() {
            m_stop = true;
            if (m_handleThread.IsAlive) {
                m_handleThread.Abort();
            }
        }
    }
}