// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserProfile.cs" company="">
//   
// </copyright>
// <summary>
//   The user profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The user profile.
    /// </summary>
    [Table("UserProfile")]
    public class UserProfile
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the user address.
        /// </summary>
        public string UserAddress { get; set; }
    }
}