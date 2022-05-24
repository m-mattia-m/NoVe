using System;
using Microsoft.AspNetCore.Mvc;
using NoVe.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
    
namespace NoVe.Controllers
{
    public class AccountController : Controller
    {

        private readonly DatabaseHelper _dbContext;
        public AccountController(DatabaseHelper dbContext) {
          _dbContext = dbContext;

          //Um diesen Constructor aufzurufen muss dieser Controller aufgerufen werden z.B. über diese Url https://localhost:5001/Account/Registe
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public void setLogin(string Email, string Password) {

            try
            {
                string passwordHash = hashPassword(Password);
                var user = _dbContext.User.Where(b => b.Email == Email).FirstOrDefault();
                if (user.VerificationStatus == 1)
                {
                    if (user.AdminVerification == 1)
                    {
                        if (user.PasswordHash == passwordHash)
                        {
                            Console.WriteLine("Passwort stimmt überein");
                            //HttpContext.Session.SetInt32("_UserID", user.Id);
                            //HttpContext.Session.SetString("_UserRole", user.Role);
                        }
                        else
                        {
                            Console.WriteLine("Passwort ist falsch.");
                        }
                    }
                    else {
                        Console.WriteLine("Dein Administrator muss zuerst noch deinen Account bestätigen.");
                    }
                }
                else {
                    Console.WriteLine("Du hast deinen Account noch nicht verifiziert");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("User mit Email: '" + Email + "' exisitert noch nicht");
            }

            //var allUsers = _dbContext.User.ToList();
            //Console.Write("allUsers: ");
            //Console.WriteLine(allUsers);
        }

        public IActionResult RegisterCheck(string Email, string Vorname, string Nachname, string Password, string PasswordCheck, int Beruf, int Klasse)
        {
            var userCount = _dbContext.User.Where(b => b.Email == Email).Count();
            if (userCount != 0)
            {
                Console.WriteLine("Email existiert schon");
            }
            else {
                if (Password == PasswordCheck)
                {
                    Console.WriteLine("Passwörter stimmen überein");
                    Random rnd = new Random();
                    int VerificationKey = rnd.Next(100000, 1000000);

                    var newUser = new User();

                    newUser.Vorname = Vorname;
                    newUser.Nachname = Nachname;
                    newUser.Email = Email;
                    newUser.PasswordHash = hashPassword(Password);
                    newUser.KlassenId = Klasse;
                    newUser.VerificationKey = VerificationKey;
                    newUser.VerificationStatus = 0;

                    _dbContext.User.Add(newUser);
                    _dbContext.SaveChanges();


                    HttpContext.Session.SetString("_RegisterEmail", Email);
                    string SessionEmail = HttpContext.Session.GetString("_RegisterEmail");
                    Console.WriteLine("Email aus Session: " + SessionEmail);

                    Console.Write("newUser: ");
                    Console.WriteLine(newUser.ToString());

                    Console.WriteLine("VerificationKey: " + VerificationKey);

                    //sendMail();
                }
            }
            return View();
        }

        public IActionResult verify(string Email, int verificationKey) // string Email, 
        {  
            Console.WriteLine(verificationKey);
            //string Email = HttpContext.Session.GetString("_RegisterEmail");
            //Console.WriteLine("Email aus Session: " + Email);
            try
            {
                
                var user = _dbContext.User.Where(b => b.Email == Email).FirstOrDefault();
                if (user.VerificationKey == verificationKey)
                {
                    Console.WriteLine("Verifizierung erfolgreich");
                    user.VerificationStatus = 1;
                    _dbContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Verifizierung ist fehlgeschlagen.");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("User mit Email: '" + Email + "' exisitert noch nicht");
            }

            return View();

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

        public string hashPassword(string Password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Password);
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
