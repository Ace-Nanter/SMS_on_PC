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
                String toHandle;

                while(!m_stop) {
                    toHandle = m_link.getLastReceived();
                    if(toHandle != null && !toHandle.isEmpty()) {
                        m_link.send("ACK");                                 // Acquittal
                        if(toHandle.startsWith("SMSHEADER")) {              // SMS Header
                            String args[] = toHandle.split(":");
                            if(checkHeader(args)) {
                                SMS sms = new SMS(Integer.parseInt(args[1]), args[2]);
                                for(int i = 0 ; i < Integer.parseInt(args[3]) ; i++) {
                                    toHandle = m_link.getLastReceived();

                                    // Wait for the stream
                                    while(toHandle == null) {
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
                                sms.send();                                 // Send the message
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
                            Log.d("Debug", "Yolo y a un ACK je déqueue");
                            m_link.popSentList();
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
