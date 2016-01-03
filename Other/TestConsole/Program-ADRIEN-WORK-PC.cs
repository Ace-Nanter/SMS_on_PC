﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace TestConsole {
    public class Synchroniser {

        public static int Init() {
            UsbRegDeviceList test = UsbDevice.AllDevices;
            for(int i = 0; i < test.Count; i++) {
                Console.WriteLine("Device : {0}", test[i].FullName);
            }

            return UsbDevice.AllDevices.Count;

        }
        /// <summary>
        /// Permet la connexion à un smartphone
        /// </summary>
        /// <returns>Retourne le device connecté en mode accessoire</returns>
        public static UsbDevice Connexion() {
            /*----------------------- Initialisation des variables ---------------------*/
            int AOA_Version = 0;                                // Version du protocole d'échange avec l'appareil
            int response = 0;                                   // Entier gérant les retours d'échanges avec l'appareil
            UsbDeviceFinder Smartphone = new UsbDeviceFinder("CB511Z2CFL");       // Numéro de série tel Adrien
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

                Console.WriteLine("Identification Accessory OK");                                 // TODO : remove
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
            UsbDevice Device = null;                        // Initialisation à null
            ErrorCode ec = ErrorCode.None;


            Console.WriteLine("Nombre de devices connectés : {0}", Init());

            try {
                Device = Connexion();                       // Mise du téléphone en mode accessory
            }
            catch (Exception e) {
                Console.WriteLine("Erreur, l'exception {1} a été levée !", e);
                Device = null;
            }

            if (Device != null) {
                Console.WriteLine("Passage en mode accessory OK");
                Console.Read();                                         // Attente avant lancement de la suite
                IUsbDevice DeviceInterface = Device as IUsbDevice;      // Déclaration de l'interface de l'appareil
                if (!ReferenceEquals(DeviceInterface, null)) {
                    DeviceInterface.SetConfiguration(1);                // Sélection de la configuration de l'interface
                    DeviceInterface.ClaimInterface(0);                  // Récupération de l'interface
                }

                // open read endpoint 1.
                //UsbEndpointReader reader = Device.OpenEndpointReader(ReadEndpointID.Ep01);

                // open write endpoint 1.
                UsbEndpointWriter writer = Device.OpenEndpointWriter(WriteEndpointID.Ep01);

                // Remove the exepath/startup filename text from the begining of the CommandLine.

                
                // Remove the exepath/startup filename text from the begining of the CommandLine.
                string message = "Coucou"; 

                if (!String.IsNullOrEmpty(message)) {
                    int bytesWritten;


                    // Envoi de la longueur
                    Console.WriteLine("Tentative d'envoi de : {0}", message.Length.ToString());
                    ec = writer.Write(Encoding.Default.GetBytes(message.Length.ToString()), 2000, out bytesWritten);
                    if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);

                    // Envoi de la chaîne en elle-même
                    Console.WriteLine("Tentative d'envoi de : {0}", message);
                    ec = writer.Write(Encoding.Default.GetBytes(message), 2000, out bytesWritten);
                    if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);

                    Console.WriteLine("Envois effectués");
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
}