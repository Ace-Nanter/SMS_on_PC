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
    public partial class Conversation : System.Windows.Window
    {
        private String contact;
        private MainWindow parent;

        public Conversation(MainWindow parent,String contact)
        {
            InitializeComponent();
            this.contact = contact;
            this.parent = parent;
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();

            EntityLayer.Conversation convs = cm.getConversationsFromContact(contact);
            ViewModel.SMS.SMSsModelView cmv = new ViewModel.SMS.SMSsModelView(convs.Messages);

            ConvTitle.Text = "Conversation avec " + convs.Receiver.Nom + " (" + convs.Receiver.Num + ")";

            FilConv.DataContext = cmv;
        }

        public String getContact()
        {
            return contact;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void AppButton_Click(object sender, EventArgs e)
        {
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();
            if(TextField.Text.Length > 0)
            {
                EntityLayer.SMS sms = new EntityLayer.SMS(TextField.Text, cm.getContactFromString(contact));
                cm.AddMessageToConv(sms, cm.getConversationsFromContact(contact));

                EntityLayer.Conversation convs = cm.getConversationsFromContact(contact);
                ViewModel.SMS.SMSsModelView cmv = new ViewModel.SMS.SMSsModelView(convs.Messages);
                FilConv.DataContext = cmv;

                IList<EntityLayer.Conversation> convs2 = cm.getConversations();
                ViewModel.Conversation.ConversationsModelView cmv2 = new ViewModel.Conversation.ConversationsModelView(convs2);
                parent.ListConversations.DataContext = cmv2;

                TextField.Text = "";
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            parent.removeConv(this);
        }
    }
}
