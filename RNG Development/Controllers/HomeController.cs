using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using RNG_Development.Models;
using System.Net.Mime;

namespace RNG_Development.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        // Method to send an email for contact form submissions
        private async Task SendContactFormEmail(string name, string email, string phone, string message)
        {
            try
            {
                // Using the email inputted by the user as the 'from' address
                var fromAddress = new MailAddress(email, name);
                var toAddress = new MailAddress("your-email@example.com", "RNG Development"); // RNG Development email address
                const string fromPassword = "your-secure-generated-app-password"; // Generated app password

                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(toAddress.Address, fromPassword)
                };

                // Creating the email message
                var mailMessage = new MailMessage
                {
                    From = fromAddress,
                    Subject = "New Contact Form Submission",
                    IsBodyHtml = true
                };

                // Using a relative path from the project root
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "RNG_Logo.png");
                var logoAttachment = new Attachment(logoPath)
                {
                    ContentId = "RNG_Logo",
                    ContentType = new ContentType("image/png")
                };

                // Adding the logo as an inline attachment
                mailMessage.Attachments.Add(logoAttachment);

                // Constructing the email body with cleaner formatting and styling
                mailMessage.Body = $@"
                                        <div style='font-family: Arial, sans-serif; color: #333;'>
                                            <div style='text-align: center; margin-bottom: 20px;'>
                                                <img src='cid:RNG_Logo' alt='RNG Development Logo' style='width: 100%;' />
                                            </div>
                                            <h2 style='text-align: center; text-decoration: underline;'>New Contact Form Submission</h2>
                                            <p style='text-align: left; margin-top: 20px;'>
                                                You have received a new contact form submission. Below are the details:
                                            </p>
                                            <p style='text-align: left; font-weight: bold;'>
                                                <strong>Name:</strong> {name}
                                            </p>
                                            <p style='text-align: left; font-weight: bold;'>
                                                <strong>Email:</strong> {email}
                                            </p>
                                            <p style='text-align: left; font-weight: bold;'>
                                                <strong>Phone:</strong> {phone}
                                            </p>
                                            <p style='text-align: left; font-weight: bold;'>
                                                <strong>Message:</strong>
                                            </p>
                                            <p style='text-align: left;'>
                                                {message}
                                            </p>
                                            <p style='text-align: left; margin-top: 20px;'>
                                                Please review the contact form submission at your earliest convenience.
                                            </p>
                                        </div>";

                // Adding recipient's address
                mailMessage.To.Add(toAddress);

                // Sending the email asynchronously
                await smtpClient.SendMailAsync(mailMessage);

                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        // Handles contact form submission, validates required fields, sends email, and provides success/error feedback
        [HttpPost]
        public async Task<IActionResult> SendEmail(string Name, string Email, string Phone, string Message)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Message))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToAction("Contact");
            }

            try
            {
                // Calling the SendContactFormEmail method to send the email
                await SendContactFormEmail(Name, Email, Phone, Message);

                // Redirecting with a success message
                TempData["SuccessMessage"] = "Your message has been sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while sending your message: {ex.Message}";
            }

            return RedirectToAction("Contact");
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
