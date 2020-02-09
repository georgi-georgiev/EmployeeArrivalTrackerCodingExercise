using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly WebAppDbContext webAppDbContext;

        public EmployeeController(WebAppDbContext webAppDbContext)
        {
            this.webAppDbContext = webAppDbContext;
        }

        public ActionResult Index(string sortOrder, string filterString)
        {
            ViewBag.NumberSortParam = string.IsNullOrEmpty(sortOrder) ? "number_desc" : "";

            IQueryable<Employee> employees = webAppDbContext.Employees;

            if(!string.IsNullOrWhiteSpace(filterString))
                employees = employees.Where(e => e.EmployeeNumber.ToString().Contains(filterString));

            switch (sortOrder)
            {
                case "number_desc":
                    employees = employees.OrderByDescending(e => e.EmployeeNumber);
                    break;
            }

            return View(employees.Select(e => new EmployeeViewModel
            {
                EmployeeNumber = e.EmployeeNumber
            }).ToList());
        }
    }
}