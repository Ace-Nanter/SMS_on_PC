using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsbLayer;

namespace EntityLayer
{
    /// <summary>
    /// SMS class. Contains all the informations of a short Message Text
    /// </summary>
    public class SMS
    {
        public static int id_setter = 1;

        private static readonly int PAGE_SIZE = 140;

        private int m_id;                           // ID of the message
        private string m_body;                      // Body of the message
        private DateTime m_date;                    // Date it was sent/received
        private Contact m_contact;                  // Contact associated to the message
        private bool m_received;                    // True if the message was received, wrong if it was sent
        private bool m_notified;                    // Was the message received ?

        public SMS() {
            m_id = id_setter++;
            m_date = DateTime.Now;
        }

        /*
        public SMS(m_body, m_date, m_received) {

        }
        */

        public SMS(string body, Contact contact) : this() {
            m_received = false;
            m_notified = false;
            m_body = body;
            m_contact = contact;
        }

        public string Body
        {
            get
            {
                return m_body;
            }

            set
            {
                m_body = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return m_date;
            }
        }

        public bool Received
        {
            get { return m_received; }
            set { m_received = value; }
        }

        public bool Notified
        {
            get { return m_notified; }
            set { m_notified = value; }
        }

        public Contact Contact
        {
            get { return m_contact; }
            set { m_contact = value; }
        }

        /// <summary>
        /// Envoi d'un SMS par le biais de l'USB
        /// </summary>
        /// <returns></returns>
        public bool send(UsbManager manager) {

            int nbComs;
            int limit = 80;
            string buffer = "";

            if(string.IsNullOrEmpty(Body)) {
                throw new Exception("Empty Message Body !");
            }

            if(Body.Length < 80) {
                manager.send("SMSHEADER:" + m_id + ":" + Contact.Num + ":1");
                manager.send("SMSBODY:1:" + Body);
            }
            else {
                nbComs = (Body.Length + limit - 1) / limit;
                buffer = "SMSHEADER:" + m_id + ":" + Contact.Num + ":" + nbComs;
                manager.send(buffer);

                int com = 1;
                for (int i = 0; i < Body.Length; i+= limit) {
                    buffer = "SMSBODY:" + com  + ":";
                    buffer += Body.Substring(i, Math.Min(limit, Body.Length - i));
                    com++;

                    // TODO : to remove
                    Console.WriteLine("Envoi de {0}", buffer);

                    manager.send(buffer);
                }
            }

            m_date = DateTime.Now;                  // The message is sent now

            return true;
        }
    }
}
