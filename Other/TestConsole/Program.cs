using System;
using System.Text;
using System.Text.RegularExpressions;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace TestConsole {
    public class Synchroniser {

        /// <summary>
        /// Permet la connexion à un smartphone
        /// </summary>
        /// <returns>Retourne le device connecté en mode accessoire</returns>
        public static UsbDevice Connexion() {
        /*----------------------- Initialisation des variables ---------------------*/
            int AOA_Version = 0;                                // Version du protocole d'échange avec l'appareil
            int response = 0;                                   // Entier gérant les retours d'échanges avec l'appareil
            UsbDeviceFinder Smartphone = new UsbDeviceFinder("YT910PKBQU");       // Numéro de série tel Adrien
            UsbDevice Device = null;
            UsbSetupPacket setupPacket;                         // Contenu des paquets envoyés
            char[] IObuffer = new char[2];                      // Pour contenir l'API reçue
            string[] hostInformations = new string[6]           // Pour stocker les infos qu'on envoie au téléphone
                { "Gigabyte",                                   // Constructeur de la machine hôte
                "MyPC",                                         // Modèle de l'hôte
                "Adrien_PC",                                    // Description de l'hôte
                "2",                                            // Version                      (?)
                "https://github.com/Ace-Nanter/SMS_on_PC",      // URI                          (?)
                "Android Open Accessory Protocol" };            // Description du protocole     (?)
        /*-----------------------------------------------------------------------------*/

            try {
                // Find and open the usb device
                Device = UsbDevice.OpenUsbDevice(Smartphone);

                // If the device is open and ready
                if (Device == null) throw new Exception("Appareil introuvable !");

         /*--------------------- Demande de l'API du téléphone ---------------------*/
                setupPacket = new UsbSetupPacket(
                  (Byte)0xc0,                // bmRequestType
                  (Byte)51,                  // bRequest
                  (Int16)0,                  // wValue
                  (Int16)0,                  // wIndex
                  (Int16)2);                 // wLength

                Device.ControlTransfer(ref setupPacket, (char[])IObuffer, 2, out response);     // On demande la version de l'AOA du device
                if (response < 0) throw new Exception("Impossible de récupérer la version AOA de l'appareil");

                AOA_Version = IObuffer[1] << 8 | IObuffer[0];

                Console.WriteLine("Version de Code du Device : " + AOA_Version);                // TODO : remove
                System.Threading.Thread.Sleep(1000);            // On laisse le temps aux choses de se faire
         /*-----------------------------------------------------------------------------*/
         /*---------------------- Envoi des informations accessory ---------------------*/

         //TODO : voir pourquoi à l'arrivée il n'y a que la première lettre des informations
                int i = 0;
                foreach (string info in hostInformations) {
                    setupPacket = new UsbSetupPacket(0x40, 52, 0, (Int16)i, (Int16)info.Length);

                    Device.ControlTransfer(ref setupPacket,
                        info.ToCharArray(),
                        info.Length,
                        out response);

                    if (response < 0) throw new Exception("Erreur lors de l'identification auprès de l'appareil !");

                    i++;
                }

                Console.WriteLine("Identification Accessory OK");                                      // TODO : remove
                                                                                                       /*-----------------------------------------------------------------------------*/
                                                                                                       /*---------------------------- Mise en mode accessory -------------------------*/
                setupPacket = new UsbSetupPacket(0x40, 53, 0, 0, 0);
                Device.ControlTransfer(ref setupPacket, null, 0, out response);
                Device.Close();

                /*--------------------------------- Reconnexion -------------------------------*/
                int attempts = 0;
                UsbDeviceFinder Accessory = new UsbDeviceFinder(0x0FCE, 0x2D00);                   // Nouveau device en mode Acessory

                while (!Device.Open() && attempts < 5)       // On se donne 5 essais pour la reconnexion
                {
                    Device = UsbDevice.OpenUsbDevice(Accessory);
                    System.Threading.Thread.Sleep(500);       // On laisse un peu de temps à chaque fois
                    attempts++;
                }

                if (!Device.Open()) throw new Exception("Impossible de récuperer le device en mode Accessory !");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);              // Si on attrape une exception on l'affiche
                UsbDevice.Exit();                           // Sortie des fonctions USB
                Device = null;
            }

            return Device;
        }


        /// <summary>
        /// Fonction principale
        /// </summary>
        /// <param name="args">Rien à donner pour le moment</param>
        public static void Main(string[] args) {
            UsbDevice Device = null;                    // Initialisation à null
            ErrorCode ec = ErrorCode.None;

            try {
                Device = Connexion();
            }
            catch {
                Device = null;
            }

            if (Device == null) throw new Exception("Problème lors de la connexion au Device !");

            // If this is a "whole" usb device (libusb-win32, linux libusb)
            // it will have an IUsbDevice interface. If not (WinUSB) the 
            // variable will be null indicating this is an interface of a 
            // device.
            IUsbDevice DeviceInterface = Device as IUsbDevice;
            if (!ReferenceEquals(DeviceInterface, null)) {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                DeviceInterface.SetConfiguration(1);

                // Claim interface #0.
                DeviceInterface.ClaimInterface(0);
            }

            // open read endpoint 1.
            UsbEndpointReader reader = Device.OpenEndpointReader(ReadEndpointID.Ep01);

            // open write endpoint 1.
            //UsbEndpointWriter writer = Device.OpenEndpointWriter(WriteEndpointID.Ep01);

            // Remove the exepath/startup filename text from the begining of the CommandLine.

            byte[] readBuffer = new byte[1024];             // Buffer pour récupérer les caractères
            while (ec == ErrorCode.None)                    // Tant que le code d'erreur reste à null
            {
                int bytesRead;

                // If the device hasn't sent data in the last 100 milliseconds,
                // a timeout error (ec = IoTimedOut) will occur. 
                ec = reader.Read(readBuffer, 100, out bytesRead);

                if (bytesRead == 0) throw new Exception("No more bytes!");              // Exception si Buffer vide alors qu'on est censé avoir lu

                // Write that output to the console.
                Console.Write(Encoding.Default.GetString(readBuffer, 0, bytesRead));
            }
            if (Device != null) {
                if (Device.IsOpen) {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = Device as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null)) {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(0);
                    }
                    Device.Close();
                }
                Device = null;

                // Free usb resources
                UsbDevice.Exit();

            }

        }
    }
}