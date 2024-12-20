using Microsoft.AspNetCore.Mvc;
using Nice_Admin_Backened.Models;
using System.Data.SqlClient;
using System.Data;

namespace Nice_Admin_Backened.Controllers
{
    public class OrderDetailController : Controller
    {
        private IConfiguration configuration;

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

        public void ProductDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Product_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<ProductDropDownModel> productList = new List<ProductDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                ProductDropDownModel productDropDownModel = new ProductDropDownModel();
                productDropDownModel.ProductID = Convert.ToInt32(data["ProductID"]);
                productDropDownModel.ProductName = (data["ProductName"]).ToString();
                productList.Add(productDropDownModel);
            }
            ViewBag.ProductList = productList;
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
        public OrderDetailController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult OrderDetailAddEditForm(int OrderDetailID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_OrderDetail_SelectByID";
            command.Parameters.AddWithValue("@OrderDetailID", OrderDetailID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            OrderDetailModel orderDetailModel = new OrderDetailModel();

            foreach (DataRow dataRow in table.Rows)
            {
                orderDetailModel.OrderDetailID = Convert.ToInt32(@dataRow["OrderDetailID"]);
                orderDetailModel.OrderID = Convert.ToInt32(@dataRow["OrderID"]);
                orderDetailModel.ProductID = Convert.ToInt32(@dataRow["ProductID"]);
                orderDetailModel.Quantity = Convert.ToInt32(@dataRow["Quantity"]);
                orderDetailModel.Amount = Convert.ToDecimal(@dataRow["Amount"]);
                orderDetailModel.TotalAmount = Convert.ToDecimal(@dataRow["TotalAmount"]);
                orderDetailModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            OrderDropDown();
            ProductDropDown();
            UserDropDown();
            return View("OrderDetailAddEditForm" , orderDetailModel);
        }

        //SERVER SIDE 
        [HttpPost]
        public IActionResult OrderDetailSave(OrderDetailModel orderDetailModel)
        {
            if (orderDetailModel.OrderDetailID <= 0)
            {
                ModelState.AddModelError("OrderDetailID", "A valid OrderDetail is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (orderDetailModel.OrderDetailID == null)
                {
                    command.CommandText = "PR_OrderDetail_Insert";
                }
                else
                {
                    command.CommandText = "PR_OrderDetail_Update";
                    command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetailModel.OrderDetailID;
                }
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderDetailModel.OrderID;
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = orderDetailModel.ProductID;
                command.Parameters.Add("@Quantity", SqlDbType.Int).Value = orderDetailModel.Quantity;
                command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = orderDetailModel.Amount;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderDetailModel.TotalAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderDetailModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("OrderDetailsListPage");
            }
            OrderDropDown();
            ProductDropDown();
            UserDropDown();
            return View("OrderDetailAddEditForm", orderDetailModel);
        }

        public IActionResult OrderDetailDelete(int OrderDetailID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_OrderDetail_Delete";
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("OrderDetails");
        }
        public IActionResult OrderDetailsListPage()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_OrderDetail_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
    }
}
