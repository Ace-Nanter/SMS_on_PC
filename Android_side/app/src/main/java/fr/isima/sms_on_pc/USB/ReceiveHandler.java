package fr.isima.sms_on_pc.USB;

import android.util.Log;


import fr.isima.sms_on_pc.SMS.SMS;

/**
 * Created by Ace Nanter on 14/02/2016.
 */
public class ReceiveHandler {

    private boolean m_stop;
    private Thread m_handle;
    private LinkManager m_link;

    public ReceiveHandler(LinkManager link) {

        m_stop = false;
        m_link = link;
        m_handle = new Thread(new Runnable() {
            @Override
            public void run() {
                int index = 0;
                int nbComs = 0;
                SMS sms = null;
                String toHandle;

                while(!m_stop) {
                    toHandle = m_link.getLastReceived();
                    if(toHandle != null && !toHandle.isEmpty()) {
                        if(toHandle.startsWith("ACK")) {
                            m_link.popSentList();
                        }
                        else {                                                  // Not a ACK
                            m_link.send("ACK");                                 // Acquittal
                            try {
                                if (toHandle.startsWith("SMSHEADER")) {         // SMS Header
                                    String args[] = toHandle.split(":");        // Get args

                                    if (checkHeader(args)) {                    // Check
                                        sms = new SMS(Integer.parseInt(args[1]), args[2]);
                                        nbComs = Integer.parseInt(args[3]);
                                        index = 1;
                                    }
                                    else {                                      // If problem
                                        throw new Exception("Incorrect Header !");
                                    }
                                } else if (toHandle.startsWith("SMSBODY")) {    // SMS Body
                                    if(index != 0 && nbComs != 0) {

                                        toHandle = toHandle.substring(8);       // Delete "SMSBODY:"

                                        // Get the number of the part of the message
                                        int i = Integer.parseInt(toHandle.split(":")[0]);
                                        toHandle = toHandle.substring(2);       // Delete number

                                        // Get a part
                                        if (i == index && index <= nbComs) {
                                            sms.appendBody(toHandle);
                                        } else if (nbComs > 0 && i != index) {
                                            throw new Exception("Incorrect part of the message received !");
                                        }

                                        // Is the message complete ?
                                        if(index == nbComs) {                   // Yes
                                            sms.send();
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
                                } else if (toHandle.startsWith("CONTACT")) {
                                    // TODO : to implement
                                }
                                // If no Exception, send the ACK

                            }
                            catch (Exception e) {
                                e.printStackTrace();
                                Log.d(ReceiveHandler.class.getSimpleName(), "An exception occured :" + e);
                            }
                        }

                    }
                    try {
                        Thread.sleep(100);                                  // For scheduling
                    }
                    catch(InterruptedException e) {
                        Log.d(ReceiveHandler.class.getSimpleName(), "Error during a sleep !");
                    }
                }       // End while
            }           // End run method
        });             // End Thread
        m_handle.start();                                                   // Start the Thread
    }

    private boolean checkHeader(String args[]) {
        boolean flag = true;

        flag &= args[0].equals("SMSHEADER");

        try {
            flag &= (Integer.parseInt(args[1]) > 0);
            flag &= (Integer.parseInt(args[3]) > 0);
        }
        catch(Exception e) {
            flag = false;
        }

        flag &= (args[2].length() >= 10);

        return flag;
    }

    public void stop() {
        m_stop = true;
        if (m_handle.isAlive()) {
            m_handle.interrupt();
        }
    }
}
