using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ReadBay.Models
{
    // Adding properties to ASpnet User table so we need to inherit from IdentityUser
    // Application User Properties have been added to the Register Identity Model
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        // Not all users will belong to a company - Some users will be individual users aswell  
        // ? is added to companyid as it can be empty or a null value, this makes it a nullable field
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        // Not mapped means that the property will not be added to the Database
        [NotMapped]
        public string Role { get; set; }
    }
}
