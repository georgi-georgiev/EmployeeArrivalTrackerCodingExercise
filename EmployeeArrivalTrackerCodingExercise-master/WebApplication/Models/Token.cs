using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime Expires { get; set; }
    }
}