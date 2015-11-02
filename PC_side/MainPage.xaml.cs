using Windows.UI.Xaml.Controls;
using LibUsbDotNet;

public static Usbdevice MyUsbDevice;

namespace PC_side
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            console.Text += "Coucou les amis !";
        }
    }
}
