package fr.isima.sms_on_pc.SMS;

import android.telephony.SmsManager;
import android.telephony.SmsMessage;

/**
 * Created by Ace Nanter on 03/01/2016.
 */

public class SMS {

    private int m_ID;
    private String m_phone_number;
    private String m_body;

    public SMS() {
        m_ID = -1;
        m_phone_number = "";
        m_body = "";
    }

    /**
     * Constructor for received SMS
     * @param phone_number Phone number of the sender
     * @param body Body of the message
     */
    public SMS(String phone_number, String body) {
        this();
        m_phone_number = phone_number;
        m_body = body;
    }

    /**
     * Constructor for sent SMS
     * @param ID ID of the message (PC)
     * @param phone_number Phone number to send
     */
    public SMS(int ID, String phone_number) {
        this();
        m_ID = ID;
        m_phone_number = phone_number;
    }

    /**
     * Add some text to the body
     * @param part text to add
     */
    public void appendBody(String part) {
        m_body += part;
    }

    // TODO : to replace
    public void send() {
        SmsManager manager = SmsManager.getDefault();

        manager.sendTextMessage("0647657049", null, "Coucou !", null, null);
    }



}
