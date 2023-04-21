using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace Bug_Tracker
{
    class Database
    {
        SqlConnection conn = new SqlConnection("Data Source=LAPTOP-Mehrael;Initial Catalog=Bug_Tracker;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();

        public DataTable read_data(String stmt , string msg)
        {
            DataTable tbl = new DataTable();  
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = stmt;
                conn.Open();
                tbl.Load(cmd.ExecuteReader());
                conn.Close();
                if (msg != "")
                    MessageBox.Show(msg, "confirm", MessageBoxButtons.OK, MessageBoxIcon.Information);
             }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
            }
            return tbl;
        }

        public bool execute_data(string stmt , string msg)
        {
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = stmt;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                if(msg != "")
                    MessageBox.Show(msg, "confirm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch
            {
                return false;
            } 
        }

    }
}

