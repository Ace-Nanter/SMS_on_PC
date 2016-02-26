using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Projet.UserControls
{
    /// <summary>
    /// Logique d'interaction pour BulleConvSender.xaml
    /// </summary>
    public partial class BulleConvSender : UserControl
    {
        public DateTime Date
        {
            get { return DateTime.Parse(this.senderDate.Text); }
            set { this.senderDate.Text = value.ToString("HH:mm",CultureInfo.InvariantCulture); }
        }

        public String SenderName
        {
            get { return this.senderName.Text; }
            set { this.senderName.Text = value; }
        }

        public String Message
        {
            get { return this.senderMessage.Text; }
            set { this.senderMessage.Text = value; }
        }

        public BulleConvSender()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
