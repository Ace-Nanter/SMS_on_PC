using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ConversationManager
    {
        private static DataAccessLayer.DalManager bdd = DataAccessLayer.DalManager.Instance;

        #region Conversations management
        public List<Conversation> getConversations()
        {
            return bdd.getConversations();
        }
        #endregion
    }
}
