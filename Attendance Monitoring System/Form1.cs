using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Attendance_Monitoring_System
{
    public partial class Home : Form
    {
        Database db = new Database();
        public Home()
        {
            InitializeComponent();
        }

        private void AddTeacherToDB_btn_Click(object sender, EventArgs e)
        {
            bool checker = true;

            string name = Name_txt.Text;
            string password = "";

            if (name == "")
            {
                MessageBox.Show("Enter a name", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checker = false;
            }

            if (Password_txt.Text == "")
            {
                MessageBox.Show("Enter a password", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checker = false;
            }

            if (Password_txt.Text != ConfirmPassword_txt.Text)
            {
                MessageBox.Show("Confirm password doesn't match the password", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checker = false;
            }
            else
                password = Password_txt.Text;

            if (checker)
            {
                string stmt = "INSERT INTO Teacher VALUES ('" + name + "', '" + password + "');";
                db.execute_data(stmt, "Added successfully");
            }
        }
    }
}
