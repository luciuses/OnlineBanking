// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="">
//   
// </copyright>
// <summary>
//   The client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.Domain.Entities
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    /// <summary>
    /// The client.
    /// </summary>
    [Table("Clients")]
    public class Client
    {
        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [HiddenInput(DisplayValue = false)]
        public int ClientId { get; set; }

        /// <summary>
        /// Gets or sets the contract number.
        /// </summary>
        [Required]
        [DisplayName("Contract Number")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid contract number")]
        public int? ContractNumber { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Required(ErrorMessage = "Please enter a client firstname")]
        [DisplayName("First name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required(ErrorMessage = "Please enter a client lastname")]
        [DisplayName("Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date birth.
        /// </summary>
        [Required]
        [DisplayName("Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateBirth { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [Required]
        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [Required(ErrorMessage = "Please enter a client status")]
        [DisplayName("Status")]
        public StatusClient? Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether deposit.
        /// </summary>
        [DisplayName("Deposit")]
        public bool Deposit { get; set; }
    }
}