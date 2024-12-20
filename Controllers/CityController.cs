using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Nice_Admin_Backened.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Nice_Admin_Backened.Controllers
{
    public class CityController : Controller
    {
        private readonly IConfiguration _configuration;

        #region configuration
        public CityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region CityListPage
        public IActionResult CityListPage()
        {
            string connectionstr = this._configuration.GetConnectionString("ConnectionString");
            //PrePare a connection
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(connectionstr);
            conn.Open();

            //Prepare a Command
            SqlCommand objCmd = conn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_LOC_City_SelectAll";

            SqlDataReader objSDR = objCmd.ExecuteReader();
            dt.Load(objSDR);
            conn.Close();
            return View("CityListPage", dt);
        }
        #endregion

        #region Delete
        public IActionResult DeleteCity(int CityID)
        {
            string connectionstr = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionstr))
            {
                conn.Open();
                using (SqlCommand sqlCommand = conn.CreateCommand())
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_LOC_City_Delete";
                    sqlCommand.Parameters.AddWithValue("@CityID", CityID);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            return RedirectToAction("CityListPage");
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
                    objCmd.CommandText = "PR_LOC_Country_SelectComboBox";

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

        #region GetStatesByCountry
        // AJAX handler for loading states dynamically
        [HttpPost]
        public JsonResult GetStatesByCountry(int CountryID)
        {
            List<StateDropDownModel> loc_State = GetStateByCountryID(CountryID); // Fetch states
            return Json(loc_State); // Return JSON response
        }
        #endregion

        #region GetStateByCountryID
        // Helper method to fetch states by country ID
        public List<StateDropDownModel> GetStateByCountryID(int CountryID)
        {
            string connectionstr = _configuration.GetConnectionString("ConnectionString");
            List<StateDropDownModel> loc_State = new List<StateDropDownModel>();

            using (SqlConnection conn = new SqlConnection(connectionstr))
            {
                conn.Open();
                using (SqlCommand objCmd = conn.CreateCommand())
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.CommandText = "PR_LOC_State_SelectComboBoxByCountryID";
                    objCmd.Parameters.AddWithValue("@CountryID", CountryID);

                    using (SqlDataReader objSDR = objCmd.ExecuteReader())
                    {
                        if (objSDR.HasRows)
                        {
                            while (objSDR.Read())
                            {
                                loc_State.Add(new StateDropDownModel
                                {
                                    StateID = Convert.ToInt32(objSDR["StateID"]),
                                    StateName = objSDR["StateName"].ToString()
                                });
                            }
                        }
                    }
                }
            }

            return loc_State;
        }
        #endregion

        #region CityAddEdit
        public IActionResult CityAddEdit(int CityID)
        {
            LoadCountryList();
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_City_SelectByPK";
            command.Parameters.AddWithValue("@CityID", CityID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CityModel cityModel = new CityModel();

            foreach (DataRow dataRow in table.Rows)
            {
                cityModel.CityID = Convert.ToInt32(@dataRow["CityID"]);
                cityModel.CityName = @dataRow["CityName"].ToString();
                cityModel.CityCode = @dataRow["CityCode"].ToString();
                cityModel.StateID = Convert.ToInt32(@dataRow["StateID"]);
                cityModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);

            }
            return View("CityAddEdit", cityModel);
        }
        #endregion

        #region CitySave
        [HttpPost]
        public IActionResult CitySave(CityModel cityModel)
        {
            if (cityModel.CityID <= 0)
            {
                ModelState.AddModelError("CityID", "A valid CityID is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (cityModel.CityID == null)
                {
                    command.CommandText = "PR_LOC_City_Insert";
                }
                else
                {
                    command.CommandText = "PR_LOC_City_Update";
                    command.Parameters.Add("@CityID", SqlDbType.Int).Value = cityModel.CityID;
                }
                command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = cityModel.CityName;
                command.Parameters.Add("@CityCode", SqlDbType.VarChar).Value = cityModel.CityCode;
                command.Parameters.Add("@StateID", SqlDbType.Decimal).Value = cityModel.StateID;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = cityModel.CountryID;
                command.ExecuteNonQuery();
                return RedirectToAction("CityListPage");
            }
            LoadCountryList();
            return View("CityAddEdit", cityModel);
        }
        #endregion

        #region CounsumingCityApi

        private readonly Uri baseAddress = new Uri("https://localhost:7077/api");
        private readonly HttpClient _client;

        [ActivatorUtilitiesConstructor]
        public CityController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        #region GetAllCities
        [HttpGet]
        public IActionResult GetAllCities()
        {
            List<CityModel> cities = new List<CityModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/City").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                cities = JsonConvert.DeserializeObject<List<CityModel>>(data);
            }
            return View("CityListPage", cities);
        }
        #endregion

        #region InsertCity
        public async Task<IActionResult> Add(int? CityID)
        {
            await GetCountryList();
            if (CityID.HasValue)
            {
                var response = await _client.GetAsync($"api/city/{CityID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var city = JsonConvert.DeserializeObject<CityModel>(data);
                    ViewBag.StateList = await GetStates(city.CountryID);
                    return View(city);
                }
            }
            return View(new CityModel());
        }
        #endregion

        #region SaveCity
        [HttpPost]
        public async Task<IActionResult> Save(CityModel city)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(city);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (city.CityID == null)
                    response = await _client.PostAsync("api/city", content);
                else
                    response = await _client.PutAsync($"api/city/{city.CityID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }
            await GetCountryList();
            return View("Add", city);
        }
        #endregion

        #region LoadCountryList
        private async Task GetCountryList()
        {
            var response = await _client.GetAsync("api/city/countries");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<CountryDropDownModel>>(data);
                ViewBag.CountryList = countries;
            }
        }
        #endregion

        #region GetStates
        [HttpPost]
        public async Task<JsonResult> GetStates(int CountryID)
        {
            var states = await GetStates(CountryID);
            return Json(states);
        }
        #endregion

        #region DeleteCity
        
        public async Task<IActionResult> Delete(int CityID)
        {
            var response = await _client.DeleteAsync($"api/City/{CityID}");
            return RedirectToAction("CityListPage");
        }
        #endregion


        #endregion
    }
}

