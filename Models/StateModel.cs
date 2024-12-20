using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_Backened.Models
{
    public class StateModel
    {
        public int? StateID { get; set; }

        [Required]
        public string StateName { get; set; }
        [Required]
        public string StateCode { get; set; }
        [Required]
        public int CountryID { get; set; }

        public string CountryName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

    }
}
