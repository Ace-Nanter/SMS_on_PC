using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ConversationManager
    {
        private static DataAccessLayer.DalManager bdd = DataAccessLayer.DalManager.Instance;

        #region Conversations management
        public List<Conversation> getConversations()
        {
            return bdd.getConversations();
        }

        public Conversation getConversationsFromContact(String contact)
        {
            return bdd.getConversationsFromContact(contact);
        }
        #endregion

        #region Contact management
        public List<Contact> getContacts()
        {
            return bdd.getContacts();
        }

        public Contact getContactFromString(String contact)
        {
            return bdd.getContactFromString(contact);
        }

        public void addContact(Contact con)
        {
            bdd.addContact(con);
        }

            #endregion
    }
}
