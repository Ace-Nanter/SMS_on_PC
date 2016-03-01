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
    public partial class Contact : Window
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

        private void ContactWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();

            // Gestion des Tournois
            IList<EntityLayer.Contact> contacts = cm.getContacts();
            ViewModel.Contact.ContactsModelView cmv = new ViewModel.Contact.ContactsModelView(contacts);
            ListContact.DataContext = cmv;
        }
    }
}
