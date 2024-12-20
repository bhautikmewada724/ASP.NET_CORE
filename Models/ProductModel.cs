using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_Backened.Models
{
    public class ProductModel
    {

        public int? ProductID{ get; set; }

        [Required(ErrorMessage = "ProductName Required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "ProductPrice Required")]
        public Double ProductPrice { get; set; }

        [Required(ErrorMessage = "ProductCode Required")]
        public string ProductCode { get; set; }
        
        [Required(ErrorMessage = "Description Required")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "UserID Required")]
        public int UserID { get; set; }

    }

    public class ProductDropDownModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }

}
