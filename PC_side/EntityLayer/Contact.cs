using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer {
    public class Contact {
        private string m_nom;
        private string m_num;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nom">Name of the contact</param>
        /// <param name="num">Phone number of the contact</param>
        public Contact(string nom, string num) {
            m_nom = nom;
            m_num = num;
        }

        public string Nom
        {
            get
            {
                return m_nom;
            }

            set
            {
                m_nom = value;
            }
        }

        public string Num
        {
            get
            {
                return m_num;
            }

            set
            {
                m_num = value;
            }
        }
    }
}
