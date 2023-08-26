using System.Collections.Generic;

using System.Windows;

using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WHD_Assistant_WPF
{
    /// <summary>
    /// Interaction logic for New_EditTemplate.xaml
    /// </summary>
    public partial class New_EditTemplate : Window
    {
        private Utils UT = new Utils();

        //public delegate void callback_data(string someData);

        //public event callback_data getData_CallBack;

        public New_EditTemplate(string Type, Template ID = null)
        {
            InitializeComponent();

            if (Type == "Edit")
            {
                btnApply.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                cmbxAssTech.SelectedItem = ID.AssignedTech;
                cmbxLocation.SelectedItem = ID.Location;
                cmbxRequestType.SelectedItem = ID.RequestType;
                cmbxStatus.SelectedItem = ID.Status;
                txtTitle.Text = ID.TemplateName;
                txtBody.Document.Blocks.Clear();
                txtBody.Document.Blocks.Add(new Paragraph(new Run(ID.Body)));
                lblTempID.Content = ID.TemplateID;
            }
            if (Type == "Add")
            {
                btnCreate.Visibility = Visibility.Visible;
            }
            LoadElements();

            checkTheme();
        }

        public void LoadElements()
        {
            List<List<string>> Lists = UT.RetreiveListFromXML();

            foreach (List<string> item in Lists)
            {
                if (item.Contains("Locations"))
                {
                    foreach (string l in item)
                    {
                        cmbxLocation.Items.Add(l);
                    }
                }

                if (item.Contains("AssignedTechs"))
                {
                    foreach (string l in item)
                    {
                        cmbxAssTech.Items.Add(l);
                    }
                }

                if (item.Contains("Statuses"))
                {
                    foreach (string l in item)
                    {
                        cmbxStatus.Items.Add(l);
                    }
                }

                if (item.Contains("RequestTypes"))
                {
                    foreach (string l in item)
                    {
                        cmbxRequestType.Items.Add(l);
                    }
                }
            }
        }

        public void checkTheme()
        {
            var bc = new BrushConverter();

            List<string> Theme = UT.RestoreTheme();

            if (Theme[0] != "" && Theme[1] != "" && Theme[2] != "")
            {
                rctTopBar.Fill = (Brush)bc.ConvertFrom(Theme[0]);
                grdMain.Background = (Brush)bc.ConvertFrom(Theme[1]);
            }
        }

        public void FillBoxes(string id, string name, string location, string asstech, string rtype, string status, string body)
        {
            lblTempID.Content = id;

            txtTitle.Text = name;

            cmbxLocation.SelectedItem = location;

            cmbxAssTech.SelectedItem = asstech;

            cmbxRequestType.SelectedItem = rtype;

            cmbxStatus.SelectedItem = status;

            txtBody.Document.Blocks.Clear();
            txtBody.Document.Blocks.Add(new Paragraph(new Run(body)));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(new TextRange(txtBody.Document.ContentStart, txtBody.Document.ContentEnd).Text);
            if (txtTitle.Text != null && new TextRange(txtBody.Document.ContentStart, txtBody.Document.ContentEnd).Text != null)
            {
                if (cmbxAssTech.SelectedItem == null || cmbxLocation.SelectedItem == null || cmbxRequestType.SelectedItem == null || cmbxStatus.SelectedItem == null)
                {
                    MessageBoxResult done = System.Windows.MessageBox.Show("Some Template data is missing, do you wan to continue anyway?", "Continue?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (done == MessageBoxResult.Yes)
                    {
                        UT.AddRecordToXML(txtTitle.Text, cmbxStatus.Text, cmbxLocation.Text, cmbxAssTech.Text, cmbxRequestType.Text, new TextRange(txtBody.Document.ContentStart, txtBody.Document.ContentEnd).Text);
                        this.Close();
                    }
                }
                else
                {
                    UT.AddRecordToXML(txtTitle.Text, cmbxStatus.Text, cmbxLocation.Text, cmbxAssTech.Text, cmbxRequestType.Text, new TextRange(txtBody.Document.ContentStart, txtBody.Document.ContentEnd).Text);
                    this.Close();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter a Title and body before creating");
            }
            UT.AddToAchCounter("TemplatesCreate");
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            UT.EditXMLNode(lblTempID.Content.ToString(), txtTitle.Text, cmbxStatus.Text, cmbxLocation.Text, cmbxAssTech.Text, cmbxRequestType.Text, new TextRange(txtBody.Document.ContentStart, txtBody.Document.ContentEnd).Text);

            UT.AddToAchCounter("TemplatesEdit");
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            UT.DeleteXMLNode(lblTempID.Content.ToString());
            this.Close();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //string myData = "Top Secret Data To Share";
        }
    }
}