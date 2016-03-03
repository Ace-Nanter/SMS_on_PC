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

namespace Projet
{
    /// <summary>
    /// Logique d'interaction pour Contact.xaml
    /// </summary>
    public partial class Contact : System.Windows.Window
    {
        private MainWindow parentWindow;


        public Contact(MainWindow parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width * 2;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public MainWindow getParent()
        {
            return parentWindow;
        }

        public void ContactWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();
            
            IList<EntityLayer.Contact> contacts = cm.getContacts();
            ViewModel.Contact.ContactsModelView cmv = new ViewModel.Contact.ContactsModelView(contacts);
            ListContact.DataContext = cmv;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Window.AddContact ac = new Window.AddContact(this);
            ac.Show();
        }

        private void ListContacts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();
            if (ListContact.SelectedItem != null)
            {
                EntityLayer.Conversation c;

                c = cm.getConversationsFromContact(((ContactModelView)ListContact.SelectedItem).Contact.Num);
                if (c == null)
                {
                    cm.AddConversation(((ContactModelView)ListContact.SelectedItem).Contact);
                    IList<EntityLayer.Conversation> convs = cm.getConversations();
                    ViewModel.Conversation.ConversationsModelView cmv = new ViewModel.Conversation.ConversationsModelView(convs);
                    parentWindow.ListConversations.DataContext = cmv;

                }

                if (!parentWindow.findConvos(((ContactModelView)ListContact.SelectedItem).Contact.Num))
                {
                    Conversation cw = new Conversation(parentWindow, ((ContactModelView)ListContact.SelectedItem).Contact.Num);
                    parentWindow.addConv(cw);
                    cw.Show();
                }   
            }
        }

        private void MenuItem_Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (ListContact.SelectedItem != null)
            {
                Window.AddContact ac = new Window.AddContact(this, ((ContactModelView)ListContact.SelectedItem).Num);
                ac.Show();
            }
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();

            if (ListContact.SelectedItem != null)
            {
                cm.removeContact(((ContactModelView)ListContact.SelectedItem).Contact);
                IList<EntityLayer.Contact> contacts = cm.getContacts();
                ViewModel.Contact.ContactsModelView cmv = new ViewModel.Contact.ContactsModelView(contacts);
                ListContact.DataContext = cmv;
            }
        }
    }
}
