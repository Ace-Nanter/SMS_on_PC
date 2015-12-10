/**
 * USB class : Gère toutes les communications entre l'ordinateur
 * et le terminal Android
 * Created by Adrien PIERREVAL on 08/12/2015.
 */

package fr.isima.sms_on_pc;

import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.os.ParcelFileDescriptor;
import android.content.Context;
import android.hardware.usb.UsbAccessory;
import android.hardware.usb.UsbManager;
import android.system.ErrnoException;
import android.system.OsConstants;
import android.util.Log;                                        // TODO : to remove

import java.nio.ByteBuffer;
import java.io.IOException;
import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.io.FileOutputStream;

public class USB {
    private static final int TENTATIVES_MAX = 5;                // 5 tentatives de connexion
    private static final int CONNECTION_TIMEOUT = 1000;         // Timeout de connexion : 1 seconde
    private static final int STOP = -1;                         // Constante qui arrête le thread de réception
    private static final int READ = 1;                          // Constante qui ordonne au thread de réception de s'exécuter

    private UsbManager m_manager;                               // Manager USB d'Android
    private UsbAccessory[] m_list_devices;                      // Liste des périphériques
    private ParcelFileDescriptor m_parcelFileDescriptor;        // Récupération des flux (?)

    private Listener m_listener;                                // Interface
    private ReceiveThread m_thread;                             // Thread de réception
    private Handler m_handler = null;                           // Permet de communiquer avec le thread de réception

    private boolean m_connected = false;                        // Booléen pour savoir si on est connecté ou non au PC
    private boolean m_stop = false;                             // Booléen pour savoir si la connexion est active ou non

    // Pour la lecture
    private FileInputStream m_input;                            // Stream d'entrée
    private FileOutputStream m_output;                          // Stream de sortie

    private String m_last_msg;                                  // Dernier message lu

    public USB(final Context context, final Listener listener) throws Exception {
        if (context == null || listener == null) {
            throw new Exception("Problème lors de l'instanciation de la connexion USB !");
        }
        m_listener = listener;
        m_manager = (UsbManager) context.getSystemService(Context.USB_SERVICE);
    }

    public boolean connect() {
        int tentatives = 0;
        while (!m_connected && tentatives < TENTATIVES_MAX) {
            m_list_devices = m_manager.getAccessoryList();
            if (m_list_devices != null && m_list_devices.length != 0) {
                m_parcelFileDescriptor = m_manager.openAccessory(m_list_devices[0]);
                if (m_parcelFileDescriptor != null) {
                    Log.d(USB.class.getSimpleName(), "Connexion réussie");
                    final FileDescriptor fd = m_parcelFileDescriptor.getFileDescriptor();
                    m_connected = true;         // Le device est connecté
                    m_output = new FileOutputStream(fd);
                    m_input = new FileInputStream(fd);

                    // Démarrage du thread
                    m_thread = new ReceiveThread();
                    m_thread.start();
                    m_handler.sendEmptyMessage(READ);
                }
            }

            // On laisse le temps avant de réessayer
            try {
                Thread.sleep(CONNECTION_TIMEOUT);
            }
            catch (InterruptedException e) {
                tentatives --;                                              // Si le sleep échoue on laisse une tentative de plus
            }

            tentatives++;
        }       // Fin While

        return m_connected;
    }

    // A rajouter dans le main : extends Activity implements AndroidOpenAccessoryBridge.Listener
    // pour définir les méthodes de l'interface ensuite
    // TODO : a redefinir
    public interface Listener {                     // Interface
        void hasRead(String s);                     // On a lu quelque chose
        void UsbStop();                             // Le Thread a été arrêté
    }

    private class ReceiveThread extends Thread {
        @Override
        public void run() {
            Looper.prepare();
            m_handler = new Handler() {
                @Override
                public void handleMessage(Message msg) {
                    switch (msg.what) {
                        case STOP:
                            Looper.myLooper().quit();
                            break;
                        case READ:
                            try {
                                if (read()) {
                                    m_listener.hasRead(m_last_msg);
                                    m_handler.sendEmptyMessage(READ);       // On relance
                                }
                            } catch (IOException e) {
                                m_handler.sendEmptyMessage(STOP);           // On arrête le Thread
                                // TODO : ajouter un log ici
                            }
                            break;
                    }
                }
            };
            Looper.loop();              // Le thread boucle
        }
    }

    public boolean read() throws IOException {
        boolean flag = true;                                                // Flag pour s'assurer que tout va bien
        int readSize = -1;                                                  // Nombre de bits lus
        final int size;                                                     // Nombre final de bits de données
        byte[] dataSize = new byte[2];                                      // Buffer pour récupérer le nombre de bits de données

        final ByteBuffer buffer = ByteBuffer.allocate(0xffff);              // Buffer pour récupérer les données

        // Récupération du nombre d'octets de données
        try {
            readSize = m_input.read(dataSize);
        } catch (IOException e) {
            if (ioExceptionIsNoSuchDevice(e)) {
                throw e;
            }
            flag = false;
        }
        if (readSize != dataSize.length) {
            flag = false;
            throw new IOException("Problème lors de la lecture dans le buffer");
        } else {
            size = ((dataSize[0] & 0xff) << 8) | (dataSize[1] & 0xff);
        }

        // Récupération des données
        if (flag) {
            try {
                readSize = m_input.read(buffer.array(), 0, size);
            } catch (IOException e) {
                if (ioExceptionIsNoSuchDevice(e)) {
                    throw e;
                }
                flag = false;
            }
            if (readSize != size) {
                flag = false;
                throw new IOException("Incorrect Size of Data !");
            }
            m_last_msg = null;
            m_last_msg = new String(buffer.array(), 0, size);
        }

        return flag;
    }

    public boolean is_connected() {
        return m_connected;
    }

    public boolean is_running() {
        return m_stop;
    }

    /* Mot clé synchronized : empêche l'appel de variables en même temps : mot clé magique,
    permet le partage des ressource comme avec des mutex */
    public synchronized boolean write(String s) throws Exception {
        return false;
    }

    public void stop() {
        // Arrêt du thread de réception
        if(m_handler != null) {
            m_handler.sendEmptyMessage(STOP);
        }
        m_listener.UsbStop();                               // On prévient que le thread est arrêté

        // Suppression des variables
        m_parcelFileDescriptor = null;
        m_input = null;
        m_output = null;
        m_handler = null;
        m_thread = null;
        m_listener = null;

        m_connected = false;
        m_stop = true;                                      // La classe va devenir inactive
    }

    private boolean ioExceptionIsNoSuchDevice(IOException ioException) {
        final Throwable cause = ioException.getCause();
        if (cause instanceof ErrnoException) {
            final ErrnoException errnoException = (ErrnoException) cause;
            return errnoException.errno == OsConstants.ENODEV;
        }
        return false;
    }
}