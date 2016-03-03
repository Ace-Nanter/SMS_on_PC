using BiblioWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ViewModel.Conversation
{
    public class ConversationModelView : ViewModelBase
    {
        private EntityLayer.Conversation m_conversation;

        public EntityLayer.Conversation Conversation
        {
            get { return m_conversation; }
            set { m_conversation = value; }
        }

        public ConversationModelView(EntityLayer.Conversation conversation)
        {
            m_conversation = conversation;
        }

        public EntityLayer.Contact Receiver
        {
            get { return m_conversation.Receiver; }
            set
            {
                m_conversation.Receiver = value;
                OnPropertyChanged("Receiver");
            }
        }

        public String ReceiverName
        {
            get
            {
                if(!m_conversation.Receiver.Nom.Equals("Inconnu"))
                {
                    return m_conversation.Receiver.Nom + "[" + NbMessage + "]";
                }
                else
                {
                    return m_conversation.Receiver.Num + "[" + NbMessage + "]";
                }
            }
        }

        public int NbMessage
        {
            get { return m_conversation.Messages.Count; }
        }

        public String LastMessage
        {
            get
            {
                if (Messages.Count != 0)
                {
                    return ((EntityLayer.SMS)Messages.Last()).Body;
                }
                else
                {
                    return "";
                }
            }
        }

        public List<EntityLayer.SMS> Messages
        {
            get { return m_conversation.Messages; }
            set
            {
                m_conversation.Messages = value;
                OnPropertyChanged("Messages");
            }
        }

    }
}
