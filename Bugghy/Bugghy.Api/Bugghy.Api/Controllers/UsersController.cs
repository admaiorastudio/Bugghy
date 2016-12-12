namespace AdMaiora.Bugghy.Api.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Mail;
    using System.Web.Http;
    using System.Web.Http.Tracing;
    using System.Web.Security;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.IdentityModel.Tokens;
    using System.Security.Claims;

    using AdMaiora.Bugghy.Api.Models;
    using AdMaiora.Bugghy.Api.DataObjects;

    using RestSharp;

    using Microsoft.Azure.Mobile.Server;
    using Microsoft.Azure.Mobile.Server.Config;
    using Microsoft.Azure.Mobile.Server.Login;
    using Microsoft.Azure.NotificationHubs;

    using SendGrid;
    using SendGrid.Helpers.Mail;
    using System.Configuration;

    public class UsersController : ApiController
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        // Authorization token duration (in days)
        public const int AUTH_TOKEN_MAX_DURATION = 1;

        public const string JWT_SECURITY_TOKEN_AUDIENCE = "https://bugghy-api.azurewebsites.net/";
        public const string JWT_SECURITY_TOKEN_ISSUER = "https://bugghy-api.azurewebsites.net/";

        #endregion

        #region Constructors

        public UsersController()
        {
        }

        #endregion

        #region Users Endpoint Methods

        [HttpPut, Route("users/register")]
        public async Task<IHttpActionResult> RegisterUser(Poco.User credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.Email))
                return BadRequest("The email is not valid!");

            if (string.IsNullOrWhiteSpace(credentials.Password))
                return BadRequest("The password is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    User user = ctx.Users.SingleOrDefault(x => x.Email == credentials.Email);
                    if (user != null)
                        return InternalServerError(new InvalidOperationException("This email has already taken!"));

                    user = new User { Email = credentials.Email, Password = credentials.Password };
                    user.Ticket = Guid.NewGuid().ToString();
                    ctx.Users.Add(user);
                    ctx.SaveChanges();

                    string apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
                    SendGridAPIClient mc = new SendGridAPIClient(apiKey);

                    Email to = new Email(user.Email);
                    Email from = new Email("info@admaiorastudio.com");
                    string subject = "Welocme to Bugghy!";
                    Content content = new Content("text/plain",
                        String.Format("Hi {0},\n\nYou registration on Bugghy is almost complete. Please click on this link to confirm your registration!\n\n{1}",
                        user.Email.Split('@')[0],
                        String.Format("https://bugghy-api.azurewebsites.net/users/confirm?ticket={0}", user.Ticket)));
                    Mail mail = new Mail(from, subject, to, content);

                    dynamic response = await mc.client.mail.send.post(requestBody: mail.Get());

                    return Ok(Dto.Wrap(new Poco.User
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        AuthAccessToken = null,
                        AuthExpirationDate = null

                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("users/verify")]
        public async Task<IHttpActionResult> VerifyUser(Poco.User credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.Email))
                return BadRequest("The email is not valid!");

            if (string.IsNullOrWhiteSpace(credentials.Password))
                return BadRequest("The password is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    User user = ctx.Users.SingleOrDefault(x => x.Email == credentials.Email);
                    if (user == null)
                        return InternalServerError(new InvalidOperationException("This email is not registered!"));

                    if (user.IsConfirmed)
                        return InternalServerError(new InvalidOperationException("This email has been already confirmed!"));

                    string p1 = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "MD5");
                    string p2 = FormsAuthentication.HashPasswordForStoringInConfigFile(credentials.Password, "MD5");
                    if (p1 != p2)
                        return InternalServerError(new InvalidOperationException("Your credentials seem to be not valid!"));

                    string apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
                    SendGridAPIClient mc = new SendGridAPIClient(apiKey);

                    Email to = new Email(user.Email);
                    Email from = new Email("info@admaiorastudio.com");
                    string subject = "Welocme to Listy!";
                    Content content = new Content("text/plain",
                        String.Format("Hi {0},\n\nYou registration on Bugghy is almost complete. Please click on this link to confirm your registration!\n\n{1}",
                        user.Email.Split('@')[0],
                        String.Format("https://bugghy-api.azurewebsites.net/users/confirm?ticket={0}", user.Ticket)));
                    Mail mail = new Mail(from, subject, to, content);

                    dynamic response = await mc.client.mail.send.post(requestBody: mail.Get());
                    if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                        return InternalServerError(new InvalidOperationException("Internal mail error. Retry later!"));

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("users/confirm")]
        public IHttpActionResult ConfirmUser(string ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket))
                return BadRequest("The ticket is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    User user = ctx.Users.SingleOrDefault(x => x.Ticket == ticket);
                    if (user == null)
                        return BadRequest("This ticket is not a real!");

                    user.IsConfirmed = true;
                    ctx.SaveChanges();

                    IHttpActionResult response;
                    //we want a 303 with the ability to set location
                    HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.RedirectMethod);
                    responseMsg.Headers.Location = new Uri("http://www.admaiorastudio.com/bugghy");
                    response = ResponseMessage(responseMsg);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("users/login")]
        public IHttpActionResult LoginUser(Poco.User credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.Email))
                return BadRequest("The email is not valid!");

            if (string.IsNullOrWhiteSpace(credentials.Password))
                return BadRequest("The password is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    User user = ctx.Users.SingleOrDefault(x => x.Email == credentials.Email);
                    if (user == null)
                        return Unauthorized();

                    if (!user.IsConfirmed)
                        return InternalServerError(new InvalidOperationException("You must confirm your email first!"));

                    if (!String.IsNullOrWhiteSpace(user.GoogleId) && user.Password == null)
                        return InternalServerError(new InvalidOperationException("You must login via Google!"));

                    string p1 = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "MD5");
                    string p2 = FormsAuthentication.HashPasswordForStoringInConfigFile(credentials.Password, "MD5");
                    if (p1 != p2)
                        return Unauthorized();

                    var token = GetAuthenticationTokenForUser(user.Email);
                    user.LoginDate = DateTime.Now.ToUniversalTime();
                    user.LastActiveDate = user.LoginDate;
                    user.AuthAccessToken = token.RawData;
                    user.AuthExpirationDate = token.ValidTo;
                    ctx.SaveChanges();

                    return Ok(Dto.Wrap(new Poco.User
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        LoginDate = user.LoginDate,
                        AuthAccessToken = user.AuthAccessToken,
                        AuthExpirationDate = user.AuthExpirationDate
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("users/login/google")]
        public async Task<IHttpActionResult> LoginUser(Google.Credentials credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.ClientID))
                return BadRequest("The Google client ID is not valid!");

            if (string.IsNullOrWhiteSpace(credentials.Email))
                return BadRequest("The email is not valid!");

            if (string.IsNullOrWhiteSpace(credentials.Token))
                return BadRequest("The Google token is not valid!");

            try
            {
                RestClient c = new RestClient(new Uri("https://www.googleapis.com"));

                // To login via google token, we need first to validate the token passed
                // To validate the token we must check if it belongs to our Google application
                // Reference: https://developers.google.com/identity/sign-in/android/backend-auth

                // Validation request
                RestRequest vr = new RestRequest("oauth2/v3/tokeninfo", Method.GET);
                vr.AddParameter("id_token", credentials.Token);
                var r = await c.ExecuteTaskAsync<Google.TokenClaims>(vr);

                if (r.StatusCode != HttpStatusCode.OK)
                    return InternalServerError(new InvalidOperationException("Unable to login via Google"));

                if (r.Data.aud != credentials.ClientID
                    || r.Data.email != credentials.Email
                    || r.Data.email_verified == false)
                {
                    return InternalServerError(new InvalidOperationException("Unable to login via Google"));
                }

                using (var ctx = new BugghyDbContext())
                {
                    // Check if we have already registered the user, if not this login method will take care of it
                    User user = ctx.Users.SingleOrDefault(x => x.Email == credentials.Email);
                    if (user == null)
                    {
                        user = new User
                        {
                            GoogleId = r.Data.sub,
                            Email = credentials.Email,
                            Password = null,
                            Ticket = Guid.NewGuid().ToString(),
                            IsConfirmed = true
                        };

                        ctx.Users.Add(user);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        user.GoogleId = r.Data.sub;
                        user.IsConfirmed = true;

                        ctx.SaveChanges();
                    }

                    var token = GetAuthenticationTokenForUser(user.Email);
                    user.LoginDate = DateTime.Now.ToUniversalTime();
                    user.LastActiveDate = user.LoginDate;
                    user.AuthAccessToken = token.RawData;
                    user.AuthExpirationDate = token.ValidTo;
                    ctx.SaveChanges();

                    return Ok(Dto.Wrap(new Poco.User
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        LoginDate = user.LoginDate,
                        AuthAccessToken = user.AuthAccessToken,
                        AuthExpirationDate = user.AuthExpirationDate
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet, Route("users/restore")]
        public IHttpActionResult RestoreUser(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return BadRequest("The access token is not valid!");

            try
            {
                using (var ctx = new BugghyDbContext())
                {
                    User user = ctx.Users.SingleOrDefault(x => x.AuthAccessToken == accessToken);
                    if (user == null)
                        return Unauthorized();

                    user.LoginDate = DateTime.Now.ToUniversalTime();
                    user.LastActiveDate = user.LoginDate;
                    ctx.SaveChanges();

                    return Ok(Dto.Wrap(new Poco.User
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        LoginDate = user.LoginDate,
                        AuthAccessToken = user.AuthAccessToken,
                        AuthExpirationDate = user.AuthExpirationDate
                    }));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

        #region Methods

        private JwtSecurityToken GetAuthenticationTokenForUser(string email)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email.Split('@')[0]),
                new Claim(JwtRegisteredClaimNames.Email, email),
            };

            // The WEBSITE_AUTH_SIGNING_KEY variable will be only available when
            // you enable App Service Authentication in your App Service from the Azure back end
            https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-dotnet-backend-how-to-use-server-sdk/#how-to-work-with-authentication

            var signingKey = Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY");
            var audience = UsersController.JWT_SECURITY_TOKEN_AUDIENCE;
            var issuer = UsersController.JWT_SECURITY_TOKEN_ISSUER;

            var token = AppServiceLoginHandler.CreateToken(
                claims,
                signingKey,
                audience,
                issuer,
                TimeSpan.FromDays(UsersController.AUTH_TOKEN_MAX_DURATION)
                );

            return token;
        }

        #endregion
    }
}