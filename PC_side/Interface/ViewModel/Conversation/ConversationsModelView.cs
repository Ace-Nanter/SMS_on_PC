using BiblioWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ViewModel.Conversation
{
    public class ConversationsModelView : ViewModelBase
    {
        private ObservableCollection<ConversationModelView> m_conversations;
        private ConversationModelView m_selectedItem;

        public ObservableCollection<ConversationModelView> Conversations
        {
            get { return m_conversations; }
            private set
            {
                m_conversations = value;
                OnPropertyChanged("Conversations");
            }
        }

        public ConversationModelView SelectedConversation
        {
            get { return m_selectedItem; }
            set
            {
                m_selectedItem = value;
                OnPropertyChanged("SelectedConversation");
            }
        }

        public ConversationsModelView(IList<EntityLayer.Conversation> Convos)
        {
            m_conversations = new ObservableCollection<ConversationModelView>();
            foreach (EntityLayer.Conversation c in Convos)
            {
                m_conversations.Add(new ConversationModelView(c));
            }
        }
    }
}
