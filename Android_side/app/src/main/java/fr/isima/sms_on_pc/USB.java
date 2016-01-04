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
import android.hardware.usb.*;
import android.util.Log;                                        // TODO : to remove

import java.nio.ByteBuffer;
import java.io.IOException;
import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.io.FileOutputStream;

public class USB {
    private static final int TENTATIVES_MAX = 5;                // 5 tentatives de connexion
    private static final int CONNECTION_TIMEOUT = 2000;         // Timeout de connexion : 2 secondes

    // Signaux utilisés
    private static final int STOP = -1;
    private static final int DONOTHING = 0;
    private static final int READ = 1;
    private static final int CONNECT = 2;

    private UsbManager m_manager;                               // Manager USB d'Android
    private ParcelFileDescriptor m_device;                      // Device récupéré

    private Listener m_listener;                                // Interface
    private USBThread m_thread;                                 // Thread de réception

    private boolean m_connected = false;                        // Booléen pour savoir si on est connecté ou non au PC
    private boolean m_stop = false;                             // Booléen pour savoir si la connexion est active ou non

    // Read/Write variables
    private FileInputStream m_input;                            // Stream d'entrée
    private FileOutputStream m_output;                          // Stream de sortie

    private String m_last_msg;                                  // Dernier message lu

    public USB(final Context context, final Listener listener) throws Exception {
        if (context == null || listener == null) {
            throw new Exception("Problème lors de l'instanciation de la connexion USB !");
        }
        m_manager = (UsbManager) context.getSystemService(Context.USB_SERVICE);
        m_listener = listener;

        m_thread = new USBThread();
        m_thread.start();


    }

    public final String get_descriptors() {
        final UsbAccessory device;

        String retour = "Aucun appareil détecté";
        final UsbAccessory[] list_devices = m_manager.getAccessoryList();


        if(list_devices != null && list_devices.length > 0) {
            device = list_devices[0];
            retour = "Description : " + device.getDescription();
            retour += "\nManufacturer : " + device.getManufacturer();
            retour += "\nModel : " + device.getModel();
            retour += "\nSerial number : " + device.getSerial();
            retour += "\nURI : " + device.getUri();
        }
        return retour;
    }

    public boolean is_connected() {
        return m_connected;
    }

    public boolean is_running() {
        return m_stop;
    }

    private class USBThread extends Thread {
        private Handler m_handler;                                                  // Gestionnaire de messages pour le thread de réception

        @Override
        public void run() {
            Looper.prepare();
            m_handler = new Handler() {
                @Override
                public void handleMessage(Message msg) {
                    switch (msg.what) {
                        case DONOTHING:
                            m_handler.sendEmptyMessageDelayed(DONOTHING, 100);      // On ne fait rien jusqu'à que le signal change
                        break;
                        case CONNECT:
                            if(connect()) {
                                m_listener.hasBeenConnected();
                                m_handler.sendEmptyMessage(READ);                   // Une fois connecté on lance la lecture
                            }
                        break;
                        case READ:
                            Log.d(USB.class.getSimpleName(), "J'essaye de lire");   // TODO : to remove
                            if (read()) {
                                Log.d(USB.class.getSimpleName(), "Appel au listener");
                                m_listener.hasRead(m_last_msg);
                            }


                            m_handler.sendEmptyMessageDelayed(READ, 100);       // On relance la lecture


                        break;
                        case STOP:
                            Looper.myLooper().quit();
                        break;
                    }
                }
            };
            m_handler.sendEmptyMessage(DONOTHING);
            Looper.loop();              // Le thread boucle
            m_handler = null;
        }

        // Fonction de connexion au device
        public boolean connect() {
            int tentatives = 0;
            while (!m_connected && tentatives < TENTATIVES_MAX) {
                final UsbAccessory[] m_list_devices = m_manager.getAccessoryList();
                if (m_list_devices != null && m_list_devices.length > 0) {
                    m_device = m_manager.openAccessory(m_list_devices[0]);
                    if (m_device != null) {
                        Log.d(USB.class.getSimpleName(), "Connexion réussie");              // TODO : to remove
                        final FileDescriptor fd = m_device.getFileDescriptor();
                        if(fd != null) {
                            m_connected = true;                                     // Le device est connecté
                            // Récupération des flux
                            m_output = new FileOutputStream(fd);
                            m_input = new FileInputStream(fd);
                        }
                    }
                }
                // On laisse le temps avant de réessayer
                try {
                    Thread.sleep(CONNECTION_TIMEOUT);
                } catch (InterruptedException e) {
                    tentatives--;                                                   // Si le sleep échoue on laisse une tentative de plus
                }
                tentatives++;
            }       // Fin While
            return m_connected;
        }
    }

    public boolean read() {
        boolean flag = true;                                                // Flag pour s'assurer que tout va bien
        int readSize = -1;                                                  // Nombre de bits lus
        byte[] buffer = new byte[16384];                                    // Buffer pour récupérer le nombre de bits de données

        try {
            m_last_msg = ">>";
            readSize = m_input.read(buffer);
                String msg = new String(buffer);
                m_last_msg += msg;

        } catch (IOException e) {
            e.printStackTrace();
            Log.d(USB.class.getSimpleName(), "Read error, exception");      // TODO : to remove
            this.stop();
            flag = false;
        }
        return flag;
    }

    public boolean write(String msg) {
        boolean flag = true;
        byte[] buffer = msg.getBytes();

        try {
            m_output.write(buffer, 0, buffer.length);
        } catch (IOException e) {
            e.printStackTrace();
            Log.d(USB.class.getSimpleName(), "Write error, exception");
            flag = false;
        }
        return flag;
    }

    // Lance la connexion
    public void connect() {
        m_thread.m_handler.sendEmptyMessage(CONNECT);
    }

    // Déconnecte l'appareil Android
    private void disconnect() {
        if (m_connected) {
            m_connected = false;
        }
        if (m_input != null) {
            try {
                m_input.close();
            } catch (IOException e) { }
            m_input = null;
        }
        if(m_output != null) {
            try {
                m_output.close();
            } catch (IOException e) { }
            m_output = null;
        }
        if (m_device != null) {
            try {
                m_device.close();
            } catch (IOException e) { }
            m_device = null;
        }
    }

    public void stop() {
        // Arrêt du thread de réception
        m_thread.m_handler.sendEmptyMessage(STOP);          // Arrêt du thread
        disconnect();                                       // Déconnexion
        m_listener.UsbStop();                               // On prévient que le thread est arrêté

        // Suppression des variables
        m_listener = null;
        m_stop = true;
    }

    public interface Listener {                     // Interface
        void hasRead(String s);                     // On a lu quelque chose
        void hasBeenConnected();                    // On vient de se connecter
        void UsbStop();                             // Le Thread a été arrêté
    }
}