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

        public void AddConversation(Contact con)
        {
            bdd.AddConversation(con);
        }
        public void AddMessageToConv(SMS sms, Conversation conv)
        {
            bdd.AddMessageToConv(sms, conv);
        }

        public void saveConversations(String FileName)
        {
            bdd.saveConversations(FileName);
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

        public void removeContact(Contact con)
        {
            bdd.removeContact(con);
        }

        public void saveContacts(String FileName)
        {
            bdd.saveContacts(FileName);
        }

        #endregion
    }
}
