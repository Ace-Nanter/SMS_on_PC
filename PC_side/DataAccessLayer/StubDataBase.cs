using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer;

namespace DataAccessLayer
{
    class StubDataBase : IBridge
    {

        private static List<Conversation> conversation;

        private static List<Contact> contacts;

        public List<Conversation> getConversations()
        {
            if (conversation == null)
            {
                conversation = new List<Conversation>();

                Conversation conv1 = new Conversation(new Contact("John", "0607080910"));
                Conversation conv2 = new Conversation(new Contact("Jim", "0470315054"));

                List<SMS> msgc1 = new List<SMS>();
                msgc1.Add(new SMS("Coucou", conv1.Receiver));
                msgc1.Add(new SMS("Salut !", null));
                msgc1.Add(new SMS("ça va ?", conv1.Receiver));
                msgc1.Add(new SMS("Pas mal pour un test", null));
                msgc1.Add(new SMS("Comme tu vois", null));

                conv1.Messages = msgc1;

                List<SMS> msgc2 = new List<SMS>();
                msgc2.Add(new SMS("Yop", conv2.Receiver));
                msgc2.Add(new SMS("Salut !", null));

                conv2.Messages = msgc2;

                conversation.Add(conv1);
                conversation.Add(conv2);
            }
            return conversation;
        }

        public Conversation getConversationsFromContact(String contact)
        {
            foreach(Conversation c in conversation)
            {
                if (c.Receiver.Num.Equals(contact))
                    return c;
            }

            return null;
        }

        #region Contact manager
        public Contact getContactFromString(String contact)
        {

            foreach (Contact c in contacts)
            {
                if (c.Num.Equals(contact))
                    return c;
            }
            return null;
        }


        public List<Contact> getContacts()
        {
            if (contacts == null)
            {
                contacts = new List<Contact>();
                contacts.Add(new Contact("John", "0607080910"));
                contacts.Add(new Contact("Jim", "0470315054"));
            }
            contacts.Sort(delegate (Contact c1, Contact c2) { return c1.Nom.CompareTo(c2.Nom); });
            return contacts;
        }

        public void removeContact(Contact con)
        {
            if (contacts != null)
            {
                contacts.Remove(con);
            }
        }

        public void addContact(Contact con)
        {
            if(contacts == null)
            {
                contacts = new List<Contact>();
            }
            contacts.Add(con);
        }
        #endregion

    }
}
