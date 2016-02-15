package fr.isima.sms_on_pc.USB;

import android.util.Log;                        // TODO : to remove

import java.io.FileDescriptor;
import java.io.FileOutputStream;
import java.util.PriorityQueue;

/**
 * Created by Ace Nanter on 19/01/2016.
 */

public class Sender {

    private boolean m_stop = false;
    private writeThread m_writeThread = null;
    private FileOutputStream m_output = null;
    private PriorityQueue<String> m_toSend;

    public Sender(FileDescriptor fd) throws Exception {
        if(fd == null) {
            m_stop = true;
            Log.d(Sender.class.getSimpleName(), "Erreur lors de l'instanciation du Sender !");
            throw new Exception("Exception occurred when creating Sender");
        }

        m_output = new FileOutputStream(fd);
        m_toSend = new PriorityQueue<>();

        m_output.flush();                                   // Flush du buffer de sortie
        m_writeThread = new writeThread();                  // Création du thread
        m_writeThread.start();                              // Lancement du thread
    }

    public void send(String msg) {
        m_toSend.add(msg);
    }

    public boolean is_active() {
        return (!m_stop);
    }

    private class writeThread extends Thread {
        @Override
        public void run() {
            int tryLeft = 5;
            String msg;
            String length;
            byte[] sizeBuffer;
            byte[] buffer;

            while (!m_stop) {
                if (!m_toSend.isEmpty()) {
                    msg = m_toSend.peek();                                              // Récupération de la chaîne à envoyer
                    if(msg != null && (!msg.isEmpty())) {
                        tryLeft = 5;

                        // Tant qu'on a pas réussi à envoyer
                        while((!m_toSend.isEmpty()) && msg.equals(m_toSend.peek()) && tryLeft > 0) {
                            try {
                                length = "" + (msg.getBytes()).length;                  // Récupération du nombre d'octets
                                sizeBuffer = length.getBytes();                         // Transcription en binaire
                                m_output.write(sizeBuffer, 0, sizeBuffer.length);       // Envoi

                                Thread.sleep(50);

                                buffer = msg.getBytes();                                // Mise au format binaire
                                m_output.write(buffer, 0, buffer.length);               // Envoi
                            }
                            catch (Exception e) {
                                Log.d(Sender.class.getSimpleName(), "An exception occured : " + e);
                            }

                            // Si on envoie un ACK on attend pas la confirmation
                            if(msg.equals("ACK")) {
                                pop();
                                break;
                            }

                            try {
                                Thread.sleep(500);                                      // Sleep pour l'ordonnancement et nouvelle tentative
                            }
                            catch(Exception e) {
                                Log.d(Sender.class.getSimpleName(), "Exception occured during a sleep : " + e);
                            }

                            tryLeft--;
                        }

                        if((!m_toSend.isEmpty()) && msg.equals(m_toSend.peek()))                                 // Echec
                            Log.d(Sender.class.getSimpleName(), "Fail to send : " + msg);
                            pop();
                    }   // End while

                    try {
                        Thread.sleep(500);
                    }
                    catch(Exception e) {
                        Log.d(Sender.class.getSimpleName(), "Exception occured during a sleep : " + e);
                    }
                }
            }           // End while
        }               // End run
    }                   // End Thread

    /**
     * Pop the FIFO
     */
    public void pop() {
        m_toSend.poll();
    }

    public boolean stop() {
        try {
            m_stop = true;
            if(m_writeThread.isAlive()) {
                m_writeThread.interrupt();
            }

            m_output.flush();
            m_output.close();
        }
        catch (Exception e) {
            Log.d(Sender.class.getSimpleName(), "Erreur lors de l'arrêt du Sender : " + e);
        }

        m_output = null;
        m_writeThread = null;

        return (m_toSend.isEmpty());
    }
}
