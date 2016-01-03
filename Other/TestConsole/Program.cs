/*-----------------------------------------------------------------------*/
/* AVANT TOUT USAGE DU PROGRAMME UTILISER ZADIG SUR LE TELEPHONE UTILISE */
/*              METTRE EN MODE WINUSB                                    */
/*-----------------------------------------------------------------------*/

using System;
using System.Text;
using System.Text.RegularExpressions;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace TestConsole {
    public class Synchroniser {

        const Boolean DEBUG = true;                      // Lance le mode débug

        /// <summary>
        /// Etablit la connexion avec le téléphone
        /// </summary>
        /// <returns>Retourne le device connecté en mode accessoire</returns>
        public static UsbDevice Connexion() {
            /*----------------------- Initialisation des variables ---------------------*/
            int choix = -1;
            int AOA_Version = 0;                                // Version du protocole d'échange avec l'appareil
            int response = 0;                                   // Entier gérant les retours d'échanges avec l'appareil
            UsbRegDeviceList DeviceList = null;                 // Liste des devices connectés
            UsbDevice Device = null;                            // Device non configuré
            UsbSetupPacket requete;                             // Requêtes envoyées
            char[] IObuffer = new char[2];                      // Pour contenir l'API reçue
            string[] hostInformations = new string[6]           // Pour stocker les infos qu'on envoie au téléphone
                { "ISIMA",                                      // Constructeur de la machine hôte
                "PC_app_side",                                  // Modèle de l'hôte
                "A .NET based Android accessory",               // Description de l'hôte
                "2",                                            // Version de l'application
                "https://github.com/Ace-Nanter/SMS_on_PC",      // URI
                "Android Open Accessory Protocol" };            // Description du protocole     (?)
            /*-----------------------------------------------------------------------------*/
            try {
                DeviceList = UsbDevice.AllLibUsbDevices;
                if (DeviceList == null || DeviceList.Count <= 0)
                    throw new Exception("Téléphone introuvable !");
                else if(DeviceList.Count == 1) {
                    choix = 0;
                }
                else {
                    Console.WriteLine("Appareils disponibles :");
                    for (int k = 0; k < DeviceList.Count; k++)
                        Console.WriteLine("{0} - {1}", k, DeviceList[k].FullName);

                    Console.WriteLine("Veuillez choisir un appareil auquel se connecter");
                    choix = int.Parse(Console.ReadLine());
                }
                
                if (!DeviceList[choix].Open(out Device))
                    throw new Exception("Impossible de se connecter au téléphone !");
                /*--------------------- Demande de l'API du téléphone ---------------------*/
                requete = new UsbSetupPacket(
                  (Byte)0xc0,                // bmRequestType : type de la requête
                  (Byte)51,                  // bRequest      : Requête
                  (Int16)0,                  // wValue        : ?
                  (Int16)0,                  // wIndex        : Indice de la requête
                  (Int16)2);                 // wLength       : Taille de la requête

                // Récupération de la version de l'Android Open Accessory du device
                Device.ControlTransfer(ref requete, (char[])IObuffer, 2, out response);
                if (response < 0) throw new Exception("Impossible de récupérer la version AOA de l'appareil");

                AOA_Version = IObuffer[1] << 8 | IObuffer[0];
                System.Threading.Thread.Sleep(500);            // On laisse le temps aux choses de se faire
                /*-----------------------------------------------------------------------------*/

                /*---------------------- Envoi des informations accessory ---------------------*/
                int i = 0;
                foreach (string info in hostInformations) {
                    requete = new UsbSetupPacket(0x40, 52, 0, (Int16)i, (Int16)info.Length);
                    Device.ControlTransfer(ref requete,
                        Encoding.UTF8.GetBytes(info),
                        info.Length,
                        out response);
                    if (response < 0) throw new Exception("Erreur lors de l'identification auprès de l'appareil !");
                    i++;
                }

                if (DEBUG) Console.WriteLine("Envoi des informations  Accessory OK");
                /*-----------------------------------------------------------------------------*/
                /*---------------------------- Mise en mode accessory -------------------------*/
                requete = new UsbSetupPacket(0x40, 53, 0, 0, 0);
                Device.ControlTransfer(ref requete, null, 0, out response);
                Device.Close();

                System.Threading.Thread.Sleep(2000);            // On laisse le temps aux choses de se faire
                /*--------------------------------- Reconnexion -------------------------------*/
                DeviceList = UsbDevice.AllLibUsbDevices;
                if (DeviceList == null || DeviceList.Count <= 0)
                    throw new Exception("Téléphone introuvable !");
                else if (DeviceList.Count == 1)
                    choix = 0;
                else {
                    Console.WriteLine("Choississez l'appareil en mode accessory :");
                    for (int k = 0; k < DeviceList.Count; k++)
                        Console.WriteLine("{0} - {1}", k, DeviceList[k].FullName);

                    Console.WriteLine("Veuillez choisir un appareil auquel se connecter");
                    choix = int.Parse(Console.ReadLine());
                }
                
                int attempts = 0;
                while (attempts < 5)                            // On se reconnecte 5 fois (par précaution)
                {
                    if (DeviceList[choix].Open(out Device))
                        break;

                    System.Threading.Thread.Sleep(500);         // On laisse un peu de temps à chaque fois
                    attempts++;
                }

                if (Device == null || (!Device.Open()))
                    throw new Exception("Impossible de récuperer le device en mode Accessory !");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);                  // Si on attrape une exception on l'affiche
                if (Device != null) Device.Close();              // On ferme la connexion à l'appareil
                Device = null;
                UsbDevice.Exit();
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

            try {
                Device = Connexion();                       // Mise du téléphone en mode accessory
            }
            catch (Exception e) {
                Console.WriteLine("Erreur lors de l'initialisation de la connexion avec le téléphone : {0}", e);
                Device = null;
            }

            if (Device != null) {
                try {
                    if (DEBUG) Console.WriteLine("Passage en mode accessory OK");
                    IUsbDevice DeviceInterface = Device as IUsbDevice;      // Déclaration de l'interface de l'appareil
                    if (ReferenceEquals(DeviceInterface, null))
                        throw new Exception("Erreur lors de la déclaration de l'interface !");
                    if (!DeviceInterface.SetConfiguration(1))               // Sélection de la configuration de l'interface
                        throw new Exception("Problème lors de la configuration de l'interface avec le téléphone !");
                    if (!DeviceInterface.ClaimInterface(0))                 // Récupération de l'interface
                        throw new Exception("Problème lors de la récupération de l'interface avec le téléphone");

                    //TODO : to remove
                    Console.Read();                                         // Attente avant lancement de la suite
                    

                    // open read endpoint 1.
                    //UsbEndpointReader reader = Device.OpenEndpointReader(ReadEndpointID.Ep01);

                    // Ouverture du premier endpoint d'écriture
                    UsbEndpointWriter writer = Device.OpenEndpointWriter(WriteEndpointID.Ep05);

                    // Message à envoyer
                    string message = "Coucou";

                    if (!String.IsNullOrEmpty(message)) {
                        UsbTransfer bytesWritten;

                        // Envoi de la longueur
                        Console.WriteLine("Tentative d'envoi de : {0}", message);

                        ec = writer.SubmitAsyncTransfer(Encoding.Default.GetBytes(message), 0, Encoding.Default.GetBytes(message).Length, 100, out bytesWritten);

                        //ec = writer.Write(Encoding.Default.GetBytes(message.Length.ToString()), 2000, out bytesWritten);
                        if (ec != ErrorCode.None) {
                            Console.WriteLine(ec);
                            throw new Exception(UsbDevice.LastErrorString);
                        }
                        /*
                        // Envoi de la chaîne en elle-même
                        Console.WriteLine("Tentative d'envoi de : {0}", message);
                        ec = writer.Write(Encoding.Default.GetBytes(message), 2000, out bytesWritten);
                        if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);
                        */
                        Console.WriteLine("Envois effectués");

                        bytesWritten.Dispose();
                    }
                }
                catch (Exception e) {
                    Console.WriteLine("Une exception a eu lieu : {0}", e);
                }
                Device.Close();
            }
            UsbDevice.Exit();
        }   // Fin Fonction main
    }       // Fin public class
}           // Fin namespace

/*
Deux problèmes : utilisation du mauvais endpoint : comment savoir lequel est le bon ?
Peut-être est-ce aussi du au fait que le thread de réception n'est pas encore ouvert
Null pointer exception sur Android trouver où quand comment pourquoi
Pour le souci des informations dont on ne reçoit que la première lettre : convertir la string en bits.
*/