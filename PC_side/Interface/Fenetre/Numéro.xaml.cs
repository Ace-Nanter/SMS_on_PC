using Projet.ViewModel.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projet.Fenetre
{
    /// <summary>
    /// Logique d'interaction pour Numéro.xaml
    /// </summary>
    public partial class Numéro : System.Windows.Window
    { 
        private MainWindow parent;
    

        public Numéro(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }
        

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();

            EntityLayer.Conversation c;

            if(AddNum.Text.Length > 0)
            {
                c = cm.getConversationsFromContact(AddNum.Text);
                if (c == null)
                {
                    cm.AddConversation(new EntityLayer.Contact(AddNum.Text));
                    IList<EntityLayer.Conversation> convs = cm.getConversations();
                    ViewModel.Conversation.ConversationsModelView cmv = new ViewModel.Conversation.ConversationsModelView(convs);
                    parent.ListConversations.DataContext = cmv;

                }

                if (!parent.findConvos(AddNum.Text))
                {
                    Conversation cw = new Conversation(parent, AddNum.Text);
                    parent.addConv(cw);
                    cw.Show();
                }

                this.Close();
            }
           
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
