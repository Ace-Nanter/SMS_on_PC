package fr.isima.sms_on_pc.SMS;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.telephony.SmsManager;
import android.telephony.SmsMessage;
import android.util.Log;

import fr.isima.sms_on_pc.USB.LinkManager;

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

                    int nbComs;
                    final int limit = 80;
                    String buffer;

                    // Get the link manager
                    LinkManager manager;

                    if(body == null || body == "") {
                        Log.d(SMS.class.getSimpleName(), "Empty message !");
                    }
                    else {
                        // Get the manager
                        try {
                            manager = LinkManager.getInstance(null, null);
                        } catch (Exception e) {
                            Log.d(SMS.class.getSimpleName(), "An exception occurred : " + e);
                            manager = null;
                        }

                        if (manager != null) {          // If manager is OK

                            Log.d(Incoming.class.getSimpleName(), "Récupération de l'instance ok !");

                            // Check if the message is multiparted or not
                            if (body.length() < 80) {

                                // TODO : to remove
                                try {
                                    Log.d(Incoming.class.getSimpleName(), "Une page");

                                    // Send the header
                                    buffer = "SMSHEADER:" + phoneNumber;
                                    //buffer += ":" + m_date.getTime().toString();
                                    buffer += ":" + 1;

                                    Log.d(Incoming.class.getSimpleName(), "header : " + buffer);

                                    manager.send(buffer);


                                    // Send the body
                                    manager.send("SMSBODY:1:" + body);

                                    Log.d(Incoming.class.getSimpleName(), "body : " + body);

                                } catch (Exception e) {
                                    Log.d(Incoming.class.getSimpleName(), "Problème : " + e);
                                }



                            } else {
                                Log.d(Incoming.class.getSimpleName(), "Plus");

                                // Send the header
                                nbComs = (body.length() + limit - 1) / limit;
                                buffer = "SMSHEADER:" + phoneNumber + ":";
                                //buffer += ":" + m_date.getTime().toString();
                                buffer += ":" + nbComs;

                                Log.d(SMS.class.getSimpleName(), "header : " + buffer);

                                manager.send(buffer);

                                // Send the body
                                int com = 1;
                                for (int k = 0; k < body.length(); k += limit) {
                                    buffer = "SMSBODY:" + com + ":";
                                    buffer += body.substring(k, k + Math.min(limit, body.length() - k));
                                    com++;

                                    Log.d(SMS.class.getSimpleName(), "body : " + buffer);

                                    manager.send(buffer);
                                }
                            }
                        } else {
                            Log.d(SMS.class.getSimpleName(), "Manager problem !");
                        }
                    }
                    //sms.sendUSB();
                }
            }
        } catch (Exception e) {
            Log.e("SmsReceiver", "Exception smsReceiver" + e);
        }
    }


}
