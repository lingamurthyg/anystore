using AnyStore.UI;
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

namespace AnyStore
{
    public partial class frmUserDashboard : Form
    {
        // Cloud-ready: Session-based state management instead of static variables
        private string _sessionId;
        private string _transactionType;

        public frmUserDashboard(string sessionId)
        {
            InitializeComponent();
            _sessionId = sessionId;
        }

        // Backward compatibility constructor (deprecated)
        public frmUserDashboard()
        {
            InitializeComponent();
            _sessionId = Guid.NewGuid().ToString();
        }
        private void frmUserDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmLogin login = new frmLogin();
            login.Show();
            this.Hide();
        }

        private void frmUserDashboard_Load(object sender, EventArgs e)
        {
            // Cloud-ready: Get username from session instead of static variable
            var session = SessionManager.GetSession(_sessionId);
            if (session != null)
            {
                lblLoggedInUser.Text = session.Username;
            }
            else
            {
                lblLoggedInUser.Text = "Guest";
            }
        }

        private void dealerAndCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDeaCust DeaCust = new frmDeaCust();
            DeaCust.Show();
        }

        private void purchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cloud-ready: Pass transaction type to form instead of using static variable
            _transactionType = "Purchase";
            frmPurchaseAndSales purchase = new frmPurchaseAndSales(_sessionId, _transactionType);
            purchase.Show();
        }

        private void salesFormsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cloud-ready: Pass transaction type to form instead of using static variable
            _transactionType = "Sales";
            frmPurchaseAndSales sales = new frmPurchaseAndSales(_sessionId, _transactionType);
            sales.Show();
        }

        private void inventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmInventory inventory = new frmInventory();
            inventory.Show();
        }
    }
}
