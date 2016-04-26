using EventServer.Infrastructure;
using EventServer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EventServer.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        ICacheManager _objCacheManager;
        public AccountsController()
        {
            _objCacheManager = Infrastructure.Cache.Instance;
        }

        [Authorize]
        [Route("registeremail")]
        public string RegisterEMail(string toAddress)
        {
            string userid = Guid.NewGuid().ToString();
            try
            {
                string from = "noreply@noreply.com";   // Replace with your "From" address. This address must be verified.
                string to = toAddress;  // Replace with a "To" address. If your account is still in the
                                        // sandbox, this address must be verified.

                string otp = GenerateOTP();

                //_objCacheManager.Add(userid, otp);

                SlidingTime _objSlidingTime = new SlidingTime(TimeSpan.FromMinutes(30));
                _objCacheManager.Add(userid, otp, CacheItemPriority.Normal, null, _objSlidingTime);

//                bool test = ValidateUser(cacheKey, otp);

                string subject = "OTP For Bulletin Registration";

                string body = "Your OTP to install the bulletin App is " + otp + ". This is valid for the next 30 mins.";

                // Supply your SMTP credentials below. Note that your SMTP credentials are different from your AWS credentials.
                const String SMTP_USERNAME = "AKIAIVJWSJ6LMD5EPQKQ";  // Replace with your SMTP username. 
                const String SMTP_PASSWORD = "AhlN7kt4ftjjACNtyTHO8k1SGaAfO1VduEXd0yu9ceUg";  // Replace with your SMTP password.

                // Amazon SES SMTP host name. This example uses the US West (Oregon) region.
                const String HOST = "email-smtp.us-west-2.amazonaws.com";

                // The port you will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
                // STARTTLS to encrypt the connection.
                const int PORT = 587;

                // Create an SMTP client with the specified host name and port.
                using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(HOST, PORT))
                {
                    // Create a network credential with your SMTP user name and password.
                    client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                    // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                    // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                    client.EnableSsl = true;

                    // Send the email. 
                    try
                    {
                        Console.WriteLine("Attempting to send an email through the Amazon SES SMTP interface...");
                        client.Send(from, to, subject, body);
                        //Console.WriteLine("Email sent!");
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("The email was not sent.");
                        //Console.WriteLine("Error message: " + ex.Message);
                    }
                }

                //Console.Write("Press any key to continue...");
                //Console.ReadKey();
            }
            catch (Exception ex) { }


            return userid;
        }

        private string GenerateOTP()
        {
            Random generator = new Random();
            string otpValue = generator.Next(100000, 999999).ToString();

            return otpValue;
        }

        [Authorize]
        [Route("validateotp")]
        public bool ValidateUser(string guid, string userEnteredOTP)
        {
            if (_objCacheManager.Contains(guid))
            {
                string generatedOTP = _objCacheManager.GetData(guid).ToString();
                if (userEnteredOTP == generatedOTP)
                    return true;
            }

            return false;
        }

        [Authorize]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Authorize]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Authorize]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!createUserModel.Email.Contains("verizon.com"))
            {
                //return Ok();//error silently. do notreveal..
                //instead return a guid..make the user feel he is correct and let him wait forever :)
                return Json<string>(Guid.NewGuid().ToString());
            }
            var user = new ApplicationUser()
            {
                UserName = createUserModel.Email.Substring(0, createUserModel.Email.IndexOf('@')),
                //UserName = Guid.NewGuid().ToString().Replace('-' , 'h'),//createUserModel.Username,
                Email = createUserModel.Email,
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, user.UserName);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));

            await this.AppUserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");


            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                //ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            result = await this.AppUserManager.SetEmailAsync(userId, "a@b.com");


            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }
    }
}
