using EntityLayer;
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
        //Conversation getConversationByContact(Contact c); //Peut être superflux ?
    }
}
