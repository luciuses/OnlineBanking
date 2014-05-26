using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Web.Mvc;

namespace OnlineBankingForManager.Domain.Entities
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [HiddenInput(DisplayValue = false)]
        public int ClientId { get; set; }

        [Required]
        [DisplayName("Contract Number")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid contract number")]
        public int? ContractNumber { get; set; }

        [Required(ErrorMessage = "Please enter a client firstname")]
        [DisplayName("First name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a client lastname")]
        [DisplayName("Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataBirth { get; set; }

        [Required]
        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter a client name")]
        [DisplayName("Status")]
        [DataType(DataType.Text)]
        public string Status { get; set; }
        
       
        public bool Deposit { get; set; }
    }
}