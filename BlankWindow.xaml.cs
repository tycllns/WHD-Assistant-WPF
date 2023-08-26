using System.Windows;
using System.Windows.Input;

namespace WHD_Assistant_WPF
{
    /// <summary>
    /// Interaction logic for BlankWindow.xaml
    /// </summary>
    public partial class BlankWindow : Window
    {
        private Utils UT = new Utils();

        public BlankWindow(string url)
        {
            InitializeComponent();
            UT.HideScriptErrors(wbLookup, true);
            wbLookup.Navigate(url);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
        }

        private void rctTop_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void imgForward_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (wbLookup.CanGoForward)
            {
                wbLookup.GoForward();
            }
        }

        private void imgBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (wbLookup.CanGoBack)
            {
                wbLookup.GoBack();
            }
        }

        private void imgHome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}