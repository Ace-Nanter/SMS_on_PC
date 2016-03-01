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

namespace Projet
{
    /// <summary>
    /// Logique d'interaction pour Conversation.xaml
    /// </summary>
    public partial class Conversation : Window
    {
        public Conversation(String contact)
        {
            InitializeComponent();
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();

            EntityLayer.Conversation convs = cm.getConversationsFromContact(contact);
            ViewModel.SMS.SMSsModelView cmv = new ViewModel.SMS.SMSsModelView(convs.Messages);
            FilConv.DataContext = cmv;
        }



        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
