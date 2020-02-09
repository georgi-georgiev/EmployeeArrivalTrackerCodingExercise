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
    public class ArrivalControllerTest
    {
        private ArrivalController controller;
        private IQueryable<Arrival> arrivals;

        [TestInitialize]
        public void Initialize()
        {
            var mockEmployees = new Mock<DbSet<Employee>>();
            var mockArrivals = new Mock<DbSet<Arrival>>();
            var mockTokens = new Mock<DbSet<Token>>();
            var mockDbContext = new Mock<WebAppDbContext>();

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id=1,
                    EmployeeNumber = 1
                },
                new Employee
                {
                    Id=2,
                    EmployeeNumber = 276
                },
                new Employee
                {
                    Id=3,
                    EmployeeNumber = 1276
                },
                new Employee
                {
                    Id=4,
                    EmployeeNumber = 1368
                },
                new Employee
                {
                    Id=5,
                    EmployeeNumber = 1600
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
                    Date = DateTime.Parse("2016-03-10")
                };

                listArrivals.Add(arrival);
            }

            arrivals = listArrivals.AsQueryable();

            mockArrivals.As<IQueryable<Arrival>>().Setup(m => m.Provider).Returns(arrivals.Provider);
            mockArrivals.As<IQueryable<Arrival>>().Setup(m => m.Expression).Returns(arrivals.Expression);
            mockArrivals.As<IQueryable<Arrival>>().Setup(m => m.ElementType).Returns(arrivals.ElementType);
            mockArrivals.As<IQueryable<Arrival>>().Setup(m => m.GetEnumerator()).Returns(arrivals.GetEnumerator());

            mockDbContext.Setup(c => c.Employees).Returns(mockEmployees.Object);
            mockDbContext.Setup(c => c.Arrivals).Returns(mockArrivals.Object);
            mockDbContext.Setup(c => c.Tokens).Returns(mockTokens.Object);

            controller = new ArrivalController(mockDbContext.Object);
        }

        [TestMethod]
        public void Index_NoSortingNoFiltering()
        {
            var result = controller.Index(null, null, null) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            foreach (var arrival in arrivals)
            {
                var actualArrival = model
                    .FirstOrDefault(e => e.Date == arrival.Date && e.EmployeeNumber == arrival.Employee.EmployeeNumber);
                Assert.IsNotNull(actualArrival);
            }
        }

        [TestMethod]
        public void Index_EmptySortingEmtyFiltering()
        {
            var result = controller.Index("", "", "") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            foreach (var arrival in arrivals)
            {
                var actualArrival = model
                    .FirstOrDefault(e => e.Date == arrival.Date && e.EmployeeNumber == arrival.Employee.EmployeeNumber);
                Assert.IsNotNull(actualArrival);
            }
        }

        [TestMethod]
        public void Index_OnlySorting()
        {
            var result = controller.Index("number_desc", "", "") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            var lastEmployeeNumber = int.MinValue;
            foreach (var arrival in arrivals)
            {
                var actualArrival = model
                    .FirstOrDefault(e => e.Date == arrival.Date && e.EmployeeNumber == arrival.Employee.EmployeeNumber);
                Assert.IsNotNull(actualArrival);
                Assert.IsTrue(lastEmployeeNumber < arrival.Employee.EmployeeNumber);

                lastEmployeeNumber = arrival.Employee.EmployeeNumber;
            }
        }

        [TestMethod]
        public void Index_OnlyDateSorting()
        {
            var result = controller.Index("date", "", "") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            var lastDate = DateTime.MinValue;
            foreach (var arrival in arrivals)
            {
                var actualArrival = model
                    .FirstOrDefault(e => e.Date == arrival.Date && e.EmployeeNumber == arrival.Employee.EmployeeNumber);
                Assert.IsNotNull(actualArrival);
                Assert.IsTrue(lastDate <= arrival.Date);

                lastDate = arrival.Date;
            }
        }

        [TestMethod]
        public void Index_OnlyDateDescSorting()
        {
            var result = controller.Index("date_desc", "", "") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            var lastDate = DateTime.MaxValue;
            foreach (var arrival in arrivals)
            {
                var actualArrival = model
                    .FirstOrDefault(e => e.Date == arrival.Date && e.EmployeeNumber == arrival.Employee.EmployeeNumber);
                Assert.IsNotNull(actualArrival);
                Assert.IsTrue(lastDate >= arrival.Date);

                lastDate = arrival.Date;
            }
        }

        [TestMethod]
        public void Index_OnlyFiltering()
        {
            var result = controller.Index("", "1368", "2016-03-10") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 1);

            var arrival = model[0];
            Assert.IsNotNull(arrival);
            Assert.IsTrue(arrival.EmployeeNumber == 1368);
            Assert.IsTrue(arrival.Date == DateTime.Parse("2016-03-10"));
        }

        [TestMethod]
        public void Index_OnlyNumberFiltering()
        {
            var result = controller.Index("", "276", "") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 2);

            var firstArrival = model[0];
            Assert.IsNotNull(firstArrival);
            Assert.IsTrue(firstArrival.EmployeeNumber == 276);
            Assert.IsTrue(firstArrival.Date == DateTime.Parse("2016-03-10"));

            var secondArrival = model[1];
            Assert.IsNotNull(secondArrival);
            Assert.IsTrue(secondArrival.EmployeeNumber == 1276);
            Assert.IsTrue(secondArrival.Date == DateTime.Parse("2016-03-10"));
        }

        [TestMethod]
        public void Index_OnlyDateFiltering()
        {
            var result = controller.Index("", "", "2016-03-10") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 5);

            foreach (var arrival in arrivals)
            {
                var actualArrival = model
                    .FirstOrDefault(e => e.Date == arrival.Date && e.EmployeeNumber == arrival.Employee.EmployeeNumber);
                Assert.IsNotNull(actualArrival);
            }
        }

        [TestMethod]
        public void Index_SortingAndFiltering()
        {
            var result = controller.Index("number_desc", "1600", "2016-03-10") as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ArrivalViewModel>;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count == 1);

            var arrival = model[0];
            Assert.IsNotNull(arrival);
            Assert.IsTrue(arrival.EmployeeNumber == 1600);
            Assert.IsTrue(arrival.Date == DateTime.Parse("2016-03-10"));
        }
    }
}
