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
        [Required]
        public string CountryName { get; set; }

        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime Modified { get; set; }

    }
}
