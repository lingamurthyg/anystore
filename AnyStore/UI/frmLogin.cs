using AnyStore.BLL;
using AnyStore.DAL;
using static AnyStore.DAL.SessionManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyStore.UI
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        loginBLL l = new loginBLL();
        loginDAL dal = new loginDAL();

        // Cloud-ready: Replaced static variable with session-based state management
        // Session ID stored per form instance instead of global static state
        private string _sessionId = Guid.NewGuid().ToString();

        private void pboxClose_Click(object sender, EventArgs e)
        {
            //Code to close this form
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            l.username = txtUsername.Text.Trim();
            l.password = txtPassword.Text.Trim();
            l.user_type = cmbUserType.Text.Trim();

            //Checking the login credentials
            bool sucess = dal.loginCheck(l);
            if(sucess==true)
            {
                //Login Successful - Cloud-ready: Store session data instead of static variable
                MessageBox.Show("Login Successful.");

                // Get user ID from DAL
                userDAL uDAL = new userDAL();
                userBLL user = uDAL.GetIDFromUsername(l.username);

                // Create session in session manager
                SessionManager.CreateSession(_sessionId, l.username, l.user_type, user.id);
                //Need to open Respective Forms based on User Type
                switch(l.user_type)
                {
                    case "Admin":
                        {
                            //Display Admin Dashboard - Pass session ID
                            frmAdminDashboard admin = new frmAdminDashboard(_sessionId);
                            admin.Show();
                            this.Hide();
                        }
                        break;

                    case "User":
                        {
                            //Display User Dashboard - Pass session ID
                            frmUserDashboard userDashboard = new frmUserDashboard(_sessionId);
                            userDashboard.Show();
                            this.Hide();
                        }
                        break;

                    default:
                        {
                            //Display an error message
                            MessageBox.Show("Invalid User Type.");
                        }
                        break;
                }
            }
            else
            {
                //login Failed
                MessageBox.Show("Login Failed. Try Again");
            }
        }
    }
}
