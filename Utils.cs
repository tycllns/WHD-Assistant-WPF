using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WHD_Assistant_WPF
{
    public class Utils
    {
        public enum TktState
        {
            New,
            Used
        }

        public Boolean IsNewOrEditOpen
        { get; set; }

        public string UserNameValue()
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            XmlNode root = doc.SelectSingleNode("Login");
            XmlNode Username = root.SelectSingleNode("Username");

            string UN = Username.InnerText;

            return UN;
        }

        public string PasswordValue()
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            XmlNode root = doc.SelectSingleNode("Login");
            XmlNode Password = root.SelectSingleNode("Password");

            string PW = Password.InnerText;

            return PW;
        }

        public void SidePanelManipulator(Grid grd, Button btn, bool open, int col)
        {
            if (open == true)
            {
                grd.ColumnDefinitions[col].MinWidth = 150;
                grd.ColumnDefinitions[col].Width = new GridLength(300);
                btn.IsEnabled = false;
                btn.Background = Brushes.LightGray;
            }
            else
            {
                grd.ColumnDefinitions[col].MinWidth = 0;
                grd.ColumnDefinitions[col].Width = new GridLength(0);
                btn.IsEnabled = true;
                btn.Background = Brushes.IndianRed;
            }
        }

        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser)
                .GetField("_axIWebBrowser2",
                          BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember(
                "Silent", BindingFlags.SetProperty, null, objComWebBrowser,
                new object[] { Hide });
        }

        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Error in base64Encode" + ex.Message);
            }
        }

        //this function Convert to Decode your Password

        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        public string readUpdateVersion()
        {
            string text = "Version:" + System.IO.File.ReadAllText(@"C:\Redrock\WHD Assistant\UpdateVersion.txt");
            return text;
        }

        public void ConnectToOL()
        {
            List<string> lstAllRecipients = new List<string>();
            //Below is hardcoded - can be replaced with db data

            Outlook.Application outlookApp = new Outlook.Application();
            Outlook._MailItem oMailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
            Outlook.Inspector oInspector = oMailItem.GetInspector;

            // Thread.Sleep(10000);

            // Recipient
            Outlook.Recipients oRecips = (Outlook.Recipients)oMailItem.Recipients;
            foreach (String recipient in lstAllRecipients)
            {
                Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(recipient);
                oRecip.Resolve();
            }

            //Add CC
            oCCRecip.Type = (int)Outlook.OlMailRecipientType.olCC;
            oCCRecip.Resolve();

            //Add Subject
            oMailItem.Subject = "Test Mail";

            // body, bcc etc...

            //Display the mailbox
            oMailItem.Display(true);

            //Response.Write(objEx.ToString());
        }

        //Objects and Variables

        public XmlDocument doc = new XmlDocument();

        private List<Template> Templates = new List<Template>();

        //Methods

        public void CheckForXMLFile()
        {
            string curFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml";
            string curLoginFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml";
            string curThemeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Theme.xml";
            string curCatFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Categories.xml";

            if (File.Exists(curFile))
            {
            }
            else
            {
                CreateXML();
            }

            if (File.Exists(curLoginFile))
            {
            }
            else
            {
                CreateLoginXML();
            }

            if (File.Exists(curThemeFile))
            {
            }
            else
            {
                CreateThemeXML();
            }

            if (File.Exists(curCatFile))
            {
            }
            else
            {
                CreateCategoriesXML();
            }
        }

        private static void CreateLoginXML()
        {
            XDocument login = new XDocument();

            login.Add(new XElement("Login", new XElement("Username"), new XElement("Password"), new XElement("RememberMe")));

            login.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
        }

        private static void CreateThemeXML()
        {
            XDocument login = new XDocument();

            login.Add(new XElement("Theme", new XElement("TopBar"), new XElement("MainForm"), new XElement("Buttons"), new XElement("FontColor"), new XElement("FontStyle")));

            login.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Theme.xml");
        }

        private static void CreateXML()
        {
            XDocument doc = new XDocument();

            doc.Add(new XElement("Templates"));

            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
        }

        private static void CreateCategoriesXML()
        {
            XDocument doc = new XDocument();

            doc.Add(new XElement("Categories", new XElement("Locations"), new XElement("Statuses"), new XElement("RequestTypes"), new XElement("AssignedTechs")));

            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Categories.xml");
        }

        public void AddRecordToXML(string TName, string Status, string Location, string AssignedTech, string Type, string Body)
        {
            List<string> IDs = TempIDs();

            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
            XmlNode root = doc.SelectSingleNode("Templates");
            XmlElement Template = doc.CreateElement("Template");
            root.AppendChild(Template);

            for (int i = 0; i < 1000; i++)
            {
                if (IDs.Contains(i.ToString()))
                {
                }
                else
                {
                    XmlAttribute id = doc.CreateAttribute("id");
                    id.Value = i.ToString();
                    Template.Attributes.Append(id);
                    break;
                }
            }

            XmlElement TemplateName = doc.CreateElement("TemplateName");
            TemplateName.InnerText = TName;
            Template.AppendChild(TemplateName);

            XmlElement TStatus = doc.CreateElement("Status");
            TStatus.InnerText = Status;
            Template.AppendChild(TStatus);

            XmlElement TLocation = doc.CreateElement("Location");
            TLocation.InnerText = Location;
            Template.AppendChild(TLocation);

            XmlElement TAssignedTech = doc.CreateElement("AssignedTech");
            TAssignedTech.InnerText = AssignedTech;
            Template.AppendChild(TAssignedTech);

            XmlElement TType = doc.CreateElement("RequestType");
            TType.InnerText = Type;
            Template.AppendChild(TType);

            XmlElement TBody = doc.CreateElement("Body");
            TBody.InnerText = Body;
            Template.AppendChild(TBody);

            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
        }

        public void AddSignatureToTemplatesXML(string SBody, bool Checked)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
            XmlNode root = doc.SelectSingleNode("Templates");

            if (root != null)
            {
                XmlElement Signature = doc.CreateElement("Signature");
                root.AppendChild(Signature);

                XmlElement SigBody = doc.CreateElement("SigBody");
                SigBody.InnerText = SBody;
                Signature.AppendChild(SigBody);

                XmlElement IsChecked = doc.CreateElement("IsChecked");
                IsChecked.InnerText = Checked.ToString();
                Signature.AppendChild(IsChecked);

                doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
            }
            else
            {
                XmlElement Signature = doc.CreateElement("Signature");
                root.AppendChild(Signature);

                XmlElement SigBody = doc.CreateElement("SigBody");
                SigBody.InnerText = SBody;
                Signature.AppendChild(SigBody);

                XmlElement IsChecked = doc.CreateElement("IsChecked");
                IsChecked.InnerText = Checked.ToString();
                Signature.AppendChild(IsChecked);

                doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
            }
        }

        public void EditSignature(string SBody, bool Checked)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
            XmlNode root = doc.SelectSingleNode("//Signature");

            if (root != null)
            {
                root.SelectSingleNode("//SigBody").InnerText = SBody;
                root.SelectSingleNode("//IsChecked").InnerText = Checked.ToString();

                doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
            }
        }

        public void CreateAchTags()
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");

            XmlNode root = doc.SelectSingleNode("Login");

            XmlElement CountersCreate = doc.CreateElement("Achievements");
            root.AppendChild(CountersCreate);

            XmlElement BCounterCreate = doc.CreateElement("Billings");
            BCounterCreate.InnerText = "0";
            CountersCreate.AppendChild(BCounterCreate);

            XmlElement TICounterCreate = doc.CreateElement("TemplateInserts");
            TICounterCreate.InnerText = "0";
            CountersCreate.AppendChild(TICounterCreate);

            XmlElement TACounterCreate = doc.CreateElement("TemplateAdds");
            TACounterCreate.InnerText = "0";
            CountersCreate.AppendChild(TACounterCreate);

            XmlElement CCounterCreate = doc.CreateElement("TemplateEdits");
            CCounterCreate.InnerText = "0";
            CountersCreate.AppendChild(CCounterCreate);

            XmlElement TECounterCreate = doc.CreateElement("Categorizations");
            TECounterCreate.InnerText = "0";
            CountersCreate.AppendChild(TECounterCreate);

            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
        }

        public Boolean DoesAchTagExistXML()
        {
        Start:
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");

            XmlNode root = doc.SelectSingleNode("Login");

            XmlNode Counters = doc.SelectSingleNode("//Achievements");

            if (Counters != null)
            {
                Debug.WriteLine("Achievements found");
                return true;
            }
            else
            {
                Debug.WriteLine("Achievements not found");
                CreateAchTags();
                goto Start;
            }
        }

        public void AddToAchCounter(string Type)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");

            int Counter;

            XmlNode BCounter = doc.SelectSingleNode("//Billings");
            XmlNode TICounter = doc.SelectSingleNode("//TemplateInserts");
            XmlNode TACounter = doc.SelectSingleNode("//TemplateAdds");
            XmlNode TECounter = doc.SelectSingleNode("//TemplateEdits");
            XmlNode CCounter = doc.SelectSingleNode("//Categorizations");

            if (Type == "Categorization")
            {
                Counter = Int32.Parse(CCounter.InnerText);
                Counter = 1 + Counter;
                CCounter.InnerText = Counter.ToString();
                doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            }
            if (Type == "Billings")
            {
                Counter = Int32.Parse(BCounter.InnerText);
                Counter = 1 + Counter;
                BCounter.InnerText = Counter.ToString();
                doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            }
            if (Type == "TemplatesCreate")
            {
                Counter = Int32.Parse(TACounter.InnerText);
                Counter = 1 + Counter;
                TACounter.InnerText = Counter.ToString();
                doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            }
            if (Type == "TemplatesEdit")
            {
                Counter = Int32.Parse(TECounter.InnerText);
                Counter = 1 + Counter;
                TECounter.InnerText = Counter.ToString();
                doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            }
            if (Type == "TemplatesInsert")
            {
                Counter = Int32.Parse(TICounter.InnerText);
                Counter = 1 + Counter;
                TICounter.InnerText = Counter.ToString();
                doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            }
        }

        public int GetCounterValue(string Type)
        {
        start:

            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            XmlNode root = doc.SelectSingleNode("Login");

            int Counter = 0;

            XmlNode BCounter = doc.SelectSingleNode("//Billings");
            XmlNode TICounter = doc.SelectSingleNode("//TemplateInserts");
            XmlNode TACounter = doc.SelectSingleNode("//TemplateAdds");
            XmlNode TECounter = doc.SelectSingleNode("//TemplateEdits");
            XmlNode CCounter = doc.SelectSingleNode("//Categorizations");

            if (DoesAchTagExistXML())
            {
                if (Type == "Categorization")
                {
                    Counter = Int32.Parse(CCounter.InnerText);
                }

                if (Type == "Billings")
                {
                    Counter = Int32.Parse(BCounter.InnerText);
                }

                if (Type == "TemplatesCreate")
                {
                    Counter = Int32.Parse(TACounter.InnerText);
                }

                if (Type == "TemplatesEdit")
                {
                    Counter = Int32.Parse(TECounter.InnerText);
                }

                if (Type == "TemplatesInsert")
                {
                    Counter = Int32.Parse(TICounter.InnerText);
                }

                return Counter;
            }
            else
            {
                CreateAchTags();
                goto start;
            }
        }

        public Boolean CheckSig()
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");

            XmlNode Signature = doc.SelectSingleNode("//Signature");

            if (Signature != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddListToXML(List<string> L, string type)
        {
            Debug.WriteLine("Trying to run AddListToXML");
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Categories.xml");
            XmlNode root = doc.SelectSingleNode("Categories");

            XmlNode Locations = doc.SelectSingleNode("Categories/Locations");
            XmlNode RequestTypes = doc.SelectSingleNode("Categories/RequestTypes");
            XmlNode AssignedTechs = doc.SelectSingleNode("Categories/AssignedTechs");
            XmlNode Statuses = doc.SelectSingleNode("Categories/Statuses");

            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Categories.xml");

            if (type == "Locations")
            {
                Debug.WriteLine($"Adding Nodes to Locations");

                if (Locations != null)
                {
                    Locations.RemoveAll();

                    foreach (string item in L)
                    {
                        Debug.WriteLine($"Adding {item} to Locations");
                        XmlElement Location = doc.CreateElement("Location");
                        Location.InnerText = item;
                        Locations.AppendChild(Location);
                    }
                    XmlElement Location2 = doc.CreateElement("Location");
                    Location2.InnerText = "Locations";
                    Locations.AppendChild(Location2);
                }
                else
                {
                    foreach (string item in L)
                    {
                        Debug.WriteLine($"Adding {item} to Locations");
                        XmlElement Location = doc.CreateElement("Location");
                        Location.InnerText = item;
                        Locations.AppendChild(Location);
                    }
                    XmlElement Location2 = doc.CreateElement("Location");
                    Location2.InnerText = "Locations";
                    Locations.AppendChild(Location2);
                }
            }

            if (type == "AssignedTechs")
            {
                if (AssignedTechs != null)
                {
                    AssignedTechs.RemoveAll();

                    foreach (string item in L)
                    {
                        XmlElement AssignedTech = doc.CreateElement("AssignedTech");
                        AssignedTech.InnerText = item;
                        AssignedTechs.AppendChild(AssignedTech);
                    }
                    XmlElement AssignedTech2 = doc.CreateElement("AssignedTech");
                    AssignedTech2.InnerText = "AssignedTechs";
                    AssignedTechs.AppendChild(AssignedTech2);
                }
                else
                {
                    foreach (string item in L)
                    {
                        XmlElement AssignedTech = doc.CreateElement("AssignedTech");
                        AssignedTech.InnerText = item;
                        AssignedTechs.AppendChild(AssignedTech);
                    }
                    XmlElement AssignedTech2 = doc.CreateElement("AssignedTech");
                    AssignedTech2.InnerText = "AssignedTechs";
                    AssignedTechs.AppendChild(AssignedTech2);
                }
            }

            if (type == "Statuses")
            {
                if (Statuses != null)
                {
                    Statuses.RemoveAll();
                    foreach (string item in L)
                    {
                        XmlElement Status = doc.CreateElement("Status");
                        Status.InnerText = item;
                        Statuses.AppendChild(Status);
                    }
                    XmlElement Status2 = doc.CreateElement("Status");
                    Status2.InnerText = "Statuses";
                    Statuses.AppendChild(Status2);
                }
                else
                {
                    foreach (string item in L)
                    {
                        XmlElement Status = doc.CreateElement("Status");
                        Status.InnerText = item;
                        Statuses.AppendChild(Status);
                    }
                    XmlElement Status2 = doc.CreateElement("Status");
                    Status2.InnerText = "Statuses";
                    Statuses.AppendChild(Status2);
                }
            }

            if (type == "RequestTypes")
            {
                if (RequestTypes != null)
                {
                    RequestTypes.RemoveAll();
                    foreach (string item in L)
                    {
                        XmlElement RequestType = doc.CreateElement("RequestType");
                        RequestType.InnerText = item;
                        RequestTypes.AppendChild(RequestType);
                    }
                    XmlElement RequestType2 = doc.CreateElement("RequestType");
                    RequestType2.InnerText = "RequestTypes";
                    RequestTypes.AppendChild(RequestType2);
                }
                else
                {
                    foreach (string item in L)
                    {
                        XmlElement RequestType = doc.CreateElement("RequestType");
                        RequestType.InnerText = item;
                        RequestTypes.AppendChild(RequestType);
                    }
                    XmlElement RequestType2 = doc.CreateElement("RequestType");
                    RequestType2.InnerText = "RequestTypes";
                    RequestTypes.AppendChild(RequestType2);
                }
            }
            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Categories.xml");
        }

        public List<string> RestoreTheme()
        {
            List<string> colors = new List<string>();

            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Theme.xml");

            XmlNodeList TList = doc.GetElementsByTagName("Theme");

            foreach (XmlNode node in TList)
            {
                Template T = new Template();

                string TBC = node.SelectSingleNode("TopBar").InnerText;
                string MainForm = node.SelectSingleNode("MainForm").InnerText;
                string Buttons = node.SelectSingleNode("Buttons").InnerText;
                string FontColor = node.SelectSingleNode("FontColor").InnerText;
                string FontStyle = node.SelectSingleNode("FontStyle").InnerText;

                colors.Add(TBC);
                colors.Add(MainForm);
                colors.Add(Buttons);
                colors.Add(FontColor);
                colors.Add(FontStyle);
            }

            return colors;
        }

        public void ChangeTheme(string mainform, string buttons, string fontcolor, string fontstyle)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Theme.xml");

            XmlNode root = doc.SelectSingleNode("Theme");
            //XmlNode TopBar = root.SelectSingleNode("TopBar");
            XmlNode MainForm = root.SelectSingleNode("MainForm");
            XmlNode Buttons = root.SelectSingleNode("Buttons");
            XmlNode FontColor = root.SelectSingleNode("FontColor");
            XmlNode FontStyle = root.SelectSingleNode("FontStyle");

            //TopBar.InnerText = topbar;
            MainForm.InnerText = mainform;
            Buttons.InnerText = buttons;
            FontColor.InnerText = fontcolor;
            FontStyle.InnerText = fontstyle;

            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Theme.xml");
        }

        public void VerifyLogin(string username, string password, bool remember)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            XmlNode root = doc.SelectSingleNode("Login");
            XmlNode Username = root.SelectSingleNode("Username");
            XmlNode Password = root.SelectSingleNode("Password");

            if (username == Username.InnerText)
            {
            }
            else
            {
                Username.InnerText = username;
            }

            if (password == Password.InnerText)
            {
            }
            else
            {
                Password.InnerText = password;
            }

            if (remember == false)
            {
                Username.InnerText = "";
                Password.InnerText = "";
            }
            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
        }

        public void CreateSavedLogin(string username, string password)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
            XmlNode root = doc.SelectSingleNode("Login");
            XmlNode Username = root.SelectSingleNode("Username");
            XmlNode Password = root.SelectSingleNode("Password");

            Username.InnerText = username;
            Password.InnerText = password;

            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Login.xml");
        }

        public List<List<string>> RetreiveListFromXML()
        {
            try
            {
                doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Categories.xml");

                List<string> LocationsList = new List<string>();
                List<string> AssignedTechsList = new List<string>();
                List<string> RequestTypesList = new List<string>();
                List<string> StatusesList = new List<string>();

                List<List<string>> Lists = new List<List<string>>();

                XmlNodeList Location = doc.GetElementsByTagName("Location");
                XmlNodeList RequestType = doc.GetElementsByTagName("RequestType");
                XmlNodeList Status = doc.GetElementsByTagName("Status");
                XmlNodeList AssignedTech = doc.GetElementsByTagName("AssignedTech");

                foreach (XmlNode node in Location)
                {
                    LocationsList.Add(node.InnerText);
                }
                foreach (XmlNode node in RequestType)
                {
                    RequestTypesList.Add(node.InnerText);
                }
                foreach (XmlNode node in Status)
                {
                    StatusesList.Add(node.InnerText);
                }
                foreach (XmlNode node in AssignedTech)
                {
                    AssignedTechsList.Add(node.InnerText);
                }

                Lists.Add(StatusesList);
                Lists.Add(LocationsList);
                Lists.Add(AssignedTechsList);
                Lists.Add(RequestTypesList);

                return Lists;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public void EditXMLNode(string id, string TName, string Status, string Location, string AssignedTech, string Type, string Body)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");

            XmlNodeList TList = doc.GetElementsByTagName("Template");

            foreach (XmlNode node in TList)
            {
                if (node.Attributes["id"].Value == id)
                {
                    node.SelectSingleNode("TemplateName").InnerText = TName;
                    node.SelectSingleNode("Location").InnerText = Location;
                    node.SelectSingleNode("RequestType").InnerText = Type;
                    node.SelectSingleNode("Status").InnerText = Status;
                    node.SelectSingleNode("AssignedTech").InnerText = AssignedTech;
                    node.SelectSingleNode("Body").InnerText = Body;
                    break;
                }
            }
            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
        }

        public List<string> TempIDs()
        {
            List<Template> Templates = GetXMLTagValues();
            List<string> IDs = new List<string>();

            foreach (Template t in Templates)
            {
                IDs.Add(t.TemplateID);
            }

            return IDs;
        }

        public List<Template> GetXMLTagValues()
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");

            XmlNodeList TList = doc.GetElementsByTagName("Template");

            foreach (XmlNode node in TList)
            {
                Template T = new Template();

                string ID = node.Attributes["id"].Value;
                string TemplateName = node.SelectSingleNode("TemplateName").InnerText;
                string Location = node.SelectSingleNode("Location").InnerText;
                string RequestType = node.SelectSingleNode("RequestType").InnerText;
                string Status = node.SelectSingleNode("Status").InnerText;
                string AssignedTech = node.SelectSingleNode("AssignedTech").InnerText;
                string Body = node.SelectSingleNode("Body").InnerText;

                T.TemplateID = ID;
                T.TemplateName = TemplateName;
                T.Location = Location;
                T.RequestType = RequestType;
                T.Status = Status;
                T.AssignedTech = AssignedTech;
                T.Body = Body;

                Templates.Add(T);
            }

            Debug.WriteLine(Templates.Count);

            return Templates;
        }

        public List<string> GetSignature()
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
            List<string> Signature = new List<string>();

            if (CheckSig() == true)
            {
                string SigBody = doc.SelectSingleNode("//SigBody").InnerText;
                string IsChecked = doc.SelectSingleNode("//IsChecked").InnerText;

                Signature.Add(SigBody);
                Signature.Add(IsChecked);

                return Signature;
            }
            else
            {
                return null;
            }
        }

        public void DeleteXMLNode(string id)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");

            XmlNodeList TList = doc.GetElementsByTagName("Templates");

            foreach (XmlNode node in TList)
            {
                XmlNodeList Children = node.ChildNodes;

                foreach (XmlNode ChildNode in Children)
                {
                    if (ChildNode.Name == "Template" && ChildNode.Attributes["id"].InnerText == id)
                    {
                        node.RemoveChild(ChildNode);
                        break;
                    }
                }
            }
            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
        }

        public void DeleteXMLCatEntry(string id)
        {
            doc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");

            XmlNodeList TList = doc.GetElementsByTagName("Templates");

            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Templates.xml");
        }

        public void LoadHTMLDoc(WebBrowser wb)
        {
            string html = (wb.Document as HtmlAgilityPack.HtmlDocument).DocumentNode.InnerHtml;
            HtmlDocument doc = new HtmlDocument();
            doc.Load(html);
        }

        public Boolean IsThisATicket(string url)
        {
            bool TORF = false;

            if (url != null)
            {
                if (url.Contains("ticket="))
                {
                    TORF = true;
                }
                else
                {
                    TORF = false;
                }
            }

            return TORF;
        }

        public string GetTktNo(string url)
        {
            if (url.Contains("ticket="))
            {
                if (url.Contains("="))
                {
                    string lastFragment = url.Split('=').Last();
                    Debug.WriteLine(lastFragment);
                    return lastFragment;
                }
            }
            return null;
        }
    }
}