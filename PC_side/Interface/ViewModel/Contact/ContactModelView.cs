using BiblioWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ViewModel.Contact
{
    public class ContactModelView : ViewModelBase
    {
        private EntityLayer.Contact m_contact;

        public EntityLayer.Contact Contact
        {
            get { return m_contact; }
            set { m_contact = value; }
        }

        public ContactModelView(EntityLayer.Contact contact)
        {
            m_contact = contact;
        }

        public String Nom
        {
            get { return m_contact.Nom; }
            set
            {
                m_contact.Nom = value;
                OnPropertyChanged("Nom");
            }
        }

        public String Num
        {
            get { return m_contact.Num; }
            set
            {
                m_contact.Num = value;
                OnPropertyChanged("Num");
            }
        }
    }
}
