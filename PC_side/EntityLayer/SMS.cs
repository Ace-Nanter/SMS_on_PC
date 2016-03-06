using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    /// <summary>
    /// SMS class. Contains all the informations of a short Message Text
    /// </summary>
    public class SMS {
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
        public SMS(int id,string body,DateTime date, Contact contact,bool received,bool notified) : this()
        {
            m_id = id;
            m_date = date;
            m_received = received;
            m_notified = notified;
            m_body = body;
            m_contact = contact;
        }

        public SMS(string body, Contact contact) : this() {
            m_received = false;
            m_notified = false;
            m_body = body;
            m_contact = contact;
        }

        public int ID
        {
            get
            {
                return m_id;
            }
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
            set
            {
                m_date = value;
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
    }
}
