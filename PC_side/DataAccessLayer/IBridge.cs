﻿using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer
{
    public interface IBridge
    {
        List<Conversation> getConversations();
        List<Contact> getContacts();
        Conversation getConversationsFromContact(String contact);
        Contact getContactFromString(String contact);
        void removeContact(Contact con);
        void addContact(Contact con);
        void AddConversation(Contact con);
        void AddMessageToConv(SMS sms, Conversation conv);
    }
}
