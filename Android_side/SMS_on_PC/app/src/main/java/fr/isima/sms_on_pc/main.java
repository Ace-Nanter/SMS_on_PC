package fr.isima.sms_on_pc;

import android.content.Context;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.hardware.usb.*;
import android.widget.TextView;

public class main extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        String s = "";
        UsbManager m_USB_manager;
        UsbAccessory[] m_list_devices;
        TextView console = null;

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        m_USB_manager = (UsbManager) getSystemService(Context.USB_SERVICE);
        m_list_devices = m_USB_manager.getAccessoryList();

        s = "Devices trouvés : " + m_list_devices.length;

        console = (TextView) findViewById(R.id.console);
        console.setText(s);
    }
}
