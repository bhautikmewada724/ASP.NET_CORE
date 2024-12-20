using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_Backened.Models
{
    public class OrderDetailModel
    {
        
        public int? OrderDetailID { get; set; }
        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ProductID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public int UserID { get; set; }
    }
}
