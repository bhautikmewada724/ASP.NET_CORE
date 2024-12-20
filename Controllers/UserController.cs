using Microsoft.AspNetCore.Mvc;
using Nice_Admin_Backened.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Nice_Admin_Backened.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            return new SqlConnection(connectionString);
        }

        public IActionResult UserAddEditForm(int UserID)
        {
            if (UserID == null)
            {
                return View(new UserModel());
            }

            UserModel userModel = new UserModel();
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_SelectByID";
                    command.Parameters.AddWithValue("@UserID", UserID);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        DataRow dataRow = table.Rows[0];

                        userModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                        userModel.UserName = dataRow["UserName"].ToString();
                        userModel.Email = dataRow["Email"].ToString();
                        userModel.Password = dataRow["Password"].ToString();
                        userModel.MobileNo = dataRow["MobileNo"].ToString();
                        userModel.Address = dataRow["Address"].ToString();
                        userModel.isActive = Convert.ToBoolean(dataRow["isActive"]);
                    }
                }
            }

            return View("UserAddEditForm", userModel);
        }

        [HttpPost]
        public IActionResult UserSave(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View("UserAddEditForm", userModel);
            }

            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (userModel.UserID == null || userModel.UserID <= 0)
                    {
                        command.CommandText = "PR_User_Insert";
                    }
                    else
                    {
                        command.CommandText = "PR_User_Update";
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = userModel.UserID;
                    }

                    command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userModel.UserName;
                    command.Parameters.Add("@Email", SqlDbType.VarChar).Value = userModel.Email;
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = userModel.Password;
                    command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userModel.MobileNo;
                    command.Parameters.Add("@Address", SqlDbType.VarChar).Value = userModel.Address;
                    command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = userModel.isActive;
                    command.ExecuteNonQuery();
                }
                return RedirectToAction("UsersListPage");

            }

        }
        public IActionResult UserDelete(int userId)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_User_Delete";
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }

            return RedirectToAction("UsersListPage");
        }

        public IActionResult UsersListPage()
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_SelectAll";
                    SqlDataReader reader = command.ExecuteReader();
                    table.Load(reader);
                }
            }
            return View(table);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
    }
}
