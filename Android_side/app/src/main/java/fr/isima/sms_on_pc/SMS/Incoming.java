package fr.isima.sms_on_pc.SMS;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.telephony.SmsManager;
import android.telephony.SmsMessage;
import android.util.Log;

/**
 * Created by Ace Nanter on 09/02/2016.
 */


public class Incoming extends BroadcastReceiver {

    final SmsManager sms = SmsManager.getDefault();

    @Override
    public void onReceive(Context context, Intent intent) {
        // Get the object of SmsManager

        // Retrieves a map of extended data from the intent.
        final Bundle bundle = intent.getExtras();

        try {
            if (bundle != null) {
                final Object[] pdusObj = (Object[]) bundle.get("pdus");

                for (int i = 0; i < pdusObj.length; i++) {
                    SmsMessage currentMessage = SmsMessage.createFromPdu((byte[]) pdusObj[i]);

                    // Get the informations
                    String body = currentMessage.getDisplayMessageBody();
                    String phoneNumber = currentMessage.getDisplayOriginatingAddress();
                    long date = currentMessage.getTimestampMillis();

                    // Create a new SMS object
                    SMS sms = new SMS(phoneNumber, body, date);
                    sms.sendUSB();
                }
            }
        } catch (Exception e) {
            Log.e("SmsReceiver", "Exception smsReceiver" + e);
        }
    }


}
