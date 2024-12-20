using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_Backened.Models
{
    public class UserModel
    {
        public int? UserID { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]


        public string MobileNo { get; set; }
        [Required]

        public string Address { get; set; }
        [Required]

        public Boolean isActive { get; set; }


    }
    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
