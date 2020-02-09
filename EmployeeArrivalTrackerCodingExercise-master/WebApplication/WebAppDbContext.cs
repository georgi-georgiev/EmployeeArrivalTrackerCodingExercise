using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication
{
    public class WebAppDbContext : DbContext
    {
        public WebAppDbContext()
            :base("DefaultConnection")
        {

        }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Arrival> Arrivals { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
    }
}