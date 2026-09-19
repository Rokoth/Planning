using Castle.Core.Logging;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Planning.Contracts.Model;
using Planning.Controllers;
using Planning.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Planning.UnitTests.UnitTests
{
    public class ControllerTests
    {
        protected const string IndexMethodName = "Index";
        protected const string ErrorControllerName = "Error";
        protected const string HomeControllerName = "Home";

        /// <summary>
        /// 0.0.5.2.1 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AccountControllerLogin_success()
        {
            var authService = new Mock<IAuthService>();
            var logger = new Mock<ILogger<AccountController>>();
            var controller = new AccountController(logger.Object, authService.Object);
            var authenticationService = new Mock<IAuthenticationService>();
            var urlHelperFactory = new Mock<IUrlHelperFactory>();

            var features = new FeatureCollection();

            // Example: Mocking request headers
            var headers = new Dictionary<string, StringValues>
            {
                { "Authorization", "Bearer my_token" },
                { "Content-Type", "application/json" }
            };
            var headerDictionary = new HeaderDictionary(headers);
            features.Set<IHttpRequestFeature>(new HttpRequestFeature { Headers = headerDictionary });

            // Example: Mocking request body
            var requestBody = "{\"name\":\"test\"}";
            var requestBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            features.Set<IHttpRequestFeature>(new HttpRequestFeature { Body = requestBodyStream });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationService.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactory.Object);
            

            // 2. Create a DefaultHttpContext with the features
            var httpContext = new DefaultHttpContext(features)
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "login"),
                    new Claim(ClaimTypes.Role, "password")
                })),
                 RequestServices  = serviceProviderMock.Object
            };
            
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            authService.Setup(s => s.Auth(It.IsAny<UserIdentity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(CreateClaims()));

            var result = await controller.Login(new UserIdentity()
            {
                Login = "login",
                Password = "password"
            }, string.Empty);
            var parsed = result as RedirectToActionResult;
            Assert.NotNull(parsed);
            Assert.Equal(IndexMethodName, parsed.ActionName);
            Assert.Equal(HomeControllerName, parsed.ControllerName);
        }

        /// <summary>
        /// 0.0.5.2.2
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AccountControllerLogout_success()
        {
            var authService = new Mock<IAuthService>();
            var logger = new Mock<ILogger<AccountController>>();
            var controller = new AccountController(logger.Object, authService.Object);
            var authenticationService = new Mock<IAuthenticationService>();
            var urlHelperFactory = new Mock<IUrlHelperFactory>();

            var features = new FeatureCollection();

            // Example: Mocking request headers
            var headers = new Dictionary<string, StringValues>
            {
                { "Authorization", "Bearer my_token" },
                { "Content-Type", "application/json" }
            };
            var headerDictionary = new HeaderDictionary(headers);
            features.Set<IHttpRequestFeature>(new HttpRequestFeature { Headers = headerDictionary });

            // Example: Mocking request body
            var requestBody = "{\"name\":\"test\"}";
            var requestBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            features.Set<IHttpRequestFeature>(new HttpRequestFeature { Body = requestBodyStream });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationService.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactory.Object);


            // 2. Create a DefaultHttpContext with the features
            var httpContext = new DefaultHttpContext(features)
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "login"),
                    new Claim(ClaimTypes.Role, "password")
                })),
                RequestServices = serviceProviderMock.Object
            };

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            authService.Setup(s => s.Auth(It.IsAny<UserIdentity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(CreateClaims()));

            var result = await controller.Logout();
            var parsed = result as RedirectToActionResult;
            Assert.NotNull(parsed);
            Assert.Equal(IndexMethodName, parsed.ActionName);
            Assert.Equal(HomeControllerName, parsed.ControllerName);
        }

        private static System.Security.Claims.ClaimsIdentity CreateClaims()
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, Guid.NewGuid().ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
                };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
