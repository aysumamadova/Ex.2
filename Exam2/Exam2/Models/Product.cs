using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Exam2.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Img { get; set; }
        [NotMapped, Required]
        public IFormFile Photo { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
