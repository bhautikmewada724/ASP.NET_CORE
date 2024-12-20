using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_Backened.Models
{
    public class CustomerModel
    {
        public int? CustomerID { get; set; }

        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string HomeAddress { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string GSTNo { get; set; }
        [Required]
        public string CityName { get; set; }
        [Required]
        public string Pincode { get; set; }
        [Required]
        public decimal NetAmount { get; set; }
        [Required]
        public int UserID { get; set; }

    }

    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
