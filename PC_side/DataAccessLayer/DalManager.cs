using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DalManager
    {
        private static DalManager INSTANCE;
        private static readonly object padlock = new object();
        IBridge bdd;

        public static DalManager Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    lock (padlock)
                    {
                        if (INSTANCE == null)
                        {
                            INSTANCE = new DalManager();
                        }
                    }
                }
                return INSTANCE;
            }
        }

        private DalManager()
        {
            bdd = new StubDataBase();
        }

        public List<Conversation> getConversations()
        {
            return bdd.getConversations();
        }

        public Conversation getConversationsFromContact(String contact)
        {
            return bdd.getConversationsFromContact(contact);
        }

        public void AddConversation(Contact con)
        {
            bdd.AddConversation(con);
        }

        public void AddMessageToConv(SMS sms, Conversation conv)
        {
            bdd.AddMessageToConv(sms, conv);
        }

        public Contact getContactFromString(String contact)
        {
            return bdd.getContactFromString(contact);
        }
        public List<Contact> getContacts()
        {
            return bdd.getContacts();
        }

        public void addContact(Contact con)
        {
            bdd.addContact(con);
        }

        public void removeContact(Contact con)
        {
            bdd.removeContact(con);
        }
    }
}
