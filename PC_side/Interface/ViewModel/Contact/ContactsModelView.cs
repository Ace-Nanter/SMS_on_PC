using BiblioWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ViewModel.Contact
{
    public class ConversationsModelView : ViewModelBase
    {

        private ObservableCollection<ContactModelView> m_contact;
        private ContactModelView m_selectedItem;

        public ObservableCollection<ContactModelView> Contacts
        {
            get { return m_contact; }
            private set
            {
                m_contact = value;
                OnPropertyChanged("Contacts");
            }
        }

        public ContactModelView SelectedContact
        {
            get { return m_selectedItem; }
            set
            {
                m_selectedItem = value;
                OnPropertyChanged("SelectedContact");
            }
        }

        public ConversationsModelView(IList<EntityLayer.Contact> conts)
        {
            m_contact = new ObservableCollection<ContactModelView>();
            foreach (EntityLayer.Contact c in conts)
            {
                m_contact.Add(new ContactModelView(c));
            }
        }
    }
}
