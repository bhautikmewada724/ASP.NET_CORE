using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Nice_Admin_Backened.Models;

namespace Nice_Admin_Backened.Controllers
{
    public class StateController : Controller
    {
        #region configuration
        private readonly IConfiguration _configuration;

        public StateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region StateListPage
        public IActionResult StateListPage()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_State_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View("StateListPage", table);
        }
        #endregion

        #region Delete
        public IActionResult DeleteState(int StateID)
        {
            string connectionstr = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionstr))
            {
                conn.Open();
                using (SqlCommand sqlCommand = conn.CreateCommand())
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_LOC_State_Delete";
                    sqlCommand.Parameters.AddWithValue("@StateID", StateID);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            return RedirectToAction("StateListPage");
        }
        #endregion

        #region CountryDropDown
        private void LoadCountryList()
        {
            string connectionstr = _configuration.GetConnectionString("ConnectionString");
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionstr))
            {
                conn.Open();
                using (SqlCommand objCmd = conn.CreateCommand())
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.CommandText = "PR_LOC_Country_DropDown";

                    using (SqlDataReader objSDR = objCmd.ExecuteReader())
                    {
                        dt.Load(objSDR); // Load data into DataTable
                    }
                }
            }

            // Map data to list
            List<CountryDropDownModel> countryList = new List<CountryDropDownModel>();
            foreach (DataRow dr in dt.Rows)
            {
                countryList.Add(new CountryDropDownModel
                {
                    CountryID = Convert.ToInt32(dr["CountryID"]),
                    CountryName = dr["CountryName"].ToString()
                });
            }
            ViewBag.CountryList = countryList; // Pass list to view
        }
        #endregion

        #region CityAddEdit
        public IActionResult StateAddEdit(int StateID)
        {
            LoadCountryList();
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_State_SelectByPK";
            command.Parameters.AddWithValue("@StateID", StateID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            StateModel stateModel = new StateModel();

            foreach (DataRow dataRow in table.Rows)
            {
                stateModel.StateID = Convert.ToInt32(@dataRow["StateID"]);
                stateModel.StateName = @dataRow["StateName"].ToString();
                stateModel.StateCode = @dataRow["StateCode"].ToString();
                stateModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);

            }
            return View("StateAddEdit", stateModel);
        }
        #endregion

        #region StateSave
        [HttpPost]
        public IActionResult StateSave(StateModel stateModel)
        {
            if (stateModel.StateID <= 0)
            {
                ModelState.AddModelError("StateID", "A valid State is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (stateModel.StateID == null)
                {
                    command.CommandText = "PR_LOC_State_Insert";
                }
                else
                {
                    command.CommandText = "PR_LOC_State_Update";
                    command.Parameters.Add("@StateID", SqlDbType.Int).Value = stateModel.StateID;
                }
                command.Parameters.Add("@StateName", SqlDbType.VarChar).Value = stateModel.StateName;
                command.Parameters.Add("@StateCode", SqlDbType.VarChar).Value = stateModel.StateCode;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = stateModel.CountryID;
                command.ExecuteNonQuery();
                return RedirectToAction("StateListPage");
            }
            LoadCountryList();
            return View("StateAddEdit", stateModel);
        }
        #endregion
    }
}
