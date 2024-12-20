using Microsoft.AspNetCore.Mvc;
using Nice_Admin_Backened.Models;
using System.Data.SqlClient;
using System.Data;

namespace Nice_Admin_Backened.Controllers
{
    public class BillsController : Controller
    {
        private IConfiguration configuration;
        public BillsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region OrderDropDown
        public void OrderDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Order_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                OrderDropDownModel orderDropDownModel = new OrderDropDownModel();
                orderDropDownModel.OrderID = Convert.ToInt32(data["OrderID"]);
                orderList.Add(orderDropDownModel);
            }
            ViewBag.OrderList = orderList;
        }

        #endregion

        #region UserDropDown
        public void UserDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_User_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<UserDropDownModel> userlist = new List<UserDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userlist.Add(userDropDownModel);
            }
            ViewBag.UserList = userlist;
        }

        #endregion

        #region BillAddEditForm
        public IActionResult BillAddEditForm(int BillID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bill_SelectByID";
            command.Parameters.AddWithValue("@BillID", BillID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            BillsModel billModel = new BillsModel();

            foreach (DataRow dataRow in table.Rows)
            {
                billModel.BillID = Convert.ToInt32(@dataRow["BillID"]);
                billModel.BillNumber = @dataRow["BillNumber"].ToString();
                billModel.BillDate = Convert.ToDateTime(@dataRow["BillDate"]);
                billModel.TotalAmount = Convert.ToDecimal(@dataRow["TotalAmount"]);
                billModel.Discount = Convert.ToDecimal(@dataRow["Discount"]);
                billModel.NetAmount = Convert.ToDecimal(@dataRow["NetAmount"]);
                billModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            OrderDropDown();
            UserDropDown();
            return View("BillAddEditForm", billModel);
        }

        #endregion

        #region BillSave
        [HttpPost]
        public IActionResult BillSave(BillsModel billModel)
        {
            if (billModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (billModel.BillID == null)
                {
                    command.CommandText = "PR_Bill_Insert";
                }
                else
                {
                    command.CommandText = "PR_Bill_Update";
                    command.Parameters.Add("@BillID", SqlDbType.Int).Value = billModel.BillID;
                }
                command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billModel.BillNumber;
                command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = billModel.BillDate;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billModel.TotalAmount;
                command.Parameters.Add("@Discount", SqlDbType.VarChar).Value = billModel.Discount;
                command.Parameters.Add("@NetAmount", SqlDbType.VarChar).Value = billModel.NetAmount;
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billModel.OrderID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = billModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("BillsListPage");
            }

            return View("BillAddEditForm", billModel);
        }
        #endregion

        #region BillDelete
        public IActionResult BillDelete(int BillID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Bill_Delete";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = BillID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("BillsListPage");
        }
        #endregion

        #region BillsListPage
        public IActionResult BillsListPage()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bill_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion
    }
}
