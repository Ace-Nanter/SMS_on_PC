package fr.isima.sms_on_pc.USB;

import android.util.Log;

import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.util.PriorityQueue;

/**
 * Created by Ace Nanter on 19/01/2016.
 */
public class Receiver {
    private boolean m_stop = false;                     // Définit si le receiver est actif ou non
    private readThread m_readThread = null;             // Thread qui permet de recevoir
    private FileInputStream m_input = null;             // Canal d'entrée
    private PriorityQueue<String> m_Received;                   // Pile qui contient les messages reçus

    private UsbInterface m_listener;                    // Listener pour prévenir l'interface graphique

    public Receiver(FileDescriptor fd, UsbInterface listener) throws Exception {
        if(fd == null && listener == null) {
            m_stop = true;
            Log.d(Receiver.class.getSimpleName(), "Erreur lors de l'instanciation du Receiver !");
            throw new Exception("Exception occurred when creating Receiver");
        }
        m_input = new FileInputStream(fd);
        m_listener = listener;

        m_Received = new PriorityQueue<>();

        m_readThread = new readThread();
        m_readThread.start();
    }

    public boolean is_active() { return (!m_stop); }

    private class readThread extends Thread {
        @Override
        public void run() {
            int dataLength;
            String msg;
            byte[] sizeBuffer = new byte[5];
            byte[] buffer;

            while(!m_stop) {
                try {
                    // Get the length
                    if (read(sizeBuffer)) {
                        dataLength = getInt(sizeBuffer);
                        if(dataLength > 0) {
                            // Get the message
                            buffer = new byte[dataLength];
                            if (read(buffer)) {
                                msg = new String(buffer);
                                if(msg != null && !msg.isEmpty()) {
                                    // Transmit what was received
                                    m_Received.add(msg);
                                    m_listener.hasRead(msg);
                                }
                            }
                        }
                    }
                }
                // If an exception occured, stop the receiver
                catch (Exception e) {
                    Log.d(Receiver.class.getSimpleName(), "An exception occured : "+ e);
                    Receiver.this.stop();
                }
            }   // End while
        }

        private boolean read(byte[] buffer) throws Exception {
            int readSize = -1;

            readSize = m_input.read(buffer);

            return (readSize > 0);
        }

        private int getInt(byte[] buffer) {
            int dataLength = -1;
            String tmp = new String(buffer);

            if (tmp != null && !tmp.isEmpty()) {
                dataLength = Integer.parseInt(tmp.replaceAll("[^a-zA-Z0-9]", ""));
            }

            return dataLength;
        }
    }

    public String getLast() {
        return m_Received.poll();
    }

    public boolean stop() {
        try {
            m_stop = true;
            if(m_readThread.isAlive()) {
                m_readThread.interrupt();
            }

            m_input.close();
        }
        catch (Exception e) {
            Log.d(Receiver.class.getSimpleName(), "Erreur lors de l'arrêt du Receiver : " + e);
        }

        m_input = null;
        m_readThread = null;

        return (m_Received.isEmpty());
    }
}