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
        public Contact()
        {
            InitializeComponent();
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width * 2;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
            if (ListContact.SelectedItem != null)
            {
                Window.AddContact ac = new Window.AddContact(this,((ContactModelView)ListContact.SelectedItem).Num);
                ac.Show();
            }
        }
    }
}
