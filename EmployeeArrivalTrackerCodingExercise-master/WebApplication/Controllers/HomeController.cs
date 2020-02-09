using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Flurl.Http;
using WebApplication.DTO;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebAppDbContext webAppDbContext;
        private readonly IClientSubscriber clientSubscriber;

        public HomeController(WebAppDbContext webAppDbContext, IClientSubscriber clientSubscriber)
        {
            this.webAppDbContext = webAppDbContext;
            this.clientSubscriber = clientSubscriber;
        }

        public ActionResult Index()
        {
            var callback = string.Format("http://localhost:57498/home/{0}", nameof(Callback));
            var response = clientSubscriber.Subscribe(callback);

            webAppDbContext.Tokens.Add(new Token
            {
                Value = response.Token,
                Expires = response.Expires
            });

            webAppDbContext.SaveChanges();

            return View();
        }

        [HttpPost]
        public ActionResult Callback(List<EmployeeDTO> employees)
        {
            if(Request == null || Request.Headers == null || !Request.Headers.AllKeys.Contains("X-Fourth-Token"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var tokens = webAppDbContext.Tokens.Where(t => t.Expires > DateTime.UtcNow).Select(t => t.Value).ToList();
            var token = Request.Headers.Get("X-Fourth-Token");

            if (!tokens.Contains(token))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            foreach (var employee in employees)
            {
                var newEmployee = new Employee
                {
                    EmployeeNumber = employee.EmployeeID
                };

                webAppDbContext.Employees
                    .Add(newEmployee);

                webAppDbContext.Arrivals
                    .Add(new Arrival
                    {
                        Employee = newEmployee,
                        EmployeeId = newEmployee.Id,
                        Date = employee.When
                    });
            }

            webAppDbContext.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}