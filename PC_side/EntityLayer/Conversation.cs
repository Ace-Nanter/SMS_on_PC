using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class Conversation
    {

        private Contact receiver;
        private List<SMS> messages;

        public Conversation()
        {
            receiver = null;
            messages = new List<SMS>();
        }

        public Conversation(Contact c)
        {
            receiver = c;
            messages = new List<SMS>();
        }

        public Contact Receiver
        {
            get{ return receiver; }
            set{ receiver = value; }
        }

        public List<SMS> Messages
        {
            get { return messages; }
            set { messages = value; }
        }
    }
}
