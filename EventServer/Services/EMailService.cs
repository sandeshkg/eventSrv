using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace EventServer.Services
{
    public class EMailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await SendMessageUsingAWS(message);
        }

        private async Task SendMessageUsingAWS(IdentityMessage message)
        {
            //try
            //{
                string from = "shanmugamcse@gmail.com";   // Replace with your "From" address. This address must be verified.
                //string to = toAddress;  // Replace with a "To" address. If your account is still in the
                                        // sandbox, this address must be verified.
                                        

                //                bool test = ValidateUser(cacheKey, otp);

                //string subject = "OTP For Bulletin Registration";

                //string body = message.Body;// "Your OTP to install the bulletin App is " + otp + ". This is valid for the next 30 mins.";

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
                        //    Console.WriteLine("Attempting to send an email through the Amazon SES SMTP interface...");
                        await client.SendMailAsync(from, message.Destination, message.Subject, message.Body);
                        //Console.WriteLine("Email sent!");
                    }
                    catch (Exception ex)
                    {
                    //    await Task.FromResult(0);
                    //    //Console.WriteLine("The email was not sent.");
                    //    //Console.WriteLine("Error message: " + ex.Message);
                    }
                }

                //Console.Write("Press any key to continue...");
                //Console.ReadKey();
            //}
            //catch (Exception ex) {
            //    await Task.FromResult(0);
            //}
        }
    }
}