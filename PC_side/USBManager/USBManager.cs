using System;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;


namespace USBLayer {

    /// <summary>
    /// USBManager gère les communications vers le device
    /// </summary>
    public class USBManager {
        // Attention : Sujet à changement selon si téléphone en mode DEBUG ou non et selon le téléphone utilisé

        private readonly string[] hostInformations = new string[6];

        private UsbDevice m_device = null;                      // Device avec lequel on se connecte
        private IUsbDevice DeviceInterface = null;              // Interface du device
        private Receiver m_receiver = null;                     // Classe qui permet de recevoir
        private Sender m_sender = null;                         // Classe qui permet d'envoyer

        private bool is_connected = false;                      // Booléen pour indiquer l'état de la connexion

        /// <summary>
        /// Constructeur par défaut de la classe USBManager
        /// </summary>
        public USBManager(/*USBInterface listener*/) {
            #region Initialisation des attributs

            hostInformations = new string[6] {                  // Stocker les infos qu'on envoie au téléphone
                Properties.Resources.constructeur,
                Properties.Resources.modele,
                Properties.Resources.description,
                Properties.Resources.version,
                Properties.Resources.URI,
                Properties.Resources.protocole
            };

            #endregion
        }

        public UsbDevice getDevice() {
            return m_device;
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
                    // TODO : ici changer les protocoles pour afficher la liste
                    Console.WriteLine("Appareils disponibles :");
                    for (int k = 0; k < DeviceList.Count; k++)
                        Console.WriteLine("{0} - {1}", k, DeviceList[k].FullName);

                    Console.WriteLine("Veuillez choisir un appareil auquel se connecter");
                    choix = int.Parse(Console.ReadLine());
                }

                if (!DeviceList[choix].Open(out m_device))
                    throw new Exception("Impossible de se connecter au téléphone !");

                #region Demande de l'API du téléphone

                requete = new UsbSetupPacket(
                  (Byte)0xc0,                // bmRequestType : type de la requête
                  (Byte)51,                  // bRequest      : Requête
                  (Int16)0,                  // wValue        : ?
                  (Int16)0,                  // wIndex        : Indice de la requête
                  (Int16)2);                 // wLength       : Taille de la requête

                // Récupération de la version de l'Android Open Accessory du device
                m_device.ControlTransfer(ref requete, (char[])buffer, 2, out response);
                if (response < 0) throw new Exception("Impossible de récupérer la version AOA de l'appareil");

                AOA_Version = buffer[1] << 8 | buffer[0];
                System.Threading.Thread.Sleep(500);            // On laisse le temps aux choses de se faire

                #endregion

                #region Envoi des informations accessory
                int i = 0;
                foreach (string info in hostInformations) {
                    requete = new UsbSetupPacket(0x40, 52, 0, (Int16)i, (Int16)info.Length);
                    m_device.ControlTransfer(ref requete,
                        Encoding.UTF8.GetBytes(info),
                        info.Length,
                        out response);
                    if (response < 0) throw new Exception("Erreur lors de l'identification auprès de l'appareil !");
                    i++;
                }
                #endregion

                #region Mise en mode accessory

                requete = new UsbSetupPacket(0x40, 53, 0, 0, 0);
                m_device.ControlTransfer(ref requete, null, 0, out response);
                m_device.Close();
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
                    if (DeviceList[choix].Open(out m_device))
                        break;

                    System.Threading.Thread.Sleep(500);         // On laisse un peu de temps à chaque fois
                    attempts++;
                }

                if (m_device == null || (!m_device.Open()))
                    throw new Exception("Impossible de récuperer le device en mode Accessory !");

                #endregion

                #region Récupération de l'interface et activation des endpoints

                if (m_device != null) {
                    DeviceInterface = m_device as IUsbDevice;                 // Déclaration de l'interface de l'appareil
                    if (ReferenceEquals(DeviceInterface, null))
                        throw new Exception("Erreur lors de la déclaration de l'interface !");
                    if (!DeviceInterface.SetConfiguration(1))               // Sélection de la configuration de l'interface
                        throw new Exception("Problème lors de la configuration de l'interface avec le téléphone !");
                    if (!DeviceInterface.ClaimInterface(0))                 // Récupération de l'interface 0
                        throw new Exception("Problème lors de la récupération de l'interface avec le téléphone");

                    // Activation de la lecture/écriture
                    //receiver = new Receiver(m_device);
                    m_sender = new Sender(this);

                    #endregion
                }
            }
            catch (Exception e) {
                flag = false;
                is_connected = false;
                // TODO : un petit log ?
                this.stop();                                        // On arrête tout
            }

            return flag;
        }

        /// <summary>
        /// Libère toutes les ressources utilisées avant de déinstancier la classe
        /// </summary>
        public void stop() {
            //m_receiver.stop();                                        // Arrêt du Receiver
            if (m_sender.stop()) {                                    // Arrêt du Sender
                // TODO : communiquer avec le display pour indiquer qu'il restait des messages à envoyer
            }

            m_receiver = null;                                        // Désinstanciation du lecteur
            m_sender = null;

            // TODO : fermer les endpoints et les ressources associées.

            if (DeviceInterface != null) {
                DeviceInterface.ReleaseInterface(0);
            }

            if (m_device != null) {
                m_device.Close();
            }
            m_device = null;
            UsbDevice.Exit();
        }
    }
}
