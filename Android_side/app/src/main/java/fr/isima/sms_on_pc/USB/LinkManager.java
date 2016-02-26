/**
 * USB class : Gère toutes les communications entre l'ordinateur
 * et le terminal Android
 * Created by Adrien PIERREVAL on 08/12/2015.
 */

package fr.isima.sms_on_pc.USB;

import android.content.BroadcastReceiver;
import android.content.Intent;
import android.os.ParcelFileDescriptor;
import android.content.Context;
import android.hardware.usb.*;
import android.util.Log;

import java.io.FileDescriptor;

public final class LinkManager {
    private static final int TENTATIVES_MAX = 5;                // 5 tentatives de connexion
    private static final int CONNECTION_TIMEOUT = 2000;         // Timeout de connexion : 2 secondes

    private UsbInterface m_listener;                            // Interface

    private UsbManager m_manager;                               // Manager USB d'Android
    private ParcelFileDescriptor m_device;                      // Device récupéré

    private Thread m_thread;                                    // Thread pour la connexion
    private BroadcastReceiver m_disconnectReceiver;             // Détecte une déconnexion

    private boolean m_connected = false;                        // Booléen pour savoir si on est connecté ou non au PC

    // Read/Write variables
    private Receiver m_receiver = null;                         // Receiver
    private Sender m_sender = null;                             // Sender
    private ReceiveHandler m_handler = null;                    // Handler

    private static volatile LinkManager m_instance = null;      // Instance du LinkManager

    /**
     * Default constructor - private for singleton pattern - not used
     */
    private LinkManager() {
        super();
    }

    /**
     * Constructor used - private for singleton pattern
     * @param context Context to get USB context
     * @param listener Listener to fire events
     * @throws Exception
     */
    private LinkManager(final Context context, final UsbInterface listener) throws Exception {
        if (context == null || listener == null) {
            throw new Exception("Problème lors de l'instanciation de la connexion USB !");
        }

        final LinkManager link = this;
        m_manager = (UsbManager) context.getSystemService(Context.USB_SERVICE);
        m_listener = listener;

        m_thread = new Thread(new Runnable() {
            /**
             * Connect to the device
             */
            @Override
            public void run() {
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
                                    m_handler = new ReceiveHandler(link);
                                    m_connected = true;                                     // Le device est connecté
                                    send("OK");                                             // Envoi d'un acquittement au PC
                                }
                                catch (Exception e) {
                                    m_connected = false;
                                    Log.d(LinkManager.class.getSimpleName(), "An exception occured : " + e);
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
                m_listener.Connected(m_connected);                                          // On prévient l'interface graphique
            }
        });

        m_thread.start();

        m_disconnectReceiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                String action = intent.getAction();

                if (UsbManager.ACTION_USB_ACCESSORY_DETACHED.equals(action)) {
                    UsbAccessory accessory = intent.getParcelableExtra(UsbManager.EXTRA_ACCESSORY);
                    if (accessory != null) {
                        disconnect();
                    }
                }
            }
        };
    }

    /**
     * Get an instance of Link Manager
     * @param context Context to transmit to the constructor
     * @param listener Listener to transmit to the listener
     * @return THE instance of Link Manager
     * @throws Exception
     */
    public final static LinkManager getInstance(final Context context, final UsbInterface listener)
        throws Exception {
        if(LinkManager.m_instance == null) {
            synchronized (LinkManager.class) {
                if(LinkManager.m_instance == null) {
                    LinkManager.m_instance = new LinkManager(context, listener);
                }
            }
        }

        return LinkManager.m_instance;
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
     * Get the last String sent
     * @return The last String sent
     */
    public String getLastReceived() {
        return m_receiver.getLast();
    }

    /**
     * Pop the sent list next to the receiving of an acquittance
     */
    public void popSentList() {
        m_sender.pop();
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

        if(m_handler != null) {
            m_handler.stop();
        }

        if (m_sender != null && m_sender.is_active()) {
            if(m_sender.stop()) {
                Log.d(LinkManager.class.getSimpleName(), "There was non-send message !");
                // TODO : autre traitement ?
            }
        }
        if(m_receiver != null && m_receiver.is_active()) {
            if(m_receiver.stop()) {
                Log.d(LinkManager.class.getSimpleName(), "All the message were not handled !");
                // TODO : autre traitement ?
            }
        }

        if (m_device != null) {
        /*    try {
                m_device.close();
            } catch (IOException e) { }*/
            m_device = null;
        }

        m_listener.UsbStop();
    }
}