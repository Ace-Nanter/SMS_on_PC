package fr.isima.sms_on_pc.USB;

import android.util.Log;

import java.util.IllegalFormatException;

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
                int toComplete = 0;
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
                            try {
                                if (toHandle.startsWith("SMSHEADER")) {         // SMS Header
                                    String args[] = toHandle.split(":");        // Get args
                                    if (checkHeader(args)) {                    // Check
                                        sms = new SMS(Integer.parseInt(args[1]), args[2]);
                                        nbComs = Integer.parseInt(args[3]);
                                        toComplete = 1;
                                    }
                                    else {                                      // If problem
                                        throw new Exception("Incorrect Header !");
                                    }
                                } else if (toHandle.startsWith("SMSBODY")) {    // SMS Body
                                    // Concatenation
                                    toHandle = toHandle.substring(8);
                                    int i = Integer.parseInt(toHandle.split(":")[0]);
                                    toHandle = toHandle.substring(2);

                                    // Get a part
                                    if (i == toComplete && nbComs > 0) {
                                        sms.appendBody(toHandle);
                                        nbComs--;   toComplete++;
                                    }
                                    else if(nbComs > 0 && i != toComplete) {
                                        throw new Exception("Body unexpected !");
                                    }

                                    // If it's over, send the message
                                    if(toComplete == 0) {
                                        sms.send();
                                    }
                                } else if (toHandle.startsWith("CONTACT")) {
                                    // TODO : to implement
                                }
                                // If no Exception, send the ACK
                                m_link.send("ACK");                             // Acquittal
                            }
                            catch (Exception e) {
                                Log.d(ReceiveHandler.class.getSimpleName(), "An exception occured :" + e);
                            }
                        }

                    }
                    try {
                        Thread.sleep(500);                                  // For scheduling
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
