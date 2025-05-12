using System.ComponentModel.DataAnnotations;


namespace FoodOrder.Application.DTOs.Authentication
{
    public class LoginUser
    {
        [Required(ErrorMessage ="User Name is required")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
