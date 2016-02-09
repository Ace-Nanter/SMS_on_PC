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
        private static readonly int PAGE_SIZE = 140;

        private string m_body;                      // Body of the message
        private int m_nbPages;                      // Number of pages contained in the SMS
        private DateTime m_date;                    // Date it was sent/received
        private Contact m_contact;                  // Contact associated to the message
        private bool m_received;                    // True if the message was received, wrong if it was sent
        private bool m_notified;                    // Was the message received ?

        public string Body
        {
            get
            {
                return m_body;
            }

            set
            {
                m_body = value;
                m_nbPages = m_body.Length / PAGE_SIZE;
            }
        }

        public int NbPages
        {
            get
            {
                return m_nbPages;
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

        internal Contact Contact
        {
            get { return m_contact; }
            set { m_contact = value; }
        }

        /// <summary>
        /// Envoi d'un SMS par le biais de l'USB
        /// </summary>
        /// <returns></returns>
        public bool send(UsbManager manager) {
            string buffer = "";

            buffer = "SMSHEADER:" + Contact.Num + ":" + NbPages;

            for(int i = 0; i < NbPages; i++) {
                buffer = "SMSBODY:" + i + ":";
                buffer += Body.Substring(i * PAGE_SIZE, PAGE_SIZE);
                manager.send(buffer);
            }

            return true;
        }
    }
}
