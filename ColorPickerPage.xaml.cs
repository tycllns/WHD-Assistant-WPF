using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WHD_Assistant_WPF
{
    /// <summary>
    /// Interaction logic for ColorPickerPage.xaml
    /// </summary>
    public partial class ColorPickerPage : Window
    {
        private Utils UT = new Utils();

        private DispatcherTimer timer = new DispatcherTimer();

        public ColorPickerPage()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(.5);
            timer.Tick += timer_Tick;
            timer.Start();

            ChangeTheme();

            List<string> Theme = UT.RestoreTheme();

            if (Theme[0] != "" && Theme[1] != "" && Theme[2] != "" && Theme[3] != "" && Theme[4] != "")
            {
                txtTBC.Text = Theme[0];
                txtMFC.Text = Theme[1];
                txtBTNColor.Text = Theme[2];
                txtFontColor.Text = Theme[3];
                cmbxFontStyle.Text = Theme[4];
            }
        }

        public void ChangeTheme()
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                var bc = new BrushConverter();
                var fc = new FontFamilyConverter();

                List<string> Theme = UT.RestoreTheme();

                if (Theme[0] != "" && Theme[1] != "" && Theme[2] != "" && Theme[3] != "" && Theme[4] != "")
                {
                    rctTopBar.Fill = (Brush)bc.ConvertFrom(Theme[0]);
                    grdMain.Background = (Brush)bc.ConvertFrom(Theme[1]);
                    btnCancel.Background = (Brush)bc.ConvertFrom(Theme[2]);
                    btn_Select.Background = (Brush)bc.ConvertFrom(Theme[2]);

                    btnCancel.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                    btn_Select.Foreground = (Brush)bc.ConvertFrom(Theme[3]);

                    txtblkNumeric.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                    txtblkSymbols.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                    txtblkUpper.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                    txtblkLower.Foreground = (Brush)bc.ConvertFrom(Theme[3]);

                    btnCancel.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                    btn_Select.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                }
            })); //Dispatcher end
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                ChangeTheme();
            };
            bw.RunWorkerAsync(100);

            var bc = new BrushConverter();

            if (bc.IsValid(txtMFC.Text) && txtMFC.Text.Length > 1)
            {
                rctMFC.Fill = (Brush)bc.ConvertFrom(txtMFC.Text);
                mfcX.Visibility = Visibility.Collapsed;
            }
            else
            {
                rctMFC.Fill = Brushes.Gray;
                mfcX.Visibility = Visibility.Visible;
            }

            if (bc.IsValid(txtBTNColor.Text) && txtBTNColor.Text.Length > 1)
            {
                rctBC.Fill = (Brush)bc.ConvertFrom(txtBTNColor.Text);
                btnX.Visibility = Visibility.Collapsed;
            }
            else
            {
                rctBC.Fill = Brushes.Gray;
                btnX.Visibility = Visibility.Visible;
            }

            if (bc.IsValid(txtTBC.Text) && txtTBC.Text.Length > 1)
            {
                rctTBC.Fill = (Brush)bc.ConvertFrom(txtTBC.Text);
                //tbcX.Visibility = Visibility.Collapsed;
            }
            else
            {
                rctTBC.Fill = Brushes.Gray;
                //tbcX.Visibility = Visibility.Visible;
            }

            if (bc.IsValid(txtFontColor.Text) && txtFontColor.Text.Length > 1)
            {
                rctFC.Fill = (Brush)bc.ConvertFrom(txtFontColor.Text);

                fntX.Visibility = Visibility.Collapsed;
            }
            else
            {
                rctFC.Fill = Brushes.Gray;

                fntX.Visibility = Visibility.Visible;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            if (txtFontColor.Text != "#" && txtBTNColor.Text != "#" && txtMFC.Text != "#" && cmbxFontStyle.SelectedItem != null)
            {
                UT.ChangeTheme(txtMFC.Text, txtBTNColor.Text, txtFontColor.Text, cmbxFontStyle.SelectedItem.ToString());
                this.Close();
            }
            else
            {
                MessageBox.Show("Please fill in all required fields");
            }
        }

        private void rctTopBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void txtMFC_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void txtBTNColor_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void txtTBC_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void txtFontStyle_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void cmbxFontStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtblkLower.FontFamily = cmbxFontStyle.SelectedItem as FontFamily;
            txtblkUpper.FontFamily = cmbxFontStyle.SelectedItem as FontFamily;
            txtblkNumeric.FontFamily = cmbxFontStyle.SelectedItem as FontFamily;
            txtblkSymbols.FontFamily = cmbxFontStyle.SelectedItem as FontFamily;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}