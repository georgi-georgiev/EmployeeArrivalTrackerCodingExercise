using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApplication;
using WebApplication.Controllers;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class EmployeeControllerTest
    {
        private EmployeeController controller;
        private IQueryable<Employee> employees;

        [TestInitialize]
        public void Initialize()
        {
            var mockEmployees = new Mock<DbSet<Employee>>();
            var mockArrivals = new Mock<DbSet<Arrival>>();
            var mockTokens = new Mock<DbSet<Token>>();
            var mockDbContext = new Mock<WebAppDbContext>();

            employees = new List<Employee>
            {
                new Employee
                {
                    Id=1,
                    EmployeeNumber = 1
                },
                new Employee
                {
                    Id=2,
                    EmployeeNumber = 123
                },
                new Employee
                {
                    Id=3,
                    EmployeeNumber = 135
                },
                new Employee
                {
                    Id=3,
                    EmployeeNumber = 1350
                },
                new Employee
                {
                    Id=4,
                    EmployeeNumber = 1601
                }
            }.AsQueryable();

            mockEmployees.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employees.Provider);
            mockEmployees.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employees.Expression);
            mockEmployees.As<IQueryable<Employee>>().Setup(m => m.ElementType).Returns(employees.ElementType);
            mockEmployees.As<IQueryable<Employee>>().Setup(m => m.GetEnumerator()).Returns(employees.GetEnumerator());

            var listArrivals = new List<Arrival>();
            foreach (var employee in employees)
            {
                var arrival = new Arrival
                {
                    Employee = employee,
                    EmployeeId = employee.Id,
                    Date = DateTime.UtcNow
                };

                listArrivals.Add(arrival);
            }

            var arrivals = listArrivals.AsQueryable();

            mockArrivals.As<IQueryable<Arrival>>().Setup(m => m.Provider).Returns(arrivals.Provider);
            mockArrivals.As<IQueryable<Arrival>>().Setup(m => m.Expression).Returns(arrivals.Expression);
            mockArrivals.As<IQueryable<Arrival>>().Setup(m => m.ElementType).Returns(arrivals.ElementType);
            mockArrivals.As<IQueryable<Arrival>>().Setup(m => m.GetEnumerator()).Returns(arrivals.GetEnumerator());

            mockDbContext.Setup(c => c.Employees).Returns(mockEmployees.Object);
            mockDbContext.Setup(c => c.Arrivals).Returns(mockArrivals.Object);
            mockDbContext.Setup(c => c.Tokens).Returns(mockTokens.Object);

            controller = new EmployeeController(mockDbContext.Object);
        }

        [TestMethod]
        public void Index_NoSortingNoFiltering()
        {
            ViewResult result = controller.Index(null, null) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<EmployeeViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            foreach(var employee in employees)
            {
                var actualEmployee = model.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
                Assert.IsNotNull(actualEmployee);
            }
        }

        [TestMethod]
        public void Index_EmptySortingEmptyFiltering()
        {
            ViewResult result = controller.Index("", "") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<EmployeeViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            foreach (var employee in employees)
            {
                var actualEmployee = model.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
                Assert.IsNotNull(actualEmployee);
            }
        }

        [TestMethod]
        public void Index_OnlySorting()
        {
            ViewResult result = controller.Index("number_desc", "") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<EmployeeViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            var lastEmployeeNumber = int.MinValue;
            foreach (var employee in employees)
            {
                var actualEmployee = model.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
                Assert.IsNotNull(actualEmployee);
                Assert.IsTrue(lastEmployeeNumber < employee.EmployeeNumber);

                lastEmployeeNumber = employee.EmployeeNumber;
            }
        }

        [TestMethod]
        public void Index_OnlyFiltering()
        {
            ViewResult result = controller.Index("", "1601") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<EmployeeViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 1);

            var employee = model[0];
            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.EmployeeNumber == 1601);
        }

        [TestMethod]
        public void Index_SortingAndFiltering()
        {
            ViewResult result = controller.Index("number_desc", "135") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<EmployeeViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 2);

            var firstEmployee = model[0];
            Assert.IsNotNull(firstEmployee);
            Assert.IsTrue(firstEmployee.EmployeeNumber == 1350);

            var secondEmployee = model[1];
            Assert.IsNotNull(secondEmployee);
            Assert.IsTrue(secondEmployee.EmployeeNumber == 135);
        }
    }
}
