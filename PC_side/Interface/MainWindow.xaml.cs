using Projet.ViewModel.Conversation;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projet
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        Contact ct;

        List<Conversation> convos;

        public MainWindow()
        {
            InitializeComponent();
            convos = new List<Conversation>();
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        public Boolean findConvos(String num)
        {
            foreach(Conversation c in convos)
            {
                if(c.getContact().Equals(num))
                {
                    c.Focus();
                    return true;
                }
            }
            return false;
        }

        public void addConv(Conversation conv)
        {
            convos.Add(conv);
        }

        public void removeConv(Conversation conv)
        {
            convos.Remove(conv);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ReduceButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            this.expander.IsExpanded = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ConnexionButton_Click(object sender, RoutedEventArgs e)
        {
            this.StateText.Text = "Connexion ...";
        }




        private void expander_Expanded(object sender, RoutedEventArgs e)
        {
            ct = new Contact(this);
            ct.Show();

           
        }

        private void expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ct.Close();
        }

        private void AppWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BusinessLayer.ConversationManager cm = new BusinessLayer.ConversationManager();
            
            IList<EntityLayer.Conversation> convs = cm.getConversations();
            ViewModel.Conversation.ConversationsModelView cmv = new ViewModel.Conversation.ConversationsModelView(convs);
            ListConversations.DataContext = cmv;
        }

        private void ListConversations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListConversations.SelectedItem != null)
            {
                if (!findConvos(((ConversationModelView)ListConversations.SelectedItem).Receiver.Num))
                {
                    Conversation cw = new Conversation(this, ((ConversationModelView)ListConversations.SelectedItem).Receiver.Num);
                    addConv(cw);
                    cw.Show();
                }
            }
        }
    }
}
