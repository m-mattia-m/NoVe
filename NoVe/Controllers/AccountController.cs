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
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NoVe.Controllers
{
  public class AccountController : Controller
  {

    private readonly DatabaseHelper _dbContext;
    public AccountController(DatabaseHelper dbContext)
    {
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

    public IActionResult setLogin(string Email, string Password)
    {

      try
      {
        string passwordHash = hashPassword(Password);
        var user = _dbContext.Users.Where(b => b.Email == Email).FirstOrDefault();
        if (user.VerificationStatus == 1 && user.archived == false)
        {
          if (user.AdminVerification == 1)
          {
            if (user.PasswordHash == passwordHash)
            {
              Console.WriteLine("Passwort stimmt überein");
              try
              {
                HttpContext.Session.SetInt32("_UserID", user.Id);
                HttpContext.Session.SetString("_UserRole", user.Role);
                Console.WriteLine("Speichern auf der Session hat funktioniert: " + user.Id);
              }
              catch (Exception e)
              {
                Console.WriteLine("Fehler beim speichern auf der Session");
              }
              ViewBag.Message = string.Format("Du hast dich erfolgreich angemeldet");
              return View("~/Views/Account/Message.cshtml");
            }
            else
            {
              Console.WriteLine("Passwort ist falsch.");
              ViewBag.Message = string.Format("Dein Passwort ist Falsch");
              return View("~/Views/Account/Message.cshtml");
            }
          }
          else
          {
            Console.WriteLine("Dein Administrator muss zuerst noch deinen Account bestätigen.");
            ViewBag.Message = string.Format("Dein Administrator muss zuerst noch deinen Account bestätigen.");
            return View("~/Views/Account/Message.cshtml");
          }
        }
        else
        {
          if (user.archived == true)
          {
            Console.WriteLine("Ihr Account wurde archiviert, bitte melden sie sich bei einem Administrator.");
            ViewBag.Message = string.Format("Ihr Account wurde archiviert, bitte melden sie sich bei einem Administrator.");
            return View("~/Views/Account/Message.cshtml");
          }
          else
          {
            Console.WriteLine("Du hast deinen Account noch nicht verifiziert");
            ViewBag.Message = string.Format("Du hast deinen Account noch nicht verifiziert");
            return View("~/Views/Account/Message.cshtml");
          }

        }
      }
      catch (Exception e)
      {
        Console.WriteLine("User mit Email: '" + Email + "' exisitert noch nicht");
        ViewBag.Message = string.Format("Du hast deinen Account noch nicht verifiziert");
        return View("~/Views/Account/Message.cshtml");
      }

      //var allUsers = _dbContext.User.Where(b => b.KlassenId == Klassenid).ToList();
      //Console.Write("allUsers: ");
      //Console.WriteLine(allUsers);
    }

    public IActionResult RegisterCheck(string Email, string Vorname, string Nachname, string Password, string PasswordCheck, int klasseCode, string berufsbildner)
    {
      var userCount = _dbContext.Users.Where(b => b.Email == Email).Count();

      Console.WriteLine("Berufsbildner: " + berufsbildner);
      if (berufsbildner != "on")
      {
        Console.WriteLine("Email-Domain wird überprüft");
        if (checkMail(Email) == true)
        {
          Console.WriteLine("Email entspricht der Domain");
        }
        else
        {
          Console.WriteLine("Email hat eine ungültige Domain");
          ViewBag.Message = string.Format("Deine Emaildomain ist nicht erlabut.");
          return View("~/Views/Account/Register.cshtml");
        }
      }
      if (userCount != 0)
      {
        Console.WriteLine("Email existiert schon");
        ViewBag.Message = string.Format("Email existiert schon.");
        return View("~/Views/Account/Register.cshtml");
      }
      else
      {
        if (Password == PasswordCheck)
        {
          var klasseCount = _dbContext.Klasses.Where(b => b.KlassenInviteCode == klasseCode).Count();
          if (klasseCount != 0)
          {
            Klasse klasse = _dbContext.Klasses.Where(b => b.KlassenInviteCode == klasseCode).FirstOrDefault();

            Console.WriteLine("Passwörter stimmen überein");
            Random rnd = new Random();
            int VerificationKey = rnd.Next(100000, 1000000);

            var newUser = new User();

            newUser.Vorname = Vorname;
            newUser.Nachname = Nachname;
            newUser.Email = Email;
            newUser.Klasse = klasse;
            newUser.PasswordHash = hashPassword(Password);
            newUser.VerificationKey = VerificationKey;
            newUser.VerificationStatus = 0;
            if (berufsbildner == "on") // true
            {
              newUser.Role = "berufsbildner";
            }

            if (checkMailSubdomain(Email) == "edu")
            {
              newUser.Role = "schueler";
            }

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            HttpContext.Session.SetString("_RegisterEmail", Email);

            Console.Write("newUser: ");
            Console.WriteLine(newUser.ToString());

            Console.WriteLine("VerificationKey: " + VerificationKey);
          }
          else
          {
            Console.WriteLine("Die Ausgewählte Klasse existiert noch nicht.");
          }



          //sendMail();
        }
      }
      return View();
    }

    public bool checkMail(string Email)
    {
      //string subdomain = checkMailSubdomain(Email);
      string domain = checkMailDomain(Email);

      var domains = _dbContext.Domains.ToList();
      foreach (Domains currentDomain in domains)
      {
        if (currentDomain.AllowedDomains == domain)
        {
          Console.WriteLine("Whole Domain: " + domain);
          //Console.WriteLine("Subdomain: " + subdomain);
          return true;
        }
      }

      return false;
    }

    public string checkMailDomain(string Email)
    {
      Regex rg = new Regex(@"(?<=@)[^.]+([a-z0-9]+\.)*[a-z0-9]+\.[a-z]+");
      MatchCollection emailDomain = rg.Matches(Email);
      string emailDomainString = emailDomain[0].ToString();

      string[] domainParts = emailDomainString.Split('.');

      if (domainParts.Length > 2)
      {
        string returnVal = domainParts[1] + "." + domainParts[2];
        return returnVal;
      }
      else if (domainParts.Length == 2)
      {
        string returnVal = domainParts[0] + "." + domainParts[1];
        return returnVal;
      }
      return "";
    }

    public string checkMailSubdomain(string Email)
    {
      Regex rg = new Regex(@"(?<=@)[^.]+([a-z0-9]+\.)*[a-z0-9]+\.[a-z]+");
      MatchCollection emailDomain = rg.Matches(Email);
      string emailDomainString = emailDomain[0].ToString();

      string[] domainParts = emailDomainString.Split('.');

      if (domainParts.Length > 2)
      {
        return domainParts[0];
      }
      return "";
    }

    public IActionResult verify(int verificationKey) // string Email, 
    {
      string Email = HttpContext.Session.GetString("_RegisterEmail");
      Console.WriteLine("Email aus Session: " + Email);
      Console.WriteLine(verificationKey);
      try
      {

        var user = _dbContext.Users.Where(b => b.Email == Email).FirstOrDefault();
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

    public void sendMail()
    {
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
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    public static string hashPassword(string Password)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(Password);
      SHA256Managed sHA256ManagedString = new SHA256Managed();
      byte[] hash = sHA256ManagedString.ComputeHash(bytes);
      return Convert.ToBase64String(hash);
    }
  }
}
