using AnyStore.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyStore.DAL
{
    class loginDAL
    {
        //Cloud-ready connection string - Get from environment variable or configuration
        private string GetConnectionString()
        {
            // Priority: Environment variable > App.config connection string
            string envConnString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnString))
            {
                return envConnString;
            }

            string configConnString = ConfigurationManager.ConnectionStrings["connstrng"]?.ConnectionString;
            if (!string.IsNullOrEmpty(configConnString))
            {
                return configConnString;
            }

            throw new InvalidOperationException("Database connection string not configured. Set DB_CONNECTION_STRING environment variable.");
        }

        public bool loginCheck(loginBLL l)
        {
            //Create a boolean variable and set its value to false and return it
            bool isSuccess = false;

            //Connecting To Database
            SqlConnection conn = new SqlConnection(GetConnectionString());

            try
            {
                //SQL Query to check login
                string sql = "SELECT * FROM tbl_users WHERE username=@username AND password=@password AND user_type=@user_type";

                //Creating SQL Command to pass value
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@username", l.username);
                cmd.Parameters.AddWithValue("@password", l.password);
                cmd.Parameters.AddWithValue("@user_type", l.user_type);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                //Checking The rows in DataTable 
                if(dt.Rows.Count>0)
                {
                    //Login Sucessful
                    isSuccess = true;
                }
                else
                {
                    //Login Failed
                    isSuccess = false;
                }
            }
            catch(Exception ex)
            {
                // Cloud-ready: Log to Console instead of MessageBox
                CloudLogger.Error("Database operation failed", ex);
            }
            finally
            {
                conn.Close();
            }

            return isSuccess;
        }
    }
}
