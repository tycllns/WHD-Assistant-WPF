using System;
using System.Collections.Generic;

using System.Windows;

using System.Windows.Input;
using System.Windows.Media;

using System.Windows.Threading;

namespace WHD_Assistant_WPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Utils UT = new Utils();

        private DispatcherTimer timer = new DispatcherTimer();

        public Window1()
        {
            InitializeComponent();

            UT.CheckForXMLFile();

            if (UT.UserNameValue().Length >= 1)
            {
                txtUsername.Text = UT.UserNameValue();
                chkbxRemember.IsChecked = true;
            }

            if (UT.PasswordValue().Length >= 1)
            {
                txtPassword.Password = UT.DecodeFrom64(UT.PasswordValue());
            }

            var bc = new BrushConverter();

            List<string> Theme = UT.RestoreTheme();

            if (Theme[0] != "" && Theme[1] != "" && Theme[2] != "")
            {
                rctTopBar.Fill = (Brush)bc.ConvertFrom(Theme[0]);
                grdLoginPage.Background = (Brush)bc.ConvertFrom(Theme[1]);
                btnLogin.Background = (Brush)bc.ConvertFrom(Theme[2]);
            }
        }

        public void Login1()
        {
            if (txtPassword.Password != null && txtUsername.Text != null)
            {
                if (chkbxRemember.IsChecked == true)
                {
                    if (UT.UserNameValue() != txtUsername.Text)
                    {
                        UT.VerifyLogin(txtUsername.Text, UT.EncodePasswordToBase64(txtPassword.Password), true);
                    }
                    if (UT.PasswordValue() != txtPassword.Password)
                    {
                        UT.VerifyLogin(txtUsername.Text, UT.EncodePasswordToBase64(txtPassword.Password), true);
                    }
                }
                else
                {
                    UT.VerifyLogin(txtUsername.Text, UT.EncodePasswordToBase64(txtPassword.Password), false);
                }

                MainWindow main = new MainWindow(txtUsername.Text, UT.EncodePasswordToBase64(txtPassword.Password));

                main.Show();
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            Login1();
            timer.Stop();
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Rectangle_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}