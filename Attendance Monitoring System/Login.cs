using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Attendance_Monitoring_System
{
    public partial class Login : Form
    {
        Database db = new Database();
        DataTable dt = new DataTable();
        public static int role = 0; // 1=>admin
        public Login()
        {
            InitializeComponent();
        }

        private void Login_btn_Click(object sender, EventArgs e)
        {
            string name = Name_txt.Text;
            string password = Password_txt.Text;

            if(name == "Admin" && password == "Admin")
            {
                role = 1;
                Home home = new Home();
                home.ShowDialog();
                this.Close();
            }
            else
            {
                dt = db.read_data("SELECT * from Teacher WHERE name = '" + name + "'AND password = '" +password + "'", "");
                if (dt.Rows.Count > 0)
                {
                    Home home = new Home();
                    home.ShowDialog();
                    this.Close();
                }
                else
                    MessageBox.Show("Name or password may be incorrect", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
