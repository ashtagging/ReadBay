using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ReadBay.Models
{
    public class Product
    {
        // Key Not neccessary as it is automatically implied
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        [Range(0, 10000)]
        public double RRP { get; set; }

        [Required]
        [Range(0, 10000)]
        public double Price { get; set; }

        [Required]
        [Range(0, 10000)]
        public double Price50 { get; set; }

        [Required]
        [Range(0, 10000)]
        public double Price100 { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Please Select A Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required(ErrorMessage = "Please Select A Book Type")]
        public int BookTypeId { get; set; }

        [ForeignKey("BookTypeId")]
        public BookType BookType { get; set; }

    }
}
