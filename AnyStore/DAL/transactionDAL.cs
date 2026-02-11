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
    class transactionDAL
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

        #region Insert Transaction Method
        public bool Insert_Transaction(transactionsBLL t, out int transactionID)
        {
            //Create a boolean value and set its default value to false
            bool isSuccess = false;
            //Set the out transactionID value to negative 1 i.e. -1
            transactionID = -1;
            //Create a SqlConnection first
            SqlConnection conn = new SqlConnection(GetConnectionString());
            try
            {
                //SQL Query to Insert Transactions
                string sql = "INSERT INTO tbl_transactions (type, dea_cust_id, grandTotal, transaction_date, tax, discount, added_by) VALUES (@type, @dea_cust_id, @grandTotal, @transaction_date, @tax, @discount, @added_by); SELECT @@IDENTITY;";

                //Sql Commandto pass the value in sql query
                SqlCommand cmd = new SqlCommand(sql, conn);

                //Passing the value to sql query using cmd
                cmd.Parameters.AddWithValue("@type", t.type);
                cmd.Parameters.AddWithValue("@dea_cust_id", t.dea_cust_id);
                cmd.Parameters.AddWithValue("@grandTotal", t.grandTotal);
                cmd.Parameters.AddWithValue("@transaction_date", t.transaction_date);
                cmd.Parameters.AddWithValue("@tax", t.tax);
                cmd.Parameters.AddWithValue("@discount", t.discount);
                cmd.Parameters.AddWithValue("@added_by", t.added_by);

                //Open Database Connection
                conn.Open();

                //Execute the Query
                object o = cmd.ExecuteScalar();

                //If the query is executed successfully then the value will not be null else it will be null
                if(o!=null)
                {
                    //Query Executed Successfully
                    transactionID = int.Parse(o.ToString());
                    isSuccess = true;
                }
                else
                {
                    //failed to execute query
                    isSuccess = false;
                }
            }
            catch(Exception ex)
            {
                // Cloud-ready: Log to Console instead of MessageBox for cloud monitoring
                CloudLogger.Error($"Database operation failed", ex);
            }
            finally
            {
                //Close the connection 
                conn.Close();
            }

            return isSuccess;
        }
        #endregion
        #region METHOD TO DISPLAY ALL THE TRANSACTION
        public DataTable DisplayAllTransactions()
        {
            //SQlConnection First
            SqlConnection conn = new SqlConnection(GetConnectionString());

            //Create a DAta Table to hold the datafrom database temporarily
            DataTable dt = new DataTable();

            try
            {
                //Write the SQL Query to Display all Transactions
                string sql = "SELECT * FROM tbl_transactions";

                //SqlCommand to Execute Query
                SqlCommand cmd = new SqlCommand(sql, conn);

                //SqlDataAdapter to Hold the data from database
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //Open DAtabase Connection
                conn.Open();

                adapter.Fill(dt);
            }
            catch(Exception ex)
            {
                // Cloud-ready: Log to Console instead of MessageBox for cloud monitoring
                CloudLogger.Error($"Database operation failed", ex);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }
        #endregion
        #region METHOD TO DISPLAY TRANSACTION BASED ON TRANSACTION TYPE
        public DataTable DisplayTransactionByType(string type)
        {
            //Create SQL Connection
            SqlConnection conn = new SqlConnection(GetConnectionString());

            //Create a DataTable
            DataTable dt = new DataTable();

            try
            {
                //Write SQL Query - FIXED: Parameterized query to prevent SQL injection
                string sql = "SELECT * FROM tbl_transactions WHERE type=@type";

                //SQL Command to Execute Query
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@type", type);
                //SQlDataAdapter to hold the data from database
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //Open DAtabase Connection
                conn.Open();
                adapter.Fill(dt);
            }
            catch(Exception ex)
            {
                // Cloud-ready: Log to Console instead of MessageBox for cloud monitoring
                CloudLogger.Error($"Database operation failed", ex);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }
        #endregion
    }
}
