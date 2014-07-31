// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The login view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The login view model.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required(ErrorMessage = "Please enter login.")]
        [Display(Name = "Login")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required(ErrorMessage = "Please enter password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember me.
        /// </summary>
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}