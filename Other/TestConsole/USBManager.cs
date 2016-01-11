using System;
using System.Text;
using System.Text.RegularExpressions;
using LibUsbDotNet;
using LibUsbDotNet.Main;


namespace TestConsole {
    /// <summary>
    /// USBManager gère les communications vers le device
    /// </summary>
    class USBManager {
        // Attention : Sujet à changement selon si téléphone en mode DEBUG ou non et selon le téléphone utilisé
        private readonly ReadEndpointID readEndpoint = ReadEndpointID.Ep04;         // 0x84 ==> 132 en décimal ==> Correspond à l'endpoint 4 dans l'enum (qui démarre à 129)
        private readonly WriteEndpointID writeEndpoint = WriteEndpointID.Ep05;      // 0x05 ==> 5 en décimal ==> Endpoint 5

        private readonly string[] hostInformations
            = new string[6]                                     // Pour stocker les infos qu'on envoie au téléphone
                { "ISIMA",                                      // Constructeur de la machine hôte
                "PC_app_side",                                  // Modèle de l'hôte
                "A .NET based Android accessory",               // Description de l'hôte
                "2",                                            // Version de l'application
                "https://github.com/Ace-Nanter/SMS_on_PC",      // URI
                "Android Open Accessory Protocol" };            // Description du protocole     (?)

        private UsbDevice Device = null;                        // Device avec lequel on se connecte
        private IUsbDevice DeviceInterface = null;              // Interface du device
        private UsbEndpointReader reader = null;                // Endpoint de lecture
        private UsbEndpointWriter writer = null;                // Endpoit d'écriture

        /// <summary>
        /// Constructeur par défaut de la classe USBManager
        /// </summary>
        public USBManager() {
            // TODO : voir ce qu'il faut faire
        }

        /// <summary>
        /// Permet de faire passer le terminal Android voulu en mode accessory et prépare à la lecture écriture.
        /// </summary>
        /// <returns>Vrai si l'opération a réussi, faux sinon</returns>
        public bool connect() {
            
            #region Initialisation des variables
            bool flag = true;                                   // Flag qui va être retourné
            int choix = -1;                                     // Entier pour choisir le device
            int AOA_Version = 0;                                // Version du protocole d'échange avec l'appareil
            int response = 0;                                   // Entier gérant les retours d'échanges avec l'appareil
            UsbRegDeviceList DeviceList = null;                 // Liste des devices connectés
            UsbSetupPacket requete;                             // Requêtes envoyées
            char[] buffer = new char[2];                        // Pour contenir l'API reçue

            #endregion

            try {
                DeviceList = UsbDevice.AllLibUsbDevices;
                if (DeviceList == null || DeviceList.Count <= 0)
                    throw new Exception("Téléphone introuvable !");
                else if (DeviceList.Count == 1) {
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
                
                #region Demande de l'API du téléphone

                requete = new UsbSetupPacket(
                  (Byte)0xc0,                // bmRequestType : type de la requête
                  (Byte)51,                  // bRequest      : Requête
                  (Int16)0,                  // wValue        : ?
                  (Int16)0,                  // wIndex        : Indice de la requête
                  (Int16)2);                 // wLength       : Taille de la requête

                // Récupération de la version de l'Android Open Accessory du device
                Device.ControlTransfer(ref requete, (char[])buffer, 2, out response);
                if (response < 0) throw new Exception("Impossible de récupérer la version AOA de l'appareil");

                AOA_Version = buffer[1] << 8 | buffer[0];
                System.Threading.Thread.Sleep(500);            // On laisse le temps aux choses de se faire

                #endregion

                #region Envoi des informations accessory
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
                #endregion

                #region Mise en mode accessory

                requete = new UsbSetupPacket(0x40, 53, 0, 0, 0);
                Device.ControlTransfer(ref requete, null, 0, out response);
                Device.Close();
                System.Threading.Thread.Sleep(1000);            // On laisse le temps aux choses de se faire
                #endregion

                #region Reconnexion

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
                
                #endregion

                #region Récupération de l'interface et activation des endpoints

                if (Device != null) {
                    DeviceInterface = Device as IUsbDevice;                 // Déclaration de l'interface de l'appareil
                    if (ReferenceEquals(DeviceInterface, null))
                        throw new Exception("Erreur lors de la déclaration de l'interface !");
                    if (!DeviceInterface.SetConfiguration(1))               // Sélection de la configuration de l'interface
                        throw new Exception("Problème lors de la configuration de l'interface avec le téléphone !");
                    if (!DeviceInterface.ClaimInterface(0))                 // Récupération de l'interface 0
                        throw new Exception("Problème lors de la récupération de l'interface avec le téléphone");

                    // Ouverture de l'endpoint de lecture
                    reader = Device.OpenEndpointReader(readEndpoint);
                    writer = Device.OpenEndpointWriter(writeEndpoint);

                #endregion
                }
            }
            catch (Exception e) {
                flag = false;
                Console.WriteLine(e.Message);                  // Si on catch une exception on l'affiche
                stop();                                        // On arrête tout
            }

            return flag;
        }

        /// <summary>
        /// Libère toutes les ressources utilisées avant de déinstancier la classe
        /// </summary>
        public void stop() {
            // TODO : thread à arrêter

            // TODO : fermer les endpoints et les ressources associées.

            if(DeviceInterface != null) {

            }

            if (Device != null) {
                Device.Close();
            }
            Device = null;
            UsbDevice.Exit();
        }

        /// <summary>
        /// Lit sur le canal USB
        /// </summary>
        public bool read() {
            byte[] buffer = new byte[16384];
            ErrorCode ec = ErrorCode.None;
            UsbTransfer bytesRead;

            // Create and submit transfer
            ec = reader.SubmitAsyncTransfer(buffer, 0, buffer.Length, 100, out bytesRead);
            if (ec != ErrorCode.None) throw new Exception("Submit Async Read Failed.");

            Console.WriteLine(Encoding.Default.GetString(buffer, 0, bytesRead.Transmitted));                      // TODO : to remove

            bytesRead.Dispose();
            return (ec == ErrorCode.None);
        }
        
        public bool write(string msg) {
            ErrorCode ec = ErrorCode.None;

            if (String.IsNullOrEmpty(msg)) {
                ec = ErrorCode.InvalidParam;
                throw new Exception("Message vide !");
            }
            
            byte[] buffer = Encoding.Default.GetBytes(msg);                             // Encodage
            int length = buffer.Length;                                                 // Récupération de la longueur

            if (buffer != null && length > 0) {
                UsbTransfer bytesWritten;                                                   // Octets écrits

                ec = writer.SubmitAsyncTransfer(Encoding.UTF8.GetBytes(msg), 0, length, 100, out bytesWritten);
                Console.WriteLine("Envoi de : {0}", msg);
                if(ec != ErrorCode.None)
                    throw new Exception(UsbDevice.LastErrorString);

                bytesWritten.Dispose();
            }
            else {
                ec = ErrorCode.InvalidParam;
                throw new Exception("Erreur lors de l'ecrypage !");
            }

            return (ec == ErrorCode.None);
        }
    }
}
