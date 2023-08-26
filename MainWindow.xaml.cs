using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace WHD_Assistant_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //PUBLIC VARIABLES & STUFF

        public List<string> LocationsList = new List<string>();
        public List<string> AssignedTechsList = new List<string>();
        public List<string> RequestTypesList = new List<string>();
        public List<string> StatusesList = new List<string>();

        private Utils UT = new Utils();

        public string Username = "";
        public string Password = "";
        public string tktno;

        private Utils.TktState state = Utils.TktState.New;

        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer ThemeTimer = new DispatcherTimer();
        private DispatcherTimer TemplateTimer = new DispatcherTimer();

        public Boolean IsOpen()
        {
            return true;
        }

        private static BackgroundWorker backgroundWorker;

        //CORE METHODS

        public MainWindow(string username, string password)
        {
            Username = username;
            Password = password;

            backgroundWorker = new BackgroundWorker();
            checkTheme();
            InitializeComponent();

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            UT.HideScriptErrors(wbWHD, true);

            grdLoader.Visibility = Visibility.Visible;

            timer.Interval = TimeSpan.FromSeconds(.005);
            timer.Tick += timer_Tick;
            timer.Start();

            ThemeTimer.Interval = TimeSpan.FromMilliseconds(.5);
            ThemeTimer.Tick += ThemeTimer_Tick;
            ThemeTimer.Start();

            TemplateTimer.Interval = TimeSpan.FromMilliseconds(2);
            TemplateTimer.Tick += TemplateTimer_Tick;
            TemplateTimer.Start();
            FillSig();

            string path2 = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\UpdateVersion.txt";

            string onlineVersion = File.ReadAllText(path2);

            lblVersion.Content = "Version" + onlineVersion;
        }

        //HTML METHODS

        public void save()
        ///Checks for Save button Element on page, if it exists, clicks it.
        {
            HTMLDocument doc = (mshtml.HTMLDocument)wbWHD.Document;

            IHTMLElement Save = doc.getElementById("saveButtonHidden");
            if (Save != null)
            {
                Save.click();
            }
        }

        public void fillCMBXs()
        ///1.Checks for Ticket Location element on page. If it exists, checks for all elements within the element and adds each entry into the Ticket Location element in the LeftTemplates panel.

        ///2.Checks for Assigned Tech element on page.If it exists, checks for all elements within the element and adds each entry into the Assigned Tech element in the LeftTemplates panel.

        ///3.Checks for Problem Type element on page.If it exists, checks for all elements within the element and adds each entry into the Request Type element in the LeftTemplates panel.

        ///4.Checks for Request Status element on page.If it exists, checks for all elements within the element and adds each entry into the Request Status element in the LeftTemplates panel.
        {
            if (wbWHD.Source != null)
            {
                if (UT.IsThisATicket(wbWHD.Source.ToString()))
                {
                    StatusesList.Clear();
                    LocationsList.Clear();
                    RequestTypesList.Clear();
                    AssignedTechsList.Clear();

                    HTMLDocument doc = (mshtml.HTMLDocument)wbWHD.Document;

                    string Problem = "";

                    IHTMLElement TicketLoc = doc.getElementById("TicketLocation");

                    IHTMLElement AssignedTech = doc.getElementById("assignedTechPopup");

                    if (cmbxAssTech.Items.Count > 2 && cmbxLocation.Items.Count > 2 && cmbxStatus.Items.Count > 2 && cmbxRequestType.Items.Count > 2)
                    {
                    }
                    else
                    {
                        if (TicketLoc != null)
                        {
                            string selected = cmbxLocation.SelectedIndex.ToString();
                            IHTMLElementCollection Locations = TicketLoc.children;

                            foreach (IHTMLElement child in Locations)
                            {
                                if (child.innerText != null)
                                {
                                    cmbxLocation.Items.Add(child.innerText);
                                    LocationsList.Add(child.innerText);
                                }
                            }
                            UT.AddListToXML(LocationsList, "Locations");
                        }

                        if (AssignedTech != null)
                        {
                            string selected = cmbxAssTech.SelectedIndex.ToString();

                            IHTMLElementCollection Assigned = AssignedTech.children;

                            foreach (IHTMLElement child in Assigned)
                            {
                                if (child.innerText != null)
                                {
                                    cmbxAssTech.Items.Add(child.innerText);
                                    AssignedTechsList.Add(child.innerText);
                                }
                            }
                            UT.AddListToXML(AssignedTechsList, "AssignedTechs");
                        }

                        IHTMLElementCollection ProblemType = doc.getElementsByTagName("select");

                        foreach (IHTMLElement selects in ProblemType)
                        {
                            if (selects.id.Contains("ProblemType_"))
                            {
                                Problem = selects.id;

                                break;
                            }
                        }

                        IHTMLElement ProbType = doc.getElementById(Problem);

                        if (ProbType != null)
                        {
                            string selected = cmbxRequestType.SelectedIndex.ToString();

                            IHTMLElementCollection problems = ProbType.children;

                            foreach (IHTMLElement child in problems)
                            {
                                if (child.innerText != null)
                                {
                                    cmbxRequestType.Items.Add(child.innerText);
                                    RequestTypesList.Add(child.innerText);
                                }
                            }
                            UT.AddListToXML(RequestTypesList, "RequestTypes");
                        }

                        IHTMLElementCollection Status2 = doc.getElementsByTagName("select");

                        foreach (IHTMLElement selects in Status2)
                        {
                            string selected = selects.getAttribute("name").ToString();

                            if (selected.Contains("7.25"))
                            {
                                IHTMLElementCollection Statuses = selects.children;

                                foreach (IHTMLElement child in Statuses)
                                {
                                    if (child.innerText != null)
                                    {
                                        StatusesList.Add(child.innerText);
                                    }
                                }

                                if (StatusesList.Contains("Open"))
                                {
                                    foreach (string t in StatusesList)
                                    {
                                        cmbxStatus.Items.Add(t);
                                    }
                                    UT.AddListToXML(StatusesList, "Statuses");
                                    break;
                                }
                                else
                                {
                                    StatusesList.Clear();
                                }
                            }

                            UT.AddListToXML(LocationsList, "Locations");
                            UT.AddListToXML(AssignedTechsList, "AssignedTechs");
                            UT.AddListToXML(RequestTypesList, "RequestTypes");
                        }
                    }
                }
            }
        }

        public void GetCCElements()
        {
            HTMLDocument doc = (mshtml.HTMLDocument)wbWHD.Document;

            IHTMLElement Save = doc.getElementById("saveButtonHidden");
        }

        //TEMPLATE METHODS

        public void addTextToPage(string newValue)
        ///Checks for NoteText element on webpage, if it exists, sets the text attribute to be equal to the NewValue parameter that is passed the method upon calling it, otherwise, checks if the new note button exists, if it does, clicks it and then sets the text attribute of the NoteText element to be equal to the NewValue parameter that is passed the method upon calling it.
        {
            HTMLDocument doc = (mshtml.HTMLDocument)wbWHD.Document;

            var txt = doc.getElementById("noteText");
            var Button = doc.getElementById("newNoteButtonHidden");

            if (txt != null)
            {
                if (chkbxIncludeSignature.IsChecked == true)
                {
                    txt.setAttribute("value", newValue + "\n" + "\n" + txtSignature.Text);
                }
                else
                {
                    txt.setAttribute("value", newValue);
                }
            }
            else if (Button != null)
            {
                Button.click();
                if (txt != null)
                {
                    if (chkbxIncludeSignature.IsChecked == true)
                    {
                        txt.setAttribute("value", newValue + "\n" + "\n" + txtSignature.Text);
                    }
                    else
                    {
                        txt.setAttribute("value", newValue);
                    }
                }
            }
        }

        public void TemplatesImport()
        {
            List<Template> Templates = UT.GetXMLTagValues();
            List<string> TNames = new List<string>();

            foreach (Template T in Templates)
            {
                TNames.Add(T.TemplateName);
            }
            foreach (Template T in Templates)
            {
                if (!cmbxTemplates.Items.Contains(T.TemplateName))
                {
                    cmbxTemplates.Items.Add(T.TemplateName);
                }
                if (cmbxTemplates.Items.Contains(T.TemplateName) && !TNames.Contains(T.TemplateName))
                {
                    cmbxTemplates.Items.Remove(T.TemplateName);
                }
            }
            Debug.WriteLine(Templates.Count);
        }

        public void categorize()
        ///Checks to see if the Templates combobox is not set to index 0, if not, creates a string called CMTitle from the selelcted item in the Templates combobox. Checks to see if the CMTitle Variable contains "CT + a number under 100", if it does, check each Public List (CTText, CTAssTech, CTLoc, CTStatus, CTType) its value. If the value is 0, do nothing, if not, change the combobox to the corresponding list index's string.

        ///If the value of the Templates combobox is 0, show a message box that says "No Template Selected".
        {
            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    HTMLDocument doc = (mshtml.HTMLDocument)wbWHD.Document;
                    string Problem = "";

                    IHTMLElement TicketLoc = doc.getElementById("TicketLocation");

                    IHTMLElement AssignedTech = doc.getElementById("assignedTechPopup");

                    if (TicketLoc != null)
                    {
                        string selected = cmbxLocation.SelectedIndex.ToString();
                        IHTMLElementCollection Locations = TicketLoc.children;

                        foreach (IHTMLElement child in Locations)
                        {
                            if (child.innerText != null)
                            {
                                if (child.getAttribute("value") == selected)
                                {
                                    child.setAttribute("selected", "selected");
                                    break;
                                }
                            }
                        }
                    }

                    if (AssignedTech != null)
                    {
                        string selected = cmbxAssTech.SelectedIndex.ToString();

                        IHTMLElementCollection Assigned = AssignedTech.children;

                        foreach (IHTMLElement child in Assigned)
                        {
                            if (child.innerText != null)
                            {
                                if (child.getAttribute("value") == selected)
                                {
                                    child.setAttribute("selected", "selected");
                                    break;
                                }
                            }
                        }
                    }

                    IHTMLElementCollection ProblemType = doc.getElementsByTagName("select");

                    foreach (IHTMLElement selects in ProblemType)
                    {
                        if (selects.id.Contains("ProblemType_"))
                        {
                            Problem = selects.id;

                            break;
                        }
                    }

                    IHTMLElement ProbType = doc.getElementById(Problem);

                    if (ProbType != null)
                    {
                        string selected = cmbxRequestType.SelectedIndex.ToString();

                        IHTMLElementCollection problems = ProbType.children;

                        foreach (IHTMLElement child in problems)
                        {
                            if (child.innerText != null)
                            {
                                if (child.getAttribute("value") == selected)
                                {
                                    child.setAttribute("selected", "selected");
                                    break;
                                }
                            }
                        }
                    }

                    IHTMLElementCollection Status2 = doc.getElementsByTagName("select");

                    foreach (IHTMLElement selects in Status2)
                    {
                        string selected = selects.getAttribute("name").ToString();

                        string selected2 = null;

                        if (cmbxStatus.SelectedItem.ToString() != null)
                        {
                            selected2 = cmbxStatus.SelectedItem.ToString();
                        }

                        if (selected.Contains(""))
                        {
                            IHTMLElementCollection Statuses = selects.children;
                            foreach (IHTMLElement child in Statuses)
                            {
                                if (child.innerText != null)
                                {
                                    if (child.innerText == selected2)
                                    {
                                        child.setAttribute("selected", "selected");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    save();
                })); //Dispatcher end
            };

            bw.RunWorkerAsync(100);
        }

        public void hideLoginPage()
        {
            HTMLDocument doc = (mshtml.HTMLDocument)wbWHD.Document;

            IHTMLElement LoginPage = doc.getElementById("LoginBox");
            IHTMLElement submenuContainer = doc.getElementById("SubmenuContainer");
            IHTMLElement headerBar = doc.getElementById("headerBar");
            IHTMLElement navigation = doc.getElementById("navigation");

            if (submenuContainer != null)
            {
                IHTMLElementCollection smContainerChildren = submenuContainer.children;

                foreach (IHTMLElement smc in smContainerChildren)
                {
                    if (smc != null)
                    {
                        if (smc.getAttribute("title").Contains("View Tickets you have flagged for yourself"))
                        {
                            smc.setAttribute("style", "display:none");
                        }
                        if (smc.getAttribute("title").Contains("List Tickets edited by you recently"))
                        {
                            smc.setAttribute("style", "display:none");
                        }
                    }
                }
            }

            if (LoginPage != null)
            {
                LoginPage.setAttribute("style", "display:none");
            }

            if (headerBar != null)
            {
                headerBar.setAttribute("style", "display:none");
            }
            if (navigation != null)
            {
                navigation.setAttribute("style", "display:none");
            }
        }

        public void checkCustomTemplate()
        ///
        ///Checks to see if noteText element exists on page. If it does not it opens the notetext box then adds the text from the template text box to the noteText. Otherwise it just adds the text from the template text box to the noteText.
        ///
        {
            HTMLDocument doc = (mshtml.HTMLDocument)wbWHD.Document;

            var txt = doc.getElementById("noteText");
            var btn = doc.getElementById("newNoteButtonHidden");

            if (txt == null)
            {
                btn.click();

                addTextToPage(txtBody.Text);
            }
            if (txt != null)
            {
                addTextToPage(txtBody.Text);

                UT.AddToAchCounter("TemplatesInsert");
            }
        }

        //THEME METHODS

        public void checkTheme()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    var bc = new BrushConverter();
                    var fc = new FontFamilyConverter();

                    List<string> Theme = UT.RestoreTheme();

                    if (Theme[0] != "" && Theme[1] != "" && Theme[2] != "" && Theme[3] != "" && Theme[4] != "")
                    {
                        topbar.Fill = (Brush)bc.ConvertFrom(Theme[0]);
                        grdspltR.Background = (Brush)bc.ConvertFrom(Theme[0]);
                        grpspltL.Background = (Brush)bc.ConvertFrom(Theme[0]);

                        grdMain.Background = (Brush)bc.ConvertFrom(Theme[1]);
                        grdLoader.Background = (Brush)bc.ConvertFrom(Theme[1]);
                        rctLoaderHiderLeft.Fill = (Brush)bc.ConvertFrom(Theme[1]);
                        rctLoaderHiderRight.Fill = (Brush)bc.ConvertFrom(Theme[1]);

                        btnBilling.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnBilling2.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnLeftOpen.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnCloseRight.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnPhoneLookup.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnCloseLeftPanel.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnRightOpen.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnClientLookup.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnSaveSig.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnESearch.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        btnStaffInfo.Background = (Brush)bc.ConvertFrom(Theme[2]);
                        lblTemplatesAdded1.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        lblTemplateInserts.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        lblTemplatesAdded.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        lblCategorized.Foreground = (Brush)bc.ConvertFrom(Theme[3]);

                        btnBilling.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnBilling2.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnLeftOpen.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnCloseRight.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnPhoneLookup.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnCloseLeftPanel.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnRightOpen.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnClientLookup.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        txtblkLoading.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnESearch.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnStaffInfo.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        lblTemplateInserts.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        lblTemplatesAdded.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        lblCategorized.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        lblTemplatesAdded1.Foreground = (Brush)bc.ConvertFrom(Theme[3]);

                        dtpBillingDate.Foreground = (Brush)bc.ConvertFrom(Theme[3]);
                        btnSaveSig.Foreground = (Brush)bc.ConvertFrom(Theme[3]);

                        btnBilling.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnBilling2.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        txtChartNumber.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnLeftOpen.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnCloseRight.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnPhoneLookup.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnCloseLeftPanel.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnRightOpen.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnClientLookup.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnSaveSig.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnStaffInfo.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        btnESearch.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);

                        txtblkLoading.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);

                        cmbxAssTech.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        cmbxLocation.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        cmbxOperator.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        cmbxStatus.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        cmbxTemplates.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        lblCategorized.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        lblTemplateInserts.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        lblTemplatesAdded.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        lblTemplatesAdded1.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);

                        txtBody.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                        txtSignature.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);

                        dtpBillingDate.FontFamily = (FontFamily)fc.ConvertFrom(Theme[4]);
                    }
                })); //Dispatcher end
            };

            bw.RunWorkerAsync(100);
        }

        //UI METHODS

        public void TopbarButtonMover()
        {
        }

        public void TemplateVisibilitySetter()
        {
            string html = wbWHD.Source.ToString();

            if (UT.IsThisATicket(html))
            {
                cmbxAssTech.Visibility = Visibility.Visible;
                cmbxLocation.Visibility = Visibility.Visible;
                cmbxRequestType.Visibility = Visibility.Visible;
                cmbxStatus.Visibility = Visibility.Visible;
                cmbxTemplates.Visibility = Visibility.Visible;
                imgCategorize.Visibility = Visibility.Visible;
                imgInsert.Visibility = Visibility.Visible;
                txtBody.Visibility = Visibility.Visible;
                btnOpen_CloseSigPanel.Visibility = Visibility.Visible;

                string tktno = UT.GetTktNo(html);
                lblTktNo.Text = tktno;

                chkbxIncludeSignature.Visibility = Visibility.Visible;
                txtSignature.Visibility = Visibility.Visible;
                btnSaveSig.Visibility = Visibility.Visible;
            }
            else
            {
                cmbxAssTech.Visibility = Visibility.Collapsed;
                cmbxLocation.Visibility = Visibility.Collapsed;
                cmbxRequestType.Visibility = Visibility.Collapsed;
                cmbxStatus.Visibility = Visibility.Collapsed;
                cmbxTemplates.Visibility = Visibility.Collapsed;
                imgCategorize.Visibility = Visibility.Collapsed;
                imgInsert.Visibility = Visibility.Collapsed;
                txtBody.Visibility = Visibility.Collapsed;
                lblTktNo.Text = "";
                chkbxIncludeSignature.Visibility = Visibility.Collapsed;
                txtSignature.Visibility = Visibility.Collapsed;
                btnSaveSig.Visibility = Visibility.Collapsed;
                btnOpen_CloseSigPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void FillSig()
        {
            List<string> Signature = UT.GetSignature();

            if (Signature != null)
            {
                txtSignature.Text = Signature[0].ToString();
                bool Ischecked = bool.Parse(Signature[1].ToString());
                chkbxIncludeSignature.IsChecked = Ischecked;
            }
        }


        //BUTTON EVENTS

        private void btnCloseLeftPanel_Click(object sender, RoutedEventArgs e)
        {
            UT.SidePanelManipulator(grdMain, btnLeftOpen, false, 0);
        }

        private void btnLeftOpen_Click(object sender, RoutedEventArgs e)
        {
            UT.SidePanelManipulator(grdMain, btnLeftOpen, true, 0);
        }

        private void btnCloseRight_Click(object sender, RoutedEventArgs e)
        {
            UT.SidePanelManipulator(grdMain, btnRightOpen, false, 4);
        }

        private void btnRightOpen_Click(object sender, RoutedEventArgs e)
        {
            UT.SidePanelManipulator(grdMain, btnRightOpen, true, 4);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            checkCustomTemplate();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btnCloseWhenRightPanelClosed_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void btnCategorize_Click(object sender, RoutedEventArgs e)
        {
            categorize();
        }

        private void btnNormal_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void btnBilling_Click(object sender, RoutedEventArgs e)
        {
            btnBilling.Visibility = Visibility.Hidden;
            btnBilling2.Visibility = Visibility.Visible;
            stkBilling.Visibility = Visibility.Visible;
        }

        private void btnBilling2_Click(object sender, RoutedEventArgs e)
        {
            if (txtChartNumber.Text.Length != 0 && dtpBillingDate != null && cmbxOperator.SelectedItem != null)
            {
                var date = Convert.ToDateTime(dtpBillingDate.Text).ToString("yyyy-MM-dd");
                Billing results = new Billing(txtChartNumber.Text, cmbxOperator.Text, date); ;

                results.Show();

                UT.AddToAchCounter("Billings");

                int Counter = UT.GetCounterValue("Billings");
                lblBillings.Content = "Billing Checks " + "\n" + Counter.ToString();
            }
            else
            {
                MessageBox.Show("Required Field Not Complete");
            }
        }

        private void btnClientLookup_Click(object sender, RoutedEventArgs e)
        {
            BlankWindow BW = new BlankWindow("");
            BW.Show();
        }

        private void btnPhoneLookup_Click(object sender, RoutedEventArgs e)
        {
            BlankWindow BW = new BlankWindow("");
            BW.Show();
        }

        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            btnBilling.Visibility = Visibility.Visible;
            btnBilling2.Visibility = Visibility.Hidden;
        }

        private void btnSaveSig_Click(object sender, RoutedEventArgs e)
        {
            if (txtSignature.Text != null)
            {
                if (UT.CheckSig())
                {
                    UT.EditSignature(txtSignature.Text, chkbxIncludeSignature.IsChecked.Value);
                    MessageBox.Show("Signature Updated");
                }
                else
                {
                    UT.AddSignatureToTemplatesXML(txtSignature.Text, chkbxIncludeSignature.IsChecked.Value);
                    MessageBox.Show("Signature Added");
                }
            }
            else
            {
                MessageBox.Show("No Signature entered, please enter a signature.");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (stkpnlSignature.Height == 0)
            {
                Storyboard sb = (this.FindResource("SignaturePanelOpen") as Storyboard);
                sb.Begin();
            }
            else
            {
                Storyboard sb = (this.FindResource("SignaturePanelClose") as Storyboard);
                sb.Begin();
            }
        }

        private void btnESearch_Click(object sender, RoutedEventArgs e)
        {
            BlankWindow BW = new BlankWindow("");
            BW.Show();
        }

        private void btnStaffInfo_Click(object sender, RoutedEventArgs e)
        {
            BlankWindow BW = new BlankWindow("");
            BW.Show();
        }

        //WINDOW EVENTS

        private void topbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        //IMAGE EVENTS

        private void imgNewNote_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            New_EditTemplate NewTemplate = new New_EditTemplate("Add");

            NewTemplate.ShowDialog();

            int Counter = UT.GetCounterValue("TemplatesCreate");

            lblTemplatesAdded.Content = "Templates Created " + "\n" + Counter.ToString();
        }

        private void imgNewNote_Copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string TktNo = lblTktNo.Text;

            if (cmbxTemplates.SelectedItem != null)
            {
                List<Template> templates = UT.GetXMLTagValues();
                foreach (Template T in templates)
                {
                    if (T.TemplateID == lblTempID.Content.ToString())
                    {
                        New_EditTemplate Edit = new New_EditTemplate("Edit", T);
                        Edit.ShowDialog();
                        break;
                    }
                }
                templates.Clear();
            }
            else
            {
                MessageBox.Show("No Template Selected");
            }

            int Counter = UT.GetCounterValue("TemplatesEdit");
            lblTemplatesAdded1.Content = "Templates Edited " + "\n" + Counter.ToString();
        }

        private void imgCategorize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int Counter = 0;

            if (tktno == lblTktNo.Text)
            {
                categorize();
            }
            else
            {
                categorize();

                UT.AddToAchCounter("Categorization");

                tktno = lblTktNo.Text;

                Counter = UT.GetCounterValue("Categorization");
                lblCategorized.Content = "Categorized " + "\n" + Counter.ToString();
            }

            if (Counter == 1)
            {
                txtblkAcheivementtxt.Text = "Categorize your first Ticket!";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 5)
            {
                txtblkAcheivementtxt.Text = "Categorize 5 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 10)
            {
                txtblkAcheivementtxt.Text = "Categorize 10 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 20)
            {
                txtblkAcheivementtxt.Text = "Categorize 20 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 30)
            {
                txtblkAcheivementtxt.Text = "Categorize 30 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 40)
            {
                txtblkAcheivementtxt.Text = "Categorize 40 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 50)
            {
                txtblkAcheivementtxt.Text = "Categorize 50 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 60)
            {
                txtblkAcheivementtxt.Text = "Categorize 60 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 70)
            {
                txtblkAcheivementtxt.Text = "Categorize 70 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 80)
            {
                txtblkAcheivementtxt.Text = "Categorize 80 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 90)
            {
                txtblkAcheivementtxt.Text = "Categorize 90 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 100)
            {
                txtblkAcheivementtxt.Text = "Categorize 100 Tickets!!!";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 200)
            {
                txtblkAcheivementtxt.Text = "Categorize 200 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 300)
            {
                txtblkAcheivementtxt.Text = "Categorize 300 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 400)
            {
                txtblkAcheivementtxt.Text = "Categorize 400 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 500)
            {
                txtblkAcheivementtxt.Text = "Categorize 500 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 600)
            {
                txtblkAcheivementtxt.Text = "Categorize 600 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 700)
            {
                txtblkAcheivementtxt.Text = "Categorize 700 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 800)
            {
                txtblkAcheivementtxt.Text = "Categorize 800 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 900)
            {
                txtblkAcheivementtxt.Text = "Categorize 900 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 1000)
            {
                txtblkAcheivementtxt.Text = "Categorize 1000 Tickets!!!";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 1500)
            {
                txtblkAcheivementtxt.Text = "Categorize 1500 Tickets!!!";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 2000)
            {
                txtblkAcheivementtxt.Text = "Categorize 2000 Tickets!!!";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 2500)
            {
                txtblkAcheivementtxt.Text = "Categorize 2500 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 3000)
            {
                txtblkAcheivementtxt.Text = "Categorize 3000 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 3500)
            {
                txtblkAcheivementtxt.Text = "Categorize 3500 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 4000)
            {
                txtblkAcheivementtxt.Text = "Categorize 4000 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 4500)
            {
                txtblkAcheivementtxt.Text = "Categorize 4500 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 5000)
            {
                txtblkAcheivementtxt.Text = "Categorize 5000 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 5500)
            {
                txtblkAcheivementtxt.Text = "Categorize 5500 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 6000)
            {
                txtblkAcheivementtxt.Text = "Categorize 6000 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 6500)
            {
                txtblkAcheivementtxt.Text = "Categorize 6500 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 7000)
            {
                txtblkAcheivementtxt.Text = "Categorize 7000 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 7500)
            {
                txtblkAcheivementtxt.Text = "Categorize 7500 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 8000)
            {
                txtblkAcheivementtxt.Text = "Categorize 8000 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 8500)
            {
                txtblkAcheivementtxt.Text = "Categorize 8500 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 9000)
            {
                txtblkAcheivementtxt.Text = "Categorize 9000 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
            if (Counter == 10000)
            {
                txtblkAcheivementtxt.Text = "Categorize 10000 Tickets";
                Storyboard sb = (this.FindResource("AcheivementPopup") as Storyboard);
                sb.Begin();
            }
        }

        private void imgInsert_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int Counter;
            if (state == Utils.TktState.Used)
            {
                checkCustomTemplate();
            }
            else
            {
                checkCustomTemplate();

                state = Utils.TktState.Used;

                Counter = UT.GetCounterValue("TemplatesInsert");
                lblTemplateInserts.Content = "Templates Inserted " + "\n" + Counter.ToString();
            }
        }

        //THEMES

        private void imgThemeCustom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorPickerPage cpp = new ColorPickerPage();
            cpp.ShowDialog();
        }

        private void imgThemeDark_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UT.ChangeTheme("#FF3F3F3F", "#FFCD5C5C", "#FFFFFF", "Segoe UI");
        }

        private void imgThemeLight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UT.ChangeTheme("#FFD3D3D3", "#FF6A5ACD", "#000000", "Segoe UI");
        }

        private void imgThemeCream_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (stkThemes.Width == 0)
            {
                Storyboard sb = (this.FindResource("ThemeButtonOpen") as Storyboard);
                sb.Begin();
            }
            else
            {
                Storyboard sb = (this.FindResource("ThemeButtonClose") as Storyboard);
                sb.Begin();
            }
        }

        //BROWSER NAVIGATION

        private void imgHome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            wbWHD.Navigate(@"");
        }

        private void imgBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (wbWHD.CanGoBack)
            {
                wbWHD.GoBack();
            }
        }

        private void imgForward_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (wbWHD.CanGoForward)
            {
                wbWHD.GoForward();
            }
        }

        //UI CONTROLS

        private void imgLogout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window1 login = new Window1();
            login.Show();
            this.Close();
        }

        //WEBBROWSER EVENTS

        private void wbWHD_Navigated(object sender, NavigationEventArgs e)
        {
            string html = wbWHD.Source.ToString();

            if (html != null)
            {
                if (UT.IsThisATicket(html))
                {
                    fillCMBXs();
                }
            }
        }

        private void wbWHD_LoadCompleted(object sender, NavigationEventArgs e)
        {
            HTMLDocument doc = (mshtml.HTMLDocument)wbWHD.Document;

            var UN = doc.getElementById("userName");
            var PW = doc.getElementById("Password");
            IHTMLElementCollection btn = doc.getElementsByName("");
            IHTMLElement badlogin = doc.getElementById("");

            if (UN != null)
            {
                UN.innerText = Username;
            }
            if (PW != null)
            {
                PW.innerText = UT.DecodeFrom64(Password);
            }

            if (badlogin != null && UN != null)
            {
                MessageBox.Show("Incorrect Password");

                this.Close();
            }
            else
            {
                foreach (IHTMLElement selects in btn)
                {
                    if (selects.className == ("aquaButtonLink") && selects.id == null)
                    {
                        selects.click();
                        break;
                    }
                }
            }

            DispatcherTimer animation = new DispatcherTimer();
            animation.Interval = TimeSpan.FromSeconds(5);
            animation.Tick += timer_Tick2;
            animation.Start();
        }

        private void wbWHD_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    grdLoader.Visibility = Visibility.Visible;
                    Storyboard sb = (this.FindResource("LoadingScreen") as Storyboard);
                    sb.Begin();
                }));// Dispatcher end
            };
            bw.RunWorkerAsync(100);
            bw.Dispose();
        }

        //COMBOBOX EVENTS

        private void cmbxTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Template> Templates = UT.GetXMLTagValues();
            foreach (Template t in Templates)
            {
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
                if (t.TemplateName == cmbxTemplates.SelectedItem as string)
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
                {
                    if (t.Location.Length != 0)
                    {
                        cmbxLocation.SelectedItem = t.Location;
                    }
                    else
                    {
                        cmbxLocation.SelectedItem = null;
                    }
                    if (t.AssignedTech.Length != 0)
                    {
                        cmbxAssTech.SelectedItem = t.AssignedTech;
                    }
                    else
                    {
                        cmbxAssTech.SelectedItem = t.AssignedTech;
                    }
                    if (t.RequestType.Length != 0)
                    {
                        cmbxRequestType.SelectedItem = t.RequestType;
                    }
                    else
                    {
                        cmbxRequestType.SelectedItem = null;
                    }
                    if (t.Status.Length != 0)
                    {
                        cmbxStatus.SelectedItem = t.Status;
                    }
                    else
                    {
                        cmbxStatus.SelectedItem = null;
                    }
                    if (t.Body.Length != 0)
                    {
                        txtBody.Text = t.Body;
                    }
                    else
                    {
                        txtBody.Text = null;
                    }

                    lblTempID.Content = t.TemplateID;
                }

#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if (cmbxTemplates.SelectedItem as string == "")
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                {
                    cmbxLocation.SelectedItem = null;
                    cmbxAssTech.SelectedItem = null;
                    cmbxRequestType.SelectedItem = null;
                    cmbxStatus.SelectedItem = null;
                    txtBody.Text = null;
                    lblTempID.Content = null;
                }
            }
        }

        //BACKGROUND WORKER EVENTS

        public void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            TemplatesImport();
        }

        private static void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        //TIMER EVENTS

        public void timer_Tick(object sender, EventArgs e)
        {
            TemplatesImport();

            if (backgroundWorker.IsBusy)
            {
            }
            else
            {
            }
        }

        public void timer_Tick2(object sender, EventArgs e)
        {
            grdLoader.Visibility = Visibility.Hidden;
        }

        public void ThemeTimer_Tick(object sender, EventArgs e)
        {
            checkTheme();

            TemplateVisibilitySetter();

            TopbarButtonMover();
        }

        public void TemplateTimer_Tick(object sender, EventArgs e)
        {
            List<Template> Templates = UT.GetXMLTagValues();

            List<Template> Temp = new List<Template>();
            Temp = UT.GetXMLTagValues();
            if (Temp != Templates)
            {
                Templates.Clear();
                Templates = UT.GetXMLTagValues();

                MessageBox.Show("Templates refreshed Temp Count:" + Templates.Count.ToString());
            }
            Templates.Clear();
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            int Counter;
        start:
            if (UT.DoesAchTagExistXML())
            {
                Counter = UT.GetCounterValue("Categorization");
                lblCategorized.Content = "Categorized " + "\n" + Counter.ToString();

                Counter = UT.GetCounterValue("Billings");
                lblBillings.Content = "Billing Checks " + "\n" + Counter.ToString();

                Counter = UT.GetCounterValue("TemplatesCreate");
                lblTemplatesAdded.Content = "Templates Created " + "\n" + Counter.ToString();

                Counter = UT.GetCounterValue("TemplatesEdit");
                lblTemplatesAdded1.Content = "Templates Edited " + "\n" + Counter.ToString();

                Counter = UT.GetCounterValue("TemplatesInsert");
                lblTemplateInserts.Content = "Templates Inserted " + "\n" + Counter.ToString();
            }
            else
            {
                UT.CreateAchTags();
                goto start;
            }
        }

        private void btnLive_Click(object sender, RoutedEventArgs e)
        {
            BlankWindow BW = new BlankWindow("");
            BW.Show();
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}