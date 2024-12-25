using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nice_Admin_Backened.Models
{
    public class CityModel
    {
        public int? CityID { get; set; }

        [Required]
        [DisplayName("City Name")]
        public string CityName { get; set; }

        [Required]
        public int CountryID { get; set; }

        [Required]
        public string CountryName { get; set; }

        [Required]
        public int StateID { get; set; }

        [Required]
        public string StateName { get; set; }

        [Required]
        public string CityCode { get; set; }
    }

    public class CountryDropDownModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }

    public class StateDropDownModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }

}
