using System;
using System.Collections.Generic;
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
            string toHandle;

            while (!m_stop) {
                toHandle = m_manager.getLastReceived();
                if (!string.IsNullOrEmpty(toHandle)) {
                    if(toHandle.StartsWith("ACK")) {
                        m_manager.popSending();
                    }
                    else {
                        m_manager.send("ACK");                              // Acquittal
                        if (toHandle.StartsWith("OK")) {
                            m_manager.hasBeenConnected();
                        }
                        
                    }
                    /*
                    if (toHandle.startsWith("SMSHEADER")) {                 // SMS Header
                        String args[] = toHandle.split(":");
                        if (checkHeader(args)) {
                            SMS sms = new SMS(Integer.parseInt(args[1]), args[2]);
                            for (int i = 0; i < Integer.parseInt(args[3]); i++) {
                                toHandle = m_link.getLastReceived();

                                // Wait for the stream
                                while (toHandle == null) {
                                    toHandle = m_link.getLastReceived();
                                    try {
                                        Thread.sleep(500);
                                    }
                                    catch (InterruptedException e) {
                                        Log.d(ReceiveHandler.class.getSimpleName(),
                                                    "Error during a sleep !");
                                        }
}

                                    // Concatenation
                                    if(toHandle.startsWith("SMSBODY")) {
                                        toHandle = toHandle.substring(8);
                                        sms.appendBody(toHandle);
                                    }
                                }
                                sms.send();                             // Send the message
                            }
                        }
                    else if(toHandle.startsWith("SMSBODY")) {           // SMS Body
                        Log.d(ReceiveHandler.class.getSimpleName(),
                            "Error : SMS body without header !");
                    }
                    else if(toHandle.startsWith("CONTACT")) {
                        // TODO : to implement
                    }
                    else if(toHandle.startsWith("ACK")) {               // Acquittal received
                        m_link.popSentList();
                    }*/
                }
                Thread.Sleep(50);                                       // For scheduling
            }   // End while
        }       // End method

        /*        
        private bool checkHeader(String args[]) {
        boolean flag = true;

        flag &= args[0].equals("SMSHEADER");

        try {
            flag &= (Integer.parseInt(args[1]) > 0);
            flag &= (Integer.parseInt(args[3]) > 0);
        }
        catch (Exception e) {
            flag = false;
        }

        flag &= (args[2].length() >= 10);

        return flag;
    }*/

        public void stop() {
            m_stop = true;
            if (m_handleThread.IsAlive) {
                m_handleThread.Abort();
            }
        }
    }
}
