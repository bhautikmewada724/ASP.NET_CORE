using Microsoft.AspNetCore.Mvc;
using Nice_Admin_Backened.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Nice_Admin_Backened.Controllers
{
    public class OrderController : Controller
    {
        private IConfiguration configuration;

        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

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

        public void CustomerDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Customer_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<CustomerDropDownModel> customerlist = new List<CustomerDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel();
                customerDropDownModel.CustomerID = Convert.ToInt32(data["CustomerID"]);
                customerDropDownModel.CustomerName = data["CustomerName"].ToString();
                customerlist.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = customerlist;
        }
        public IActionResult OrderAddEditForm(int OrderID)
        {

            UserDropDown();
            CustomerDropDown();
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_SelectByID";
            command.Parameters.AddWithValue("@OrderId", OrderID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            OrderModel orderModel = new OrderModel();

            foreach (DataRow dataRow in table.Rows)
            {
                orderModel.OrderID = Convert.ToInt32(@dataRow["OrderID"]);
                orderModel.OrderDate = Convert.ToDateTime(@dataRow["OrderDate"]);
                orderModel.PaymentMode = @dataRow["PaymentMode"].ToString();
                orderModel.TotalAmount = Convert.ToDecimal(@dataRow["TotalAmount"]);
                orderModel.ShippingAddress = @dataRow["ShippingAddress"].ToString();
                orderModel.CustomerId = Convert.ToInt32(@dataRow["CustomerId"]);
                orderModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            return View("OrderAddEditForm", orderModel);

        }

        //SERVER SIDE

        [HttpPost]
        public IActionResult OrderSave(OrderModel orderModel)
        {
            if (orderModel.OrderID <= 0)
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
                if (orderModel.OrderID == null)
                {
                    command.CommandText = "PR_Order_Insert";
                }
                else
                {
                    command.CommandText = "PR_Order_Update";
                    command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderModel.OrderID;
                }
                command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
                command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
                command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
                command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = orderModel.CustomerId;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("OrderListPage");
            }
            return View("OrderAddEditForm", orderModel);
        }


        #region OrderDelete
        public IActionResult OrderDelete(int OrderID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Order_Delete";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("OrderListPage");
        }

        #endregion

        #region OrderListPage
        public IActionResult OrderListPage()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion
    }
}
