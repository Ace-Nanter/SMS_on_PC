using System;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using EntityLayer;

namespace UsbLayer {

    /// <summary>
    /// UsbManager gère les communications vers le device
    /// </summary>
    public class UsbManager {

        // Requirements for Singleton pattern
        private static UsbManager INSTANCE;
        private static readonly object padlock = new object();

        // Informations envoyées au téléphone
        private readonly string[] hostInformations = new string[6];

        private UsbInterface m_listener = null;                 // Listener to send events

        private UsbDevice m_device = null;                      // Device avec lequel on se connecte
        private IUsbDevice DeviceInterface = null;              // Interface du device
        private Receiver m_receiver = null;                     // Classe qui permet de recevoir
        private Sender m_sender = null;                         // Classe qui permet d'envoyer
        private ReceiveHandler m_handler = null;                // Classe qui permet de gérer les messages reçus

        private bool m_connected = false;                      // Booléen pour indiquer l'état de la connexion

        public static UsbManager getInstance(UsbInterface listener)
        {
            if (listener == null && INSTANCE == null) {
                return null;
            }

            if (INSTANCE == null) {
                lock (padlock) {
                    if (INSTANCE == null) {
                        INSTANCE = new UsbManager(listener);
                    }
                }
            }
            return INSTANCE;
        }

        /// <summary>
        /// Constructeur par défaut de la classe UsbManager
        /// </summary>
        private UsbManager(UsbInterface listener) { // (Obligé de préciser pour éviter les confusions)
            #region Initialisation des attributs

            m_listener = listener;

            hostInformations = new string[6] {                  // Stocker les infos qu'on envoie au téléphone
                Properties.Resources.constructeur,
                Properties.Resources.modele,
                Properties.Resources.description,
                Properties.Resources.version,
                Properties.Resources.URI,
                Properties.Resources.protocol
            };

            #endregion
        }

        public static int USBLimit {
            get
            {
                int value;
                if (!int.TryParse(Properties.Resources.maxBuffer, out value))
                    value = 16384;

                return value;
            }
        }

        /// <summary>
        /// Get the Device used by the manager
        /// </summary>
        /// <returns>The device used by the manager.</returns>
        public UsbDevice getDevice() {
            return m_device;
        }

        /// <summary>
        /// Say if the manager is connected or not to the device.
        /// </summary>
        /// <returns>True if it is, otherwise false.</returns>
        public bool is_connected() {
            return m_connected;
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
            #region Choix du device

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

                #endregion
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
                    m_connected = true;
                    m_receiver = new Receiver(this);
                    m_sender = new Sender(this);
                    m_handler = new ReceiveHandler(this);
                    m_listener.hasBeenConnected();

                    #endregion
                }
            }
            catch (Exception e) {
                flag = false;
                m_connected = false;
                // TODO : un petit log ?
                Console.WriteLine(e);
                this.stop();                                        // On arrête tout
            }

            return flag;
        }

        /// <summary>
        /// Return the last string read by the reader
        /// </summary>
        /// <returns>The last string read</returns>
        public string getLastReceived() {
            return m_receiver.getLast();
        }

        public void popSending() {
            m_sender.pop();
        }

        /// <summary>
        /// Send a message by USB
        /// </summary>
        /// <param name="msg">Message to send</param>
        public void send(string msg) {
            m_sender.send(msg);
        }

        /// <summary>
        /// Send a SMS by USB
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool send(SMS sms) {

            int nbComs;
            int limit = 80;
            string buffer = "";

            if (string.IsNullOrEmpty(sms.Body)) {
                throw new Exception("Empty Message Body !");
            }

            if (sms.Body.Length < 80) {
                send("SMSHEADER:" + sms.ID + ":" + sms.Contact.Num + ":1");
                send("SMSBODY:1:" + sms.Body);
            }
            else {
                nbComs = (sms.Body.Length + limit - 1) / limit;
                buffer = "SMSHEADER:" + sms.ID + ":" + sms.Contact.Num + ":" + nbComs;
                send(buffer);

                int com = 1;
                for (int i = 0; i < sms.Body.Length; i += limit) {
                    buffer = "SMSBODY:" + com + ":";
                    buffer += sms.Body.Substring(i, Math.Min(limit, sms.Body.Length - i));
                    com++;

                    // TODO : to remove
                    Console.WriteLine("Envoi de {0}", buffer);

                    send(buffer);
                }
            }

            sms.Date = DateTime.Now;                  // The message is sent now

            return true;
        }

        /// <summary>
        /// Libère toutes les ressources utilisées avant de déinstancier la classe
        /// </summary>
        public void stop() {

            if(m_handler != null) {
                m_handler.stop();
            }

            if (m_sender != null && m_sender.active()) {
                if (m_sender.stop()) {                                      // Arrêt du Sender
                                                                            // TODO : communiquer avec le display pour indiquer qu'il restait des messages à envoyer
                }
            }

            if(m_receiver != null && m_receiver.active()) {                 // Arrêt du Receiver
                m_receiver.stop();
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

        /// <summary>
        /// Appelle la méthode hasBeenConnected du listener
        /// </summary>
        public void hasBeenConnected() {
            m_connected = true;
            m_listener.hasBeenConnected();
        }

        /// <summary>
        /// Appelle la méthode hasRead du listener
        /// </summary>
        /// <param name="msg"></param>
        public void hasRead(string msg) {
            m_listener.hasRead(msg);
        }

        /// <summary>
        /// Appelle la méthode hasBeenStopped du listener
        /// </summary>
        public void hasBeenStopped() {
            m_listener.hasBeenStopped();
        }
    }
}
