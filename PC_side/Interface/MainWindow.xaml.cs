using BusinessLayer;
using Projet.ViewModel.Conversation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using UsbLayer;
using EntityLayer;

namespace Projet
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, UsbInterface
    {

        Contact ct;
        UsbManager m_manager = null;
        List<Conversation> convos;

        public MainWindow()
        {
            InitializeComponent();
            convos = new List<Conversation>();
            m_manager = UsbManager.getInstance(this);
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            initUSB();
        }

        private void initUSB() {
            try {
                if(m_manager.connect()) {
                    MessageBox.Show("OK !",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else {
                    MessageBox.Show("Failure !"
                        ,"Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception e) {
                MessageBox.Show("An exception occured : " + e,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            m_manager.stop();
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

        public void hasRead(string msg) {
            
        }

        public void hasBeenConnected() {
            //StateText.Text = "Connecté";
        }

        public void hasBeenStopped() {
            StateText.Text = "Arrêté";
        }

        /// <summary>
        /// Add the message to the conversation
        /// </summary>
        /// <param name="sms"></param>
        public void smsReceived(SMS sms) {
            
        }
    }
}
