using System.ComponentModel.DataAnnotations;

namespace OnlineBankingForManager.WebUI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage="Please enter login.")]
        [Display(Name = "Login")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}