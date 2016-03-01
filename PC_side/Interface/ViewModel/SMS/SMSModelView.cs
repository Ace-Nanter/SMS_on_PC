using BiblioWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.ViewModel.SMS
{
    public class SMSModelView : ViewModelBase
    {
        private EntityLayer.SMS m_sms;

        public EntityLayer.SMS Sms
        {
            get { return m_sms; }
            set { m_sms = value; }
        }

        public SMSModelView(EntityLayer.SMS Sms)
        {
            m_sms = Sms;
        }

        public String Contact
        {
            get
            {
                if (m_sms.Contact != null)
                    return m_sms.Contact.Nom;
                else
                    return "Moi";
            }
        }

        public String Heure
        {
            get { return "[" + m_sms.Date.Hour  + ":" + m_sms.Date.Minute + "] "; }
        }

        public String Message
        {
            get { return m_sms.Body; }
            set
            {
                m_sms.Body = value;
                OnPropertyChanged("Body");
            }
        }
    }
}
