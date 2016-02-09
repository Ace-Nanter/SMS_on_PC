package fr.isima.sms_on_pc.SMS;

import android.telephony.SmsManager;

/**
 * Created by Ace Nanter on 03/01/2016.
 */

public class SMS {

    public SMS() {

    }

    public void send() {
        SmsManager manager = SmsManager.getDefault();
        manager.sendTextMessage("0647657049", null, "Coucou !", null, null);
    }



}
