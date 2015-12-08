/**
 * USB class : Gère toutes les communications entre l'ordinateur
 * et le terminal Android
 * Created by Adrien PIERREVAL on 08/12/2015.
 */

package fr.isima.sms_on_pc;

import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.os.ParcelFileDescriptor;
import android.hardware.usb.UsbAccessory;
import android.hardware.usb.UsbManager;

import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.io.FileOutputStream;

public class USB {
    private static final int TENTATIVES_MAX = 5;                // 5 tentatives de connexion
    private static final int CONNECTION_TIMEOUT = 2000;         // Timeout de connexion : 2 secondes
    private static final int STOP = -1;                         // Constante qui arrête le thread de réception

    private UsbManager m_manager;                               // Manager USB d'Android
    private UsbAccessory[] m_list_devices;                      // Liste des périphériques
    private ParcelFileDescriptor m_parcelFileDescriptor;        // Récupération des flux (?)
    private Byte[] m_buffer;                                    // Buffer pour récupérer/envoyer les messages

    private Listener m_listener;                                // Interface
    private ReceiveThread m_thread;                             // Thread de réception
    private Handler m_handler;                                  // Permet de communiquer avec le thread de réception

    private boolean m_connected = false;                        // Booléen pour savoir si on est connecté ou non au PC
    private boolean m_stop = false;                             // Arrêt total de la connexion USB

    // Pour la lecture
    private char[] buffer;                                      // Buffer pour récupérer les caractères, doit faire au minimum 16384 bytes
    private FileInputStream m_input;                            // Stream d'entrée
    private FileOutputStream m_output;                          // Stream de sortie


    public USB(final Context context, final Listener listener) throws Exception {
        if (context == null || listener == null) {
            throw new Exception("Problème lors de l'instanciation de la connexion USB !");
        }
        m_listener = listener;
        m_manager = (UsbManager) context.getSystemService(Context.USB_SERVICE);

        int i = 0;
        while (!m_connected && i < TENTATIVES_MAX) {
            m_list_devices = m_manager.getAccessoryList();
            m_parcelFileDescriptor = m_manager.openAccessory(m_list_devices[0]);
            if (m_parcelFileDescriptor != null) {
                final FileDescriptor fd = m_parcelFileDescriptor.getFileDescriptor();
                m_connected = true;         // Le device est connecté
                m_output = new FileOutputStream(fd);
                m_input = new FileInputStream(fd);
            }

            Thread.sleep(CONNECTION_TIMEOUT);
            i++;
        }

        // A partir d'ici l'appareil est censé être connecté
        m_thread = new ReceiveThread();
        m_thread.start();
    }

    // A rajouter dans le main : extends Activity implements AndroidOpenAccessoryBridge.Listener
    // pour définir les méthodes de l'interface ensuite
    // TODO : a redefinir
    public interface Listener {                         // Interface
        void onAobRead(byte[] buffer);                  // Read
        void onAoabShutdown();                          // Shutdown
    }


    private class ReceiveThread extends Thread {
        @Override
        public void run() {
            Looper.prepare();
            m_handler = new Handler() {
                @Override
                public void handleMessage(Message msg) {
                    if(msg.what == STOP) {
                        Looper.myLooper().quit();
                    }
                }
            };
            detectAccessory();
            Looper.loop();
            detachAccessory();
            mIsShutdown = true;
            mListener.onAoabShutdown();

            // Clean stuff up
            mHandler = null;
            mListener = null;
            mUsbManager = null;
            mReadBuffer = null;
            mInternalThread = null;
        }
    }

/*


            private static final int STOP_THREAD = 1;
            private static final int MAYBE_READ = 2;

            private Handler mHandler;

            @Override
            public void run() {
                Looper.prepare();
                mHandler = new Handler() {
                    @Override
                    public void handleMessage(Message msg) {
                        switch (msg.what) {
                            case STOP_THREAD:
                                Looper.myLooper().quit();
                                break;
                            case MAYBE_READ:
                                final boolean readResult;
                                try {
                                    readResult = mReadBuffer.read(mInputStream);
                                } catch (IOException exception) {
                                    terminate();
                                    break;
                                }
                                if (readResult) {
                                    if (mReadBuffer.size == 0) {
                                        mHandler.sendEmptyMessage(STOP_THREAD);
                                    } else {
                                        mListener.onAoabRead(mReadBuffer);
                                        mReadBuffer.reset();
                                        mHandler.sendEmptyMessage(MAYBE_READ);
                                    }
                                } else {
                                    mHandler.sendEmptyMessageDelayed(MAYBE_READ, READ_COOLDOWN_MS);
                                }
                                break;
                        }
                    }
                };
                detectAccessory();
                Looper.loop();
                detachAccessory();
                mIsShutdown = true;
                mListener.onAoabShutdown();

                // Clean stuff up
                mHandler = null;
                mListener = null;
                mUsbManager = null;
                mReadBuffer = null;
                mInternalThread = null;
            }
        }

        void terminate() {
            mHandler.sendEmptyMessage(STOP_THREAD);
        }

        private void detectAccessory() {
            while (!mIsAttached) {
                if (mIsShutdown) {
                    mHandler.sendEmptyMessage(STOP_THREAD);
                    return;
                }
                try {
                    Thread.sleep(CONNECT_COOLDOWN_MS);
                } catch (InterruptedException exception) {
                    // pass
                }
                final UsbAccessory[] accessoryList = mUsbManager.getAccessoryList();
                if (accessoryList == null || accessoryList.length == 0) {
                    continue;
                }
                if (accessoryList.length > 1) {
                    Log.w(TAG, "Multiple accessories attached!? Using first one...");
                }
                maybeAttachAccessory(accessoryList[0]);
            }
        }

        private void maybeAttachAccessory(final UsbAccessory accessory) {
            final ParcelFileDescriptor parcelFileDescriptor = mUsbManager.openAccessory(accessory);
            if (parcelFileDescriptor != null) {
                final FileDescriptor fileDescriptor = parcelFileDescriptor.getFileDescriptor();
                mIsAttached = true;
                mOutputStream = new FileOutputStream(fileDescriptor);
                mInputStream = new FileInputStream(fileDescriptor);
                mParcelFileDescriptor = parcelFileDescriptor;
                mHandler.sendEmptyMessage(MAYBE_READ);
            }
        }

        private void detachAccessory() {
            if (mIsAttached) {
                mIsAttached = false;
            }
            if (mInputStream != null) {
                closeQuietly(mInputStream);
                mInputStream = null;
            }
            if (mOutputStream != null) {
                closeQuietly(mOutputStream);
                mOutputStream = null;
            }
            if (mParcelFileDescriptor != null) {
                closeQuietly(mParcelFileDescriptor);
                mParcelFileDescriptor = null;
            }
        }

        private void closeQuietly(Closeable closable) {
            try {
                closable.close();
            } catch (IOException exception) {
                // pass
            }
        }

    }

*/






        };


    public boolean is_connected() {
        return m_connected;
    }

    public String read() throws Exception {
        return "Lapin";
    }

    /* Mot clé synchronized : empêche l'appel de variables en même temps : mot clé magique,
    permet le partage des ressource comme avec des mutex */
    public synchronized boolean write(String s) throws Exception {
        return false;
    }

    public void stop(void) {
        m_Handler.sendEmptyMessage(0);
        // TODO : Arrêter tout, supprimer les variables, et déconnecter le device.
    }
/*
    private void openAccessory() {
        Log.d(TAG, "openAccessory: " + accessory);
        mFileDescriptor = mUsbManager.openAccessory(mAccessory);
        if (mFileDescriptor != null) {
            FileDescriptor fd = mFileDescriptor.getFileDescriptor();
            mInputStream = new FileInputStream(fd);
            mOutputStream = new FileOutputStream(fd);
            Thread thread = new Thread(null, this, "AccessoryThread");
            thread.start();
        }
    }*/

}