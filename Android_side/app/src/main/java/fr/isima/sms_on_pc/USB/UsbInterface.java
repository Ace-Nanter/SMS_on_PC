package fr.isima.sms_on_pc.USB;

/**
 * Created by Ace Nanter on 24/01/2016.
 */
public interface UsbInterface {
    void hasRead(String s);                     // On a lu quelque chose
    void UsbStop();                             // Le Thread a été arrêté
}
