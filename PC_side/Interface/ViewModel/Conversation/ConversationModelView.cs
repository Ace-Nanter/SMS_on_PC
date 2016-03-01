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

        public int NbMessage
        {
            get { return m_conversation.Messages.Count; }
        }

        public String LastMessage
        {
            get { return ((EntityLayer.SMS)Messages.Last()).Body + "[" + NbMessage + "]"; }
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
