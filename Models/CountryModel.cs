using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_Backened.Models
{
    public class CountryModel
    {
        public int? CountryID { get; set; }

        [Required]
        public string CountryCode { get; set; }

        [Required]
        public string CountryName { get; set; }
    }
}
