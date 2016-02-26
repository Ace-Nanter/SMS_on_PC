package fr.isima.sms_on_pc.SMS;

import android.content.ContentValues;
import android.content.Context;
import android.net.Uri;
import android.telephony.SmsManager;
import android.util.Log;

import java.util.ArrayList;
import java.util.Calendar;

import fr.isima.sms_on_pc.USB.LinkManager;


/**
 * Created by Ace Nanter on 03/01/2016.
 */

public class SMS {

    private static Context m_context = null;    // Context

    private int m_ID;                           // ID of the message
    private String m_phoneNumber;              // Phone number
    private String m_body;                      // Body of the message
    private Calendar m_date;                    // Date du message

    public SMS() {
        m_ID = -1;
        m_phoneNumber = "";
        m_body = "";
    }

    /**
     * Constructor for received SMS
     * @param phone_number Phone number of the sender
     * @param body Body of the message
     */
    public SMS(String phone_number, String body, long date) {
        this();
        m_phoneNumber = phone_number;
        m_body = body;
        m_date = Calendar.getInstance();
        m_date.setTimeInMillis(date);
    }

    /**
     * Constructor for sent SMS
     * @param ID ID of the message (PC)
     * @param phone_number Phone number to send
     */
    public SMS(int ID, String phone_number) {
        this();
        m_ID = ID;
        m_phoneNumber = phone_number;
    }

    /**
     * Initialise the context of the SMS sender
     * @param context Activity to get
     */
    public static void init(Context context) {
        m_context = context;
    }

    /**
     * Add some text to the body
     * @param part text to add
     */
    public void appendBody(String part) { m_body += part; }

    public void send() {
        SmsManager manager = SmsManager.getDefault();

        // Prepare the multiple parts of the message
        ArrayList<String> parts = manager.divideMessage(m_body);
        int messageCount = parts.size();

        // Do the sending

        if(messageCount > 1) {
            manager.sendMultipartTextMessage(m_phoneNumber, null, parts, null, null);
        }
        else {
            manager.sendTextMessage(m_phoneNumber, null, m_body, null, null);
        }

        // Add the message to the database
        if(m_context != null) {
            ContentValues values = new ContentValues();
            values.put("address", m_phoneNumber);
            values.put("body", m_body);

            Uri sentURI = Uri.parse("content://sms/sent");
            m_context.getContentResolver().insert(sentURI, values);
        }
    }

    /**
     * Send the message to the computer by USB
     */
    public void sendUSB() {
        int nbComs;
        final int limit = 80;
        String buffer = "";

        LinkManager manager = null;

        if(m_body == null || m_body == "") {
            Log.d(SMS.class.getSimpleName(), "Empty message !");
        }

        // Get the manager
        try {
            manager = LinkManager.getInstance(null, null);
        }
        catch(Exception e) {
            Log.d(SMS.class.getSimpleName(), "An exception occurred : " + e);
            manager = null;
        }

        if(manager != null) {
            // Check if the message is multiparted or not
            if(m_body.length() < 80) {
                // Send the header
                buffer = "SMSHEADER:" + m_phoneNumber;
                buffer += ":" + m_date.getTime().toString();
                buffer += ":" + 1;

                Log.d(SMS.class.getSimpleName(), "header : " + buffer);

                manager.send(buffer);

                // Send the body
                manager.send("SMSBODY:1:" + m_body);

            }
            else {
                // Send the header
                nbComs = (m_body.length() + limit - 1) / limit;
                buffer = "SMSHEADER:" + m_phoneNumber + ":";
                buffer += ":" + m_date.getTime().toString();
                buffer += ":" + nbComs;

                manager.send(buffer);

                // Send the body
                int com = 1;
                for (int i = 0; i < m_body.length() ; i+= limit) {
                    buffer = "SMSBODY:" + com  + ":";
                    buffer += m_body.substring(i, i + Math.min(limit, m_body.length() - i));
                    com++;

                    manager.send(buffer);
                }
            }
        }
        else {
            Log.d(SMS.class.getSimpleName(), "Manager problem !");
        }

    }
}
