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

namespace Projet.UserControls
{
    /// <summary>
    /// Logique d'interaction pour AppButton.xaml
    /// </summary>
    public partial class AppButton : UserControl
    {

        /// <summary>
        /// Gets or sets the Label which is displayed next to the field
        /// </summary>
        public String Label
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
              typeof(AppButton), new PropertyMetadata("Default"));


        public AppButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event EventHandler Event;

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (Event != null)
            {
                Event(this, new EventArgs());
            }
        }
    }
}
