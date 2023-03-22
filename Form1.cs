using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace tSwitch
{
    public partial class Form1 : Form
    {
        private string _connectionStringXpath = Properties.Settings.Default.TSVConfigXPath;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtSQLServer.Text = $"\\\\{Properties.Settings.Default.SQLDataSource}";
            lbDatabases.DataSource = DatabaseList();
        }

        private List<string> DatabaseList()
        {

            List<string> databaseList = new List<string>();
            string sqlDataSource = Properties.Settings.Default.SQLDataSource;
            string sqlPassword = Properties.Settings.Default.SQLPassword;

            string connectionString = $"Data Source={sqlDataSource};Initial Catalog=master;User ID=sa;Password={sqlPassword};Connection Timeout=300";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT name FROM sys.databases where name like 'TSV%' ", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    databaseList.Add(reader[0].ToString());
                }
            }
            return databaseList;
        
        }

        private string getCurrentDatabase()
        {
            // Load the xml file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Properties.Settings.Default.TSVConfig);

            //get the "add" element wiht key = "ConnectionString"
            XmlNode xmlNode = xmlDoc.SelectSingleNode(_connectionStringXpath);

            string currentString = xmlNode.Attributes["value"].Value;

            foreach (string s in currentString.Split(';'))
            {
                if (s.IndexOf("Catalog") > 0)
                    return s.Split('=')[1].ToString();
            }
            return "";
        }

        private void lbDatabases_DoubleClick(object sender, EventArgs e)
        {
            string selectedDatabase = lbDatabases.SelectedValue.ToString();
            string currentDatabase = getCurrentDatabase();
            if (MessageBox.Show($"Switch TSV database [{currentDatabase}] to [{selectedDatabase}]?", "Swtich TSV database", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
               UpdateTSVConfig(currentDatabase, selectedDatabase);           
        }

        private void UpdateTSVConfig(string currentDb, string selectedDb)
        {
            // Load the xml file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Properties.Settings.Default.TSVConfig);

            //get the "add" element wiht key = "ConnectionString"
            XmlNode xmlNode = xmlDoc.SelectSingleNode(_connectionStringXpath);

            string currentString = xmlNode.Attributes["value"].Value;
            string newString = currentString.Replace(currentDb, selectedDb);
            xmlNode.Attributes["value"].Value = newString;
            try
            {
                xmlDoc.Save(Properties.Settings.Default.TSVConfig);
                MessageBox.Show($"Current database is set to: [{selectedDb}]. Please restart TSV", "TSV DB switched successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured while updating the TSV configuration file: {ex.Message}");
            }
            

        }
    }
}
