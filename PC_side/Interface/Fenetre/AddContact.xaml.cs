using Projet.ViewModel.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projet.Window
{
    /// <summary>
    /// Logique d'interaction pour AddContact.xaml
    /// </summary>
    public partial class AddContact : System.Windows.Window
    {
        private Contact parentWindow;


        public AddContact(Contact contact)
        {
            InitializeComponent();
            this.parentWindow = contact;
        }

        public AddContact(Contact parentWindow,String contact)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();

            EntityLayer.Contact contacts = cm.getContactFromString(contact);
            ViewModel.Contact.ContactModelView cmv = new ViewModel.Contact.ContactModelView(contacts);
            this.DataContext = cmv;

            ButtonAdd.Text = "+ Modifier";
        }
        

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Regex rgx = new Regex(@"^[0-9]+$");
            if (rgx.IsMatch(AddNum.Text))
            {
                BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();

                EntityLayer.Contact contacts = cm.getContactFromString(AddNum.Text);
                if (contacts == null)
                {
                    EntityLayer.Contact c;

                    if (AddNom.Text.Length == 0)
                    {
                        c = new EntityLayer.Contact(AddNum.Text);
                    }
                    else
                    {
                        c = new EntityLayer.Contact(AddNom.Text, AddNum.Text);
                    }
                    cm.addContact(c);
                    IList<EntityLayer.Contact> icon = cm.getContacts();
                    parentWindow.ListContact.DataContext = new ViewModel.Contact.ContactsModelView(icon);

                }
                this.Close();
            }
            else
            {
                AddTitle.Text = "Ajout d'un contact (Numéro Invalide)";
            }
           
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            parentWindow.ListContact.Items.Refresh();
            parentWindow.getParent().ListConversations.Items.Refresh();
        }
    }
}
