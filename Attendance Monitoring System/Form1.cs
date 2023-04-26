using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
        public class courses
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class attendance
        {
            public int actual_id { get; set; }
            public int id { get; set; }
            public string seatNum { get; set; }
            public string student_name { get; set; }
            public bool attended { get; set; }
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
            }
            else
            {
                AddStudents_btn.Visible = false;
                AddCourses_btn.Visible = false;
                AddTeacher_btn.Visible = false;

                List<courses> lst = new List<courses>();

                dt.Clear();
                dt = db.read_data("SELECT * FROM Courses WHERE TeacherID = " + Login.UserID, "");

                Teacher_courses_cmbx.Items.Clear();
                course_name_cmbx.Items.Clear();
                Course_view_attendance_cmbx.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    lst.Add(new courses()
                    {
                        id = i + 1,
                        name = dt.Rows[i][1].ToString(),
                    });

                    ComboboxItem item = new ComboboxItem();

                    item.Text = dt.Rows[i][1].ToString();
                    item.Value = (int)dt.Rows[i][0];
                    Teacher_courses_cmbx.Items.Add(item);
                    course_name_cmbx.Items.Add(item);
                    Course_view_attendance_cmbx.Items.Add(item);
                }
                BindingSource bs = new BindingSource();
                bs.DataSource = lst;
                View_courses_dataGridView.DataSource = bs;
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

            if (tbl.Rows.Count > 0)
                MessageBox.Show("This student already enrolled in this course", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                stmt = "INSERT INTO StudentCourses VALUES(" + studentID + "," + courseID + ")";
                db.execute_data(stmt, "Added Successfully");
            }
        }

        private void AddStudentToCourse_btn_Click(object sender, EventArgs e)
        {
            AddStudentCourse_panel.BringToFront();
        }

        private void ViewCourses_btn_Click(object sender, EventArgs e)
        {
            ViewCourses_panel.BringToFront();
        }

        private void TakeAttendance_btn_Click(object sender, EventArgs e)
        {
            Take_attendance_panel.BringToFront();
        }

        private void course_name_cmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable tbl = new DataTable();

            string query = "SELECT s.id, s.name, s.department, s.seat_number FROM Student s JOIN StudentCourses a ON s.id = a.StudentID WHERE a.CourseID = " + (course_name_cmbx.SelectedItem as ComboboxItem).Value.ToString() + ";";
            tbl = db.read_data(query, "");

            List<attendance> lst = new List<attendance>();
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                lst.Add(new attendance()
                {
                    actual_id = (int)tbl.Rows[i][0],
                    id = i + 1,
                    student_name = tbl.Rows[i][1].ToString(),
                    seatNum = tbl.Rows[i][3].ToString(),
                    attended = false
                });
            }
            BindingSource bs = new BindingSource();
            bs.DataSource = lst;
            Attendance_dataGridView.DataSource = bs;
            Attendance_dataGridView.Columns["actual_id"].Visible = false;

            Attendance_dataGridView.Columns["id"].HeaderText = "#";
            Attendance_dataGridView.Columns["id"].ReadOnly = true;

            Attendance_dataGridView.Columns["student_name"].HeaderText = "Student";
            Attendance_dataGridView.Columns["student_name"].ReadOnly = true;

            Attendance_dataGridView.Columns["seatNum"].HeaderText = "Seat Num";
            Attendance_dataGridView.Columns["seatNum"].ReadOnly = true;

            Attendance_dataGridView.Columns["attended"].HeaderText = "";
        }

        private void Save_attendance_btn_Click(object sender, EventArgs e)
        {
            string stmt;
            bool good = false;
            string courseID = (course_name_cmbx.SelectedItem as ComboboxItem).Value.ToString();

            DateTime today = DateTime.UtcNow.Date;
            string date = today.ToString("yyyy-MM-dd");

            for (int i = 0; i < Attendance_dataGridView.Rows.Count; i++)
            {
                string studentID = Attendance_dataGridView.Rows[i].Cells["actual_id"].Value.ToString();
                string attended = "";
                if ((bool)Attendance_dataGridView.Rows[i].Cells["attended"].Value)
                    attended = "1";
                else
                    attended = "0";

                stmt = "INSERT INTO Attendance (date, CourseID, StudentID, Attended) VALUES ('" + date + "'," + courseID + "," + studentID + "," + attended + ");";

                if (db.execute_data(stmt, ""))
                    good = true;
                else
                    good = false;
            }

            if (good)
                MessageBox.Show("Data Saved");
        }

        private void Course_view_attendance_cmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable tbl = new DataTable();

            string date = dateTimePicker.Value.ToString("yyyy-MM-dd");

            string query = "SELECT s.name, s.seat_number, a.attended FROM Student s JOIN Attendance a ON s.id = a.StudentID WHERE a.CourseID = " + (Course_view_attendance_cmbx.SelectedItem as ComboboxItem).Value.ToString() + " AND a.date = '" + date + "'";
            tbl = db.read_data(query, "");

            List<attendance> lst = new List<attendance>();
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                lst.Add(new attendance()
                {
                    actual_id = 0,
                    id = i + 1,
                    student_name = tbl.Rows[i][0].ToString(),
                    seatNum = tbl.Rows[i][1].ToString(),
                    attended = (bool)tbl.Rows[i][2],
                });
            }
            BindingSource bs = new BindingSource();
            bs.DataSource = lst;
            ViewAttendance_dataGridView.DataSource = bs;
            ViewAttendance_dataGridView.Columns["actual_id"].Visible = false;

            ViewAttendance_dataGridView.Columns["id"].HeaderText = "#";
            ViewAttendance_dataGridView.Columns["id"].ReadOnly = true;

            ViewAttendance_dataGridView.Columns["student_name"].HeaderText = "Student";
            ViewAttendance_dataGridView.Columns["student_name"].ReadOnly = true;

            ViewAttendance_dataGridView.Columns["seatNum"].HeaderText = "Seat Num";
            ViewAttendance_dataGridView.Columns["seatNum"].ReadOnly = true;

            ViewAttendance_dataGridView.Columns["attended"].HeaderText = "";
            ViewAttendance_dataGridView.Columns["attended"].ReadOnly = true;
        }

        private void AttendanceReport_btn_Click(object sender, EventArgs e)
        {
            Attendance_Report_panel.BringToFront();
        }

        private void Save_as_pdf_btn_Click(object sender, EventArgs e)
        {
            string date = dateTimePicker.Value.ToString("yyyy-MM-dd");

            Document doc = new Document();

            PdfWriter.GetInstance(doc, new FileStream((Course_view_attendance_cmbx.SelectedItem as ComboboxItem).Text.ToString() + " " + date + " Attendance.pdf", FileMode.Create));

            doc.Open();
            Paragraph title = new Paragraph((Course_view_attendance_cmbx.SelectedItem as ComboboxItem).Text.ToString() + " " + date + " Attendance Report\n\n", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20f, iTextSharp.text.Font.BOLD));

            title.Alignment = Element.ALIGN_CENTER;

            doc.Add(title);

            PdfPTable table = new PdfPTable(ViewAttendance_dataGridView.Columns.Count-1);

            foreach (DataGridViewColumn column in ViewAttendance_dataGridView.Columns)
            {
                if (column.HeaderText == "actual_id")
                    continue;

                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                table.AddCell(cell);
            }

            foreach (DataGridViewRow row in ViewAttendance_dataGridView.Rows)
            {
                int cellIndex = 0;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if(cellIndex!=0)
                        table.AddCell(cell.Value.ToString());
                    cellIndex++;
                }
            }

            doc.Add(table);
            doc.Close();

            MessageBox.Show("PDF Saved");
        }
    }
}
