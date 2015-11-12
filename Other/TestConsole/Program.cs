using System;
using System.Text;
using System.Text.RegularExpressions;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace TestConsole
{
    internal class ReadWriteEventDriven
    {
        public static DateTime LastDataEventDate = DateTime.Now;
        public static UsbDevice MyUsbDevice;                                                // Device avec lequel on communique

        public static void Main(string[] args)
        {
            int devVersion, reponse = 0;   // Retour d'échanges avec l'appareil
            UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder("YT910PKBQU");      // Numéro de série tel Adrien
            UsbSetupPacket setupPacket;                     // "En-tête" des requêtes
            char[] IObuffer = new char[2];                  // Pour contenir l'API reçue
            string[] hostInformations = new string[6]       // Pour stocker les infos qu'on envoie au téléphone
                {"Gigabyte",                                // Manufacturer
                "MyPC",                                     // Model
                "Adrien_PC",                                // Description
                "1",                                        // Version (???)
                "https://github.com/Ace-Nanter/SMS_on_PC",  // URI
                "Android Open Accessory Protocol" };        // Description du protocole ?

            ErrorCode ec = ErrorCode.None;                  // Code d'erreur, sorte de flag
            
            try
            {
                // Find and open the usb device.
                MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

                // If the device is open and ready
                if (MyUsbDevice == null) throw new Exception("Device Not Found.");

                IUsbDevice AndroidInterface = MyUsbDevice as IUsbDevice;       // On prend une interface ?

                AndroidInterface.ClaimInterface(0);

                /*--------------------- Demande de l'API du téléphone ---------------------*/
                setupPacket = new UsbSetupPacket(
                (Byte)0xc0,                // bmRequestType
                (Byte)51,                  // bRequest
                (Int16)0,                  // wValue
                (Int16)0,                  // wIndex
                (Int16)2);                 // wLength

                MyUsbDevice.ControlTransfer(ref setupPacket, (char[])IObuffer, 2, out reponse);       // On demande la version du device
                if(reponse < 0)             // TODO : Exception à mettre en place
                {
                    Console.WriteLine("Problème lors de la demande de la version !");
                }
                devVersion = IObuffer[1] << 8 | IObuffer[0];
                Console.WriteLine("Version de Code du Device : " + devVersion);

                System.Threading.Thread.Sleep(1000);       // On laisse un peu de temps
                /*-----------------------------------------------------------------------------*/
                /*---------------------- Envoi des informations accessory ---------------------*/
                
                int i = 0;

                //TODO : voir pourquoi à l'arrivée il n'y a que la première lettre des informations
                while(reponse >= 0 && i < hostInformations.Length)
                {
                    setupPacket = new UsbSetupPacket(0x40, 52, 0, (Int16) i, (Int16) hostInformations[i].Length);
                    
                    MyUsbDevice.ControlTransfer(ref setupPacket,
                        hostInformations[i].ToCharArray(),
                        hostInformations[i].Length,
                        out reponse);

                    Console.WriteLine("Taille des chaîne de caractères :" + hostInformations[i].Length);

                    if (reponse < 0)
                        Console.WriteLine("Problème lors de l'identification Accessory");
                    i++;
                }

                Console.WriteLine("Identification Accessory OK");
                /*-----------------------------------------------------------------------------*/

                /*---------------------------- Mise en mode accessory -------------------------*/
                setupPacket = new UsbSetupPacket(0x40, 53, 0, 0, 0);
                MyUsbDevice.ControlTransfer(ref setupPacket, null, 0, out reponse);
                AndroidInterface.ReleaseInterface(0);
                MyUsbDevice.Close();


                // RECONNEXION
                i = 0;
                UsbDeviceFinder MyUsbFinder2 = new UsbDeviceFinder(0x0FCE, 0x2D00);         // Device en mode Acessory
                if (MyUsbDevice.IsOpen) Console.WriteLine("Cherche pas il est ouvert le device");
                while (i < 5)                // 5 essais
                {
                    MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder2);
                    System.Threading.Thread.Sleep(1000);       // On laisse un peu de temps
                    i++;
                    Console.WriteLine("Je suis dans la boucle de reconnexion");
                }
                

                

                //MyUsbDevice.Close();
                //MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);
                //MyUsbDevice.ActiveEndpoints.Clear();
                
                Console.WriteLine("Success bordel !");

                // MyUsbDevice.ControlTransfer(handle, 0x40, 52, 0, 0, (char*)manufacturer, strlen(manufacturer), 0);

                /*if (lenghtTransferred < 2)                 // Tout n'a pas été transféré
                {
                    Console.WriteLine("Problème lors de la demande accessory !");
                }*/



                System.Threading.Thread.Sleep(100000);       // On laisse un peu de temps
                UsbDevice.Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }
        }
    }
}