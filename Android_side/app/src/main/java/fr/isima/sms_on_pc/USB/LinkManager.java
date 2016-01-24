/**
 * USB class : Gère toutes les communications entre l'ordinateur
 * et le terminal Android
 * Created by Adrien PIERREVAL on 08/12/2015.
 */

package fr.isima.sms_on_pc.USB;

import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.os.ParcelFileDescriptor;
import android.content.Context;
import android.hardware.usb.*;
import android.util.Log;                                        // TODO : to remove

import java.io.IOException;
import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.io.FileOutputStream;

public class LinkManager {
    private static final int TENTATIVES_MAX = 5;                // 5 tentatives de connexion
    private static final int CONNECTION_TIMEOUT = 2000;         // Timeout de connexion : 2 secondes

    private UsbManager m_manager;                               // Manager USB d'Android
    private ParcelFileDescriptor m_device;                      // Device récupéré

    private UsbInterface m_listener;                            // Interface

    private boolean m_connected = false;                        // Booléen pour savoir si on est connecté ou non au PC

    // Read/Write variables
    private Receiver m_receiver = null;                         // Receiver
    private Sender m_sender = null;                             // Sender

    public LinkManager(final Context context, final UsbInterface listener) throws Exception {
        if (context == null || listener == null) {
            throw new Exception("Problème lors de l'instanciation de la connexion USB !");
        }
        m_manager = (UsbManager) context.getSystemService(Context.USB_SERVICE);
        m_listener = listener;

        connect();
    }

    /**
     * Connect to the device
     * @return true if the connection is established otherwise wrong
     */
    public boolean connect() {
        int tentatives = 0;
        while (!m_connected && tentatives < TENTATIVES_MAX) {
            final UsbAccessory[] m_list_devices = m_manager.getAccessoryList();     // Récupération de la liste des devices connectés
            if (m_list_devices != null && m_list_devices.length > 0) {
                m_device = m_manager.openAccessory(m_list_devices[0]);              // Récupération du device
                if (m_device != null) {
                    final FileDescriptor fd = m_device.getFileDescriptor();         // Récupération du File Descriptor
                    if(fd != null) {
                        try {
                            m_sender = new Sender(fd);
                            m_receiver = new Receiver(fd, m_listener);
                            m_connected = true;                                     // Le device est connecté
                        }
                        catch (Exception e) {
                            m_connected = false;
                        }
                    }
                }
            }
            // On laisse le temps avant de réessayer
            try {
                Thread.sleep(CONNECTION_TIMEOUT);
            } catch (InterruptedException e) {
                tentatives--;                                                       // Si le sleep échoue on laisse une tentative de plus
            }
            tentatives++;
        }
        return m_connected;
    }

    /**
     * Get the descriptors of the connected devices
     * @return A string containing the descriptors
     */
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

    /**
     * Say if a device is connected or not
     * @return True or false
     */
    public boolean is_connected() {
        return m_connected;
    }

    /**
     * Say if Sender and Receiver are running
     * @return True or false
     */
    public boolean isOK() {
        return (m_receiver.is_active() && m_sender.is_active());
    }

    /**
     * Transmit the message to send to the sender
     * @param msg
     */
    public void send(String msg) {
        m_sender.send(msg);
    }

    /**
     * Disconnect Android from USB
     */
    public void disconnect() {
        if (m_connected) {
            m_connected = false;
        }
        if (m_sender != null && m_sender.is_active()) {
            if(m_sender.stop()) {
                Log.d(LinkManager.class.getSimpleName(), "There was non-send message !");
                // TODO : autre traitement ?
            }
        }
        if(m_receiver != null && m_receiver.is_active()) {
            m_receiver.stop();
        }

        if (m_device != null) {
            try {
                m_device.close();
            } catch (IOException e) { }
            m_device = null;
        }

        m_listener.UsbStop();
    }
}