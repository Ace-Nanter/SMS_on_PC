package fr.isima.sms_on_pc;

import android.content.Context;
import android.os.ParcelFileDescriptor;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.TextView;
import android.hardware.usb.*;

import java.io.FileDescriptor;
import java.io.FileInputStream;


public class Main extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        String s;
        UsbManager m_USB_manager;
        UsbAccessory[] m_list_devices;
        UsbAccessory mAccessory;
        ParcelFileDescriptor mFileDescriptor;
        FileInputStream mInputStream;
        TextView console;

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        console = (TextView) findViewById(R.id.console);

        m_USB_manager = (UsbManager) getSystemService(Context.USB_SERVICE);
        m_list_devices = m_USB_manager.getAccessoryList();

        if(console != null) {
            if(m_list_devices != null) {
                s = "Device trouvés : " +  m_list_devices.length;
                UsbAccessory PC = m_list_devices[0];

                // Test de read

                mFileDescriptor = m_USB_manager.openAccessory(PC);
                if(mFileDescriptor != null) {
                    FileDescriptor fd = mFileDescriptor.getFileDescriptor();
                    mInputStream = new FileInputStream(fd);
                    //mOutputStream = new FileOutputStream(fd);
                    Thread thread = new Thread(null, this, "AccessoryThread");
                    thread.start();
                }


            }
            else {
                s = "Aucun device trouvé !";
            }

            console.setText(s);
        }
    }
}
