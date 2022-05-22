using System;
using Microsoft.AspNetCore.Mvc;
using NoVe.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace NoVe.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _dbContext;
        public AccountController(DatabaseHelper dbContext) {
          _dbContext = dbContext;

          //Um diesen Constructor aufzurufen muss dieser Controller aufgerufen werden z.B. über diese Url https://localhost:5001/Account/Register

          var allUsers = _dbContext.User.ToList();

          var newUser = new User();
          newUser.Vorname = "Apfel";
          newUser.Nachname = "Most";
          newUser.Email = "apfel@most.ch";
          newUser.PasswordHash = "GurKenSauGeIstMeeEeGaFeiNUndMussAlsRoteFarbEkonSuMIErtwerDeN";

          _dbContext.User.Add(newUser);
          _dbContext.SaveChanges();
        }

        public IActionResult Login()
        {
            return View();
        }

        public void setLogin(string Email, string Password) {
            Console.WriteLine(Email +  " - " + Password);
            sendMail();

        }

        public void register(string Email, string Password, string PasswordCheck)
        {
            if (Password == PasswordCheck) {
                Console.WriteLine("Passwörter stimmen überein");
            }
        }

        public void sendMail() {
            string From = "nove@mattiamueggler.ch";
            string Password = "?x9Ls!Uva77vJvic*SsF";
            string SmtpHost = "asmtp.mail.hostpoint.ch";
            int SmtpPortSSL = 465;
            int SmtpPort = 587;

            string To = "mattia@mattiamueggler.ch";
            string Subject = "TestEmail NoVe";
            string Body = "Ich bin der body der Testemail";

            MailMessage message = new MailMessage(From, To);
            message.Subject = Subject;
            message.Body = Body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient(SmtpHost, SmtpPortSSL);
            System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential(From, Password);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;

            try
            {
                client.Send(message);
                Console.WriteLine("Email wurde gesendet");
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }


        }
    }
}
