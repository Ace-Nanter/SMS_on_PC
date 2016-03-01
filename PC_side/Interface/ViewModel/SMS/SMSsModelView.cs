using BiblioWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ViewModel.SMS
{
    public class SMSsModelView : ViewModelBase
    {
        private ObservableCollection<SMSModelView> m_sms;
        private SMSModelView m_selectedItem;

        public ObservableCollection<SMSModelView> SMSs
        {
            get { return m_sms; }
            private set
            {
                m_sms = value;
                OnPropertyChanged("SMSs");
            }
        }

        public SMSModelView SelectedContact
        {
            get { return m_selectedItem; }
            set
            {
                m_selectedItem = value;
                OnPropertyChanged("SelectedContact");
            }
        }

        public SMSsModelView(IList<EntityLayer.SMS> smss)
        {
            m_sms = new ObservableCollection<SMSModelView>();
            foreach (EntityLayer.SMS s in smss)
            {
                m_sms.Add(new SMSModelView(s));
            }
        }
    }
}
