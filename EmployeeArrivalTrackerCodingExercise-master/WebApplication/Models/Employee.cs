using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeNumber { get; set; }
    }
}