using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGTH.Dvtel.Rest.Filters;

namespace SGTH.Dvtel.Rest.Tests.Filters
{
    [TestClass]
    public abstract class BasicAuthenticatationAttributeTest<T> where T : BasicAuthenticationAttribute, new()
    {
        protected string Username { get; set; }

        protected string Password { get; set; }

        #region AuthenticateAsync Test Methods ...

        [TestMethod]
        public virtual async Task AuthenticateAsync_From_HttpAuthenticationContext_Without_AuhtherizationHeader_NoOperation_Test()
        {
            // Arrange            
            HttpAuthenticationContext ctx = CreateHttpAuthentication(null, null);

            // Act
            BasicAuthenticationAttribute auth = new T();
            await auth.AuthenticateAsync(ctx, new CancellationToken());

            // Assert
            Assert.IsNull(ctx.Request.Headers.Authorization);
            Assert.IsNull(ctx.Principal);
        }

        [TestMethod]
        public virtual async Task AuthenticateAsync_From_HttpAuthenticationContext_IsNot_Basic_NoOperation_Test()
        {
            // Arrange
            HttpAuthenticationContext ctx = CreateHttpAuthentication(new AuthenticationHeaderValue("Bearer"), null);

            // Act
            BasicAuthenticationAttribute auth = new T();
            await auth.AuthenticateAsync(ctx, new CancellationToken());

            // Assert
            Assert.IsNotNull(ctx.Request.Headers.Authorization);
            Assert.IsNull(ctx.Principal);
        }

        [TestMethod]
        public virtual async Task AuthenticateAsync_From_HttpAuthenticationContext_Basic_With_Missing_Credentials_Test()
        {
            // Arrange            
            HttpAuthenticationContext ctx = CreateHttpAuthentication(new AuthenticationHeaderValue("Basic"), null);

            // Act            
            BasicAuthenticationAttribute auth = new T();
            await auth.AuthenticateAsync(ctx, new CancellationToken());
            HttpResponseMessage response = await ctx.ErrorResult.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.IsNotNull(ctx.Request.Headers.Authorization);
            Assert.IsNull(ctx.Principal);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual("Missing credentials", response.ReasonPhrase);
        }

        [TestMethod]
        public virtual async Task AuthenticateAsync_From_HttpAuthenticationContext_Basic_With_Invalid_Credentials_Test()
        {
            // Arrange
            byte[] parameterBytes = Encoding.UTF8.GetBytes("1234567890");
            string parameter = Convert.ToBase64String(parameterBytes);
            HttpAuthenticationContext ctx = CreateHttpAuthentication(new AuthenticationHeaderValue("Basic", parameter), null);

            // Act
            BasicAuthenticationAttribute auth = new T();
            await auth.AuthenticateAsync(ctx, new CancellationToken());
            HttpResponseMessage response = await ctx.ErrorResult.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.IsNotNull(ctx.Request.Headers.Authorization);
            Assert.IsNull(ctx.Principal);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual("Invalid credentials", response.ReasonPhrase);
        }

        [TestMethod]
        public virtual async Task AuthenticateAsync_From_HttpAuthenticationContext_Basic_With_Valid_Credentials_Test()
        {
            // Arrange
            byte[] parameterBytes = Encoding.UTF8.GetBytes($"{Username}:{Password}");
            string parameter = Convert.ToBase64String(parameterBytes);
            HttpAuthenticationContext ctx = CreateHttpAuthentication(new AuthenticationHeaderValue("Basic", parameter), null);

            // Act
            BasicAuthenticationAttribute auth = new T();
            await auth.AuthenticateAsync(ctx, new CancellationToken());

            // Assert
            Assert.IsNotNull(ctx.Request.Headers.Authorization);
            Assert.IsNotNull(ctx.Principal);
            Assert.IsTrue(ctx.Principal.Identity.IsAuthenticated);
            Assert.AreEqual("usertest", ctx.Principal.Identity.Name);
        }

        [TestMethod]
        public virtual async Task AuthenticateAsync_From_HttpAuthenticationContext_Basic_With_Invalid_UsernameOrPassword_Test()
        {
            // Arrange
            byte[] parameterBytes = Encoding.UTF8.GetBytes($"{Username}:{Password}");
            string parameter = Convert.ToBase64String(parameterBytes);
            HttpAuthenticationContext ctx = CreateHttpAuthentication(new AuthenticationHeaderValue("Basic", parameter), null);

            // Act
            BasicAuthenticationAttribute auth = new T();
            await auth.AuthenticateAsync(ctx, new CancellationToken());
            HttpResponseMessage response = await ctx.ErrorResult.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.IsNotNull(ctx.Request.Headers.Authorization);
            Assert.IsNull(ctx.Principal);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual("Invalid username or password", response.ReasonPhrase);
        }

        private HttpAuthenticationContext CreateHttpAuthentication(AuthenticationHeaderValue header, IPrincipal principal)
        {
            HttpControllerContext ctrl = new HttpControllerContext { Request = new HttpRequestMessage() };
            HttpActionContext action = new HttpActionContext { ControllerContext = ctrl };
            HttpAuthenticationContext auth = new HttpAuthenticationContext(action, principal);
            HttpRequestHeaders headers = auth.Request.Headers;
            headers.Authorization = header;
            return auth;
        }

        #endregion

        #region ChallangeAsync Test Methods ...

        [TestMethod]
        public virtual async Task ChallangeAsync_From_Unauthorized_Status_Without_Realm_Test()
        {
            // Arrange
            HttpActionContext actionContext = new HttpActionContext();
            IHttpActionResult action = new StatusCodeResult(HttpStatusCode.Unauthorized, new HttpRequestMessage());
            HttpAuthenticationChallengeContext ctx = new HttpAuthenticationChallengeContext(actionContext, action);

            // Act
            BasicAuthenticationAttribute auth = new T();
            await auth.ChallengeAsync(ctx, new CancellationToken());
            HttpResponseMessage response = await ctx.Result.ExecuteAsync(new CancellationToken());
            
            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual(1, response.Headers.WwwAuthenticate.Count);
            Assert.IsNotNull(response.Headers.WwwAuthenticate.SingleOrDefault(h => h.Scheme.Equals("Basic")));
        }

        [TestMethod]
        public virtual async Task ChallangeAsync_From_Unauthorized_Status_With_Realm_Test()
        {
            // Arrange
            HttpActionContext actionContext = new HttpActionContext();
            IHttpActionResult action = new StatusCodeResult(HttpStatusCode.Unauthorized, new HttpRequestMessage());
            HttpAuthenticationChallengeContext ctx = new HttpAuthenticationChallengeContext(actionContext, action);

            // Act
            BasicAuthenticationAttribute auth = new T
            {
                Realm = "Environment Test"
            };
            await auth.ChallengeAsync(ctx, new CancellationToken());
            HttpResponseMessage response = await ctx.Result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual(1, response.Headers.WwwAuthenticate.Count);
            Assert.IsNotNull(response.Headers.WwwAuthenticate.SingleOrDefault(h => h.Scheme.Equals("Basic")));
            Assert.AreEqual($"realm=\"{auth.Realm}\"" ,response.Headers.WwwAuthenticate.Single(h => h.Scheme.Equals("Basic")).Parameter);
        }

        [TestMethod]
        public virtual async Task ChallangeAsync_From_Distinct_Unauthorized_NoOperation_Test()
        {
            // Arrange
            HttpActionContext actionContext = new HttpActionContext();
            IHttpActionResult action = new StatusCodeResult(HttpStatusCode.OK, new HttpRequestMessage());
            HttpAuthenticationChallengeContext ctx = new HttpAuthenticationChallengeContext(actionContext, action);

            // Act
            BasicAuthenticationAttribute auth = new T();
            await auth.ChallengeAsync(ctx, new CancellationToken());
            HttpResponseMessage response = await ctx.Result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.IsNotNull(response);
            Assert.AreNotEqual(HttpStatusCode.Unauthorized, response.StatusCode);                        
        }

        #endregion
    }
}
