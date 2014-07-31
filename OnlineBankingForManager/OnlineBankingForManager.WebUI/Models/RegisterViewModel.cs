// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The register view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The register view model.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required]
        [Display(Name = "Login")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Repeat Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        [Required]
        [Display(Name = "Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the user address.
        /// </summary>
        [Display(Name = "Address")]
        public string UserAddress { get; set; }
    }
}