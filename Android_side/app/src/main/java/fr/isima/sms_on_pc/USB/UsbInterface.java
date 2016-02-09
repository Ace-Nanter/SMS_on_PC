package fr.isima.sms_on_pc.USB;

import java.util.EventListener;

/**
 * Created by Ace Nanter on 24/01/2016.
 */
public interface UsbInterface {
    void Connected(boolean is_connected);       // Connection established
    void hasRead(String s);                     // On a lu quelque chose
    void UsbStop();                             // Le Thread a été arrêté
}
