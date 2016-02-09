using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer {
    class Contact {
        private string m_nom;
        private string m_num;

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
