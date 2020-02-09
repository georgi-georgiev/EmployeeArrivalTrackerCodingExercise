using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApplication;
using WebApplication.Controllers;
using WebApplication.DTO;
using WebApplication.Models;

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            var dbContext = new WebAppDbContext();
            var subscriber = new TestClientSubscriber();
            var controller = new HomeController(dbContext, subscriber);
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Callback_MissingHttpRequestContext()
        {
            var dbContext = new WebAppDbContext();
            var subscriber = new TestClientSubscriber();
            var controller = new HomeController(dbContext, subscriber);
            var employees = new List<EmployeeDTO>
            {
                new EmployeeDTO
                {
                    EmployeeID=1,
                    When = DateTime.UtcNow
                }
            };
            var result = controller.Callback(employees) as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public void Callback_WrongToken()
        {
            var dbContext = new WebAppDbContext();

            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var headers = new NameValueCollection();
            headers.Add("X-Fourth-Token", Guid.NewGuid().ToString());
            request.Setup(r => r.Headers).Returns(headers);
            httpContext.Setup(c => c.Request).Returns(request.Object);

            var subscriber = new TestClientSubscriber();
            var controller = new HomeController(dbContext, subscriber);
            var controllerContext = new ControllerContext(new RequestContext(httpContext.Object, new RouteData()), controller);
            controller.ControllerContext = controllerContext;

            var employees = new List<EmployeeDTO>
            {
                new EmployeeDTO
                {
                    EmployeeID=1,
                    When = DateTime.UtcNow
                }
            };
            var result = controller.Callback(employees) as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public void Callback_ExpiredToken()
        {
            var token = Guid.NewGuid().ToString();
            var dbContext = new WebAppDbContext();
            dbContext.Tokens.Add(new Token
            {
                Value = token,
                Expires = DateTime.UtcNow.AddHours(-1)
            });

            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var headers = new NameValueCollection();
            headers.Add("X-Fourth-Token", token);
            request.Setup(r => r.Headers).Returns(headers);
            httpContext.Setup(c => c.Request).Returns(request.Object);

            var subscriber = new TestClientSubscriber();
            var controller = new HomeController(dbContext, subscriber);
            var controllerContext = new ControllerContext(new RequestContext(httpContext.Object, new RouteData()), controller);
            controller.ControllerContext = controllerContext;

            var employees = new List<EmployeeDTO>
            {
                new EmployeeDTO
                {
                    EmployeeID=1,
                    When = DateTime.UtcNow
                }
            };
            var result = controller.Callback(employees) as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public void Callback()
        {
            var token = Guid.NewGuid().ToString();

            var mockEmployees = new Mock<DbSet<Employee>>();
            var mockArrivals = new Mock<DbSet<Arrival>>();
            var mockTokens = new Mock<DbSet<Token>>();
            var mockDbContext = new Mock<WebAppDbContext>();

            var tokens = new List<Token>
            {
                new Token
                {
                    Id=1,
                    Value = token,
                    Expires = DateTime.UtcNow.AddHours(8)
                }
            }.AsQueryable();

            mockTokens.As<IQueryable<Token>>().Setup(m => m.Provider).Returns(tokens.Provider);
            mockTokens.As<IQueryable<Token>>().Setup(m => m.Expression).Returns(tokens.Expression);
            mockTokens.As<IQueryable<Token>>().Setup(m => m.ElementType).Returns(tokens.ElementType);
            mockTokens.As<IQueryable<Token>>().Setup(m => m.GetEnumerator()).Returns(tokens.GetEnumerator());

            mockDbContext.Setup(c => c.Employees).Returns(mockEmployees.Object);
            mockDbContext.Setup(c => c.Arrivals).Returns(mockArrivals.Object);
            mockDbContext.Setup(c => c.Tokens).Returns(mockTokens.Object);

            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var headers = new NameValueCollection();
            headers.Add("X-Fourth-Token", token);
            request.Setup(r => r.Headers).Returns(headers);
            httpContext.Setup(c => c.Request).Returns(request.Object);

            var subscriber = new TestClientSubscriber();
            var controller = new HomeController(mockDbContext.Object, subscriber);
            var controllerContext = new ControllerContext(new RequestContext(httpContext.Object, new RouteData()), controller);
            controller.ControllerContext = controllerContext;

            var employees = new List<EmployeeDTO>
            {
                new EmployeeDTO
                {
                    EmployeeID=1,
                    When = DateTime.UtcNow
                }
            };
            var result = controller.Callback(employees) as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        }
    }
}
