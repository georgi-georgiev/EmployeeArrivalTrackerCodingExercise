using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class ArrivalController : Controller
    {
        private readonly WebAppDbContext webAppDbContext;

        public ArrivalController(WebAppDbContext webAppDbContext)
        {
            this.webAppDbContext = webAppDbContext;
        }

        public ActionResult Index(string sortOrder, string filterNumberString, string filterDateString)
        {
            ViewBag.EmployeeNumberSortParam = string.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewBag.DateSortParam = sortOrder == "date" ? "date_desc" : "date";

            IQueryable<Arrival> arrivals = webAppDbContext.Arrivals;

            if (!string.IsNullOrWhiteSpace(filterNumberString))
                arrivals = arrivals.Where(a => a.Employee.EmployeeNumber.ToString().Contains(filterNumberString));

            if (!string.IsNullOrWhiteSpace(filterDateString))
            {
                var filterDate = DateTime.Parse(filterDateString);
                arrivals = arrivals.Where(a => a.Date.Year == filterDate.Year &&
                    a.Date.Month == filterDate.Month && a.Date.Day == filterDate.Day);
            }

            switch (sortOrder)
            {
                case "number_desc":
                    arrivals = arrivals.OrderByDescending(a => a.Employee.EmployeeNumber);
                    break;
                case "date":
                    arrivals = arrivals.OrderBy(a => a.Date);
                    break;
                case "date_desc":
                    arrivals = arrivals.OrderByDescending(a => a.Date);
                    break;
            }

            return View(arrivals.Select(a => new ArrivalViewModel
            {
                Date = a.Date,
                EmployeeNumber = a.Employee.EmployeeNumber
            }).ToList());
        }
    }
}