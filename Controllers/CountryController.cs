using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
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
            if (countryModel.CountryID <= 0)
            {
                ModelState.AddModelError("CountryID", "A valid CityID is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (countryModel.CountryID == null)
                {
                    command.CommandText = "PR_LOC_Country_Insert";
                }
                else
                {
                    command.CommandText = "PR_LOC_Country_Update";
                    command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryModel.CountryID;
                }
                command.Parameters.Add("@CountryName", SqlDbType.VarChar).Value = countryModel.CountryName;
                command.Parameters.Add("@CountryCode", SqlDbType.VarChar).Value = countryModel.CountryCode;
                command.ExecuteNonQuery();
                return RedirectToAction("CountryListPage");
            }
            return View("CountryAddEdit", countryModel);
        }
        #endregion

        #region CounsumingCountryApi

        private readonly Uri baseAddress = new Uri("https://localhost:7077/api");
        private readonly HttpClient _client;

        [ActivatorUtilitiesConstructor]
        public CountryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult GetAllCountries()
        {
            List<CountryModel> countries = new List<CountryModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Country").Result;

            if (response.IsSuccessStatusCode) { 
                string data = response.Content.ReadAsStringAsync().Result;
                countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
            }
            return View("CountryListPage", countries);
        }
        #endregion
    }
}
