using System;
using System.Data;
using System.Data.Odbc;
using System.Windows;
using System.Windows.Input;

namespace WHD_Assistant_WPF
{
    /// <summary>
    /// Interaction logic for Billing.xaml
    /// </summary>
    public partial class Billing : Window
    {
        public Billing(string patid, string modifier, string date)
        {
            InitializeComponent();
            PatID = patid;
            Date = date;
            Modifier = modifier;
            connect();
        }

        public string PatID = "";
        public string Modifier = "";
        public string Date = "";

        public void connect()
        {
            try
            {

                string conn_string = "DSN=Production;Trusted_Connection=yes;";
  
                OdbcConnection PMConn = new OdbcConnection(conn_string);
                PMConn.Open();

                string sqlString = @"select h.patid
                    , h.v_patient_name
                    , h.v_service_value
                    

                    from system..... h
                    inner join system.... d
                    on d.patid = h.patid and = h.JOIN_TO_TX_HISTORY
                    where
                    h. patid = '{0}'
                    and h.date_of_service {1} '{2}'
                    order by h.ID";
                sqlString = String.Format(sqlString, Pat, Modifier, Date);
                System.Data.Odbc.OdbcCommand cmd99 = new System.Data.Odbc.OdbcCommand(sqlString, PMConn);
                System.Data.Odbc.OdbcDataReader dreader99 = cmd99.ExecuteReader();

                string patID = "";
                string v_service_value = "";
                DataTable table = new DataTable();

                table.Columns.Add("ClientName", typeof(string));                    //1
                table.Columns.Add("Service Value", typeof(string));                 //5
                

                while (dreader99.Read())
                {
                    patID = dreader99["patid"].ToString();
                    v_service_value = dreader99["v_service_value"].ToString();

                    table.Rows.Add(
                        patID,                          //1
                        v_service_value,                //5

                    dgBilling.ItemsSource = table.AsDataView();
                }
                PMConn.Close();
                PMConn.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void rctTop_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}