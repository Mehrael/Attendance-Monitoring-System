using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Attendance_Monitoring_System
{
    public partial class Home : Form
    {
        Database db = new Database();
        DataTable dt = new DataTable();
        public Home()
        {
            InitializeComponent();
        }
        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        private void AddTeacherToDB_btn_Click(object sender, EventArgs e)
        {
            bool checker = true;

            string name = Name_txt.Text;
            string password = "";

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
                Home_Load(sender, e);
            }
        }

        private void AddTeacher_btn_Click(object sender, EventArgs e)
        {
            AddTeacher_panel.BringToFront();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            if (Login.role)           //Admin
            {
                ViewCourses_btn.Enabled = false;
                AddStudentToCourse_btn.Enabled = false;
                TakeAttendance_btn.Enabled = false;
                AttendanceReport_btn.Enabled = false;
            }
            else
            {
                AddStudents_btn.Visible = false;
                AddCourses_btn.Visible = false;
                AddTeacher_btn.Visible = false;
                AssignTeacherCourse_btn.Visible = false;

                dt.Clear();
                dt = db.read_data("SELECT * FROM Teacher", "");

                Teacher_cmbx.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ComboboxItem item = new ComboboxItem();

                    item.Text = dt.Rows[i][1].ToString();
                    item.Value = (int)dt.Rows[i][0];
                    Teacher_cmbx.Items.Add(item);
                }
                ////////////////////////////////////////////////

                dt.Clear();
                dt = db.read_data("SELECT * FROM Courses WHERE TeacherID = " + Login.UserID, "");

                Teacher_courses_cmbx.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ComboboxItem item = new ComboboxItem();

                    item.Text = dt.Rows[i][1].ToString();
                    item.Value = (int)dt.Rows[i][0];
                    Teacher_courses_cmbx.Items.Add(item);
                }
                ////////////////////////////////////////////////

                dt.Clear();
                dt = db.read_data("SELECT * FROM Student", "");

                Student_course_cmbx.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ComboboxItem item = new ComboboxItem();

                    item.Text = dt.Rows[i][1].ToString();
                    item.Value = (int)dt.Rows[i][0];
                    Student_course_cmbx.Items.Add(item);
                }

            }
        }
        private void AddCourseToDB_btn_Click(object sender, EventArgs e)
        {
            string TeacherID = (Teacher_cmbx.SelectedItem as ComboboxItem).Value.ToString();
            string Subject = Subject_txt.Text;

            string stmt = "INSERT INTO Courses VALUES ('" + Subject + "'," + TeacherID + ");";
            db.execute_data(stmt, "Added successfully");
        }

        private void AddCourses_btn_Click(object sender, EventArgs e)
        {
            AddCourses_panel.BringToFront();
        }

        private void AddStudentToDB_btn_Click(object sender, EventArgs e)
        {
            string name = StudentName_txt.Text;
            string dep = Department_cmbx.SelectedItem.ToString();
            string seat = SeatNum_txt.Text;

            string stmt = "INSERT INTO Student VALUES ('" + name + "','" + dep + "','" + seat + "');";
            db.execute_data(stmt, "Added successfully");
        }

        private void AddStudents_btn_Click(object sender, EventArgs e)
        {
            AddStudent_panel.BringToFront();
        }

        private void Add_Student_Course_btn_Click(object sender, EventArgs e)
        {
            string courseID = (Teacher_courses_cmbx.SelectedItem as ComboboxItem).Value.ToString();
            string studentID = (Student_course_cmbx.SelectedItem as ComboboxItem).Value.ToString();

            string stmt = "SELECT * FROM StudentCourses WHERE CourseID = " + courseID + "AND studentID = " + studentID;
            DataTable tbl = new DataTable();
            tbl = db.read_data(stmt, "");

            if(tbl.Rows.Count > 0)
                MessageBox.Show("This student already enrolled in this course","",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            else
            {
                stmt = "INSERT INTO StudentCourses VALUES(" + studentID + "," + courseID + ")";
                db.execute_data(stmt, "Added Successfully");
            }
        }
    }
}
