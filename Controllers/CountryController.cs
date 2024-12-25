using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Nice_Admin_Backened.Models;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.Xml;
using Newtonsoft.Json;

namespace Nice_Admin_Backened.Controllers
{
    public class CountryController : Controller
    {

        #region configuration
        private readonly IConfiguration _configuration;
        
        public CountryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region CountryListPage 
        public IActionResult CountryListPage()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_Country_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View("CountryListPage", table);
        }
        #endregion

        #region Delete
        public IActionResult DeleteCountry(int CountryID)
        {
            string connectionstr = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionstr))
            {
                conn.Open();
                using (SqlCommand sqlCommand = conn.CreateCommand())
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_LOC_Country_Delete";
                    sqlCommand.Parameters.AddWithValue("@CountryID", CountryID);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            return RedirectToAction("CountryListPage");
        }
        #endregion

        #region CountryAddEdit
        public IActionResult CountryAddEdit(int CountryID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_Country_SelectByPK";
            command.Parameters.AddWithValue("@CountryID", CountryID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CountryModel countryModel = new CountryModel();

            foreach (DataRow dataRow in table.Rows)
            {
                countryModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);
                countryModel.CountryName = @dataRow["CountryName"].ToString();
                countryModel.CountryCode = @dataRow["CountryCode"].ToString();
            }
            return View("CountryAddEdit", countryModel);
        }
        #endregion

        #region CountrySave
        [HttpPost]
        public IActionResult CountrySave(CountryModel countryModel)
        {
            // Validate the CountryID to ensure it's provided
            if (countryModel.CountryID <= 0 && string.IsNullOrWhiteSpace(countryModel.CountryName))
            {
                ModelState.AddModelError("CountryID", "A valid CountryID is required.");
                ModelState.AddModelError("CountryName", "Country Name is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;

                    if (countryModel.CountryID <= 0 || countryModel.CountryID == null ) // Insert case
                    {
                        command.CommandText = "PR_LOC_Country_Insert";
                        command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;

                    }
                    else // Update case
                    {
                        command.CommandText = "PR_LOC_Country_Update";
                        command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryModel.CountryID;
                        command.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;

                    }

                    command.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = countryModel.CountryName;
                    command.Parameters.Add("@CountryCode", SqlDbType.NVarChar).Value = countryModel.CountryCode;

                    command.ExecuteNonQuery();
                }
                return RedirectToAction("CountryListPage");
            }
            return View("CountryAddEdit", countryModel);
        }
        #endregion

    }
}
