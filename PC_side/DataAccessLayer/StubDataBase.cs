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
        public List<Conversation> getConversations()
        {
            List<Conversation> convo = new List<Conversation>();

            Conversation conv1 = new Conversation(new Contact("John","0607080910"));
            Conversation conv2 = new Conversation(new Contact("Jim", "0470315054"));

            List<SMS> msgc1 = new List<SMS>();
            msgc1.Add(new SMS("Coucou", conv1.Receiver));
            msgc1.Add(new SMS("Salut !", null));
            msgc1.Add(new SMS("ça va ?", conv1.Receiver));
            msgc1.Add(new SMS("Pas mal pour un test", null));
            msgc1.Add(new SMS("Comme tu le vois", null));

            conv1.Messages = msgc1;

            List<SMS> msgc2 = new List<SMS>();
            msgc1.Add(new SMS("Yop", conv2.Receiver));
            msgc1.Add(new SMS("Salut !", null));
            
            conv2.Messages = msgc2;

            convo.Add(conv1);
            convo.Add(conv2);

            return convo;
        }
    }
}
