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
using System.Threading.Tasks;

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
                if (user.VerificationStatus == 1)
                {
                    if (user.AdminVerification == 1)
                    {
                        if (user.archived == false)
                        {
                            DateTime dateTime = DateTime.Now;
                            DateTime dateTimeCheck = user.LoginFailedFrom.AddMinutes(5);
                            if (dateTime > dateTimeCheck || (dateTime > dateTimeCheck && user.LoginFailedCount < 4))
                            {
                                if (user.PasswordHash == passwordHash)
                                {
                                    Console.WriteLine("Passwort stimmt überein");
                                    try
                                    {
                                        HttpContext.Session.SetInt32("_UserID", user.Id);
                                        HttpContext.Session.SetString("_UserRole", user.Role);
                                        TempData["UserID"] = user.Id;
                                        user.LoginFailedCount = 0;
                                        _dbContext.SaveChanges();
                                        Console.WriteLine("Speichern auf der Session hat funktioniert '_UserID': " + user.Id);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Fehler beim speichern auf der Session");
                                    }
                                    return GetLandingPage();
                                }
                                else
                                {
                                    dateTime = DateTime.Now;
                                    dateTimeCheck = user.LoginFailedFrom.AddMinutes(5);
                                    if (user.LoginFailedCount == 3 && dateTime > dateTimeCheck)
                                    {
                                        user.LoginFailedFrom = dateTime;
                                    }
                                    user.LoginFailedCount = user.LoginFailedCount + 1;
                                    _dbContext.SaveChanges();
                                    Console.WriteLine("Passwort ist falsch.");
                                    if (user.LoginFailedCount >= 3)
                                    {
                                        ViewBag.Message = string.Format("Sie haben das Passwort zu viel Mal falsch eingegeben, versuchen Sie es in 5 Minuten nochmals.");
                                    }
                                    else
                                    {
                                        ViewBag.Message = string.Format("Dein Passwort ist Falsch");
                                    }
                                    return View("~/Views/Account/Message.cshtml");
                                }
                            }
                            else
                            {
                                ViewBag.Message = string.Format("Sie haben sich zu viel mal falsch eingeloggt, versuchen sie es in 5 Minuten nochmals.");
                                return View("~/Views/Account/Message.cshtml");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ist archiviert.");
                            ViewBag.Message = string.Format("Du wurdest archiviert, melde dich beim Admin.");
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
                    Console.WriteLine("Du hast deinen Account noch nicht verifiziert");
                    ViewBag.Message = string.Format("Du hast deinen Account noch nicht verifiziert");
                    return View("VerifyEmailLater", VerifyEmailLater());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("User mit Email: '" + Email + "' exisitert noch nicht");
                ViewBag.Message = string.Format("Es gibt noch keinen Account mit dieser Email, bitte registriere dich.");
                return View("~/Views/Account/Message.cshtml");
            }

            //var allUsers = _dbContext.User.Where(b => b.KlassenId == Klassenid).ToList();
            //Console.Write("allUsers: ");
            //Console.WriteLine(allUsers);
        }

        public IActionResult RegisterCheck(string Email, string Vorname, string Nachname, string Password, string PasswordCheck, int klasseCode, string berufsbildner)
        {
            // Check if fields are empty
            if (string.IsNullOrEmpty(Email))
            {
                ViewBag.Message = string.Format("Die Email darf nicht leer sein.");
                return View("~/Views/Account/Register.cshtml");
            }
            if (string.IsNullOrEmpty(Vorname))
            {
                ViewBag.Message = string.Format("Der Vorname darf nicht leer sein.");
                return View("~/Views/Account/Register.cshtml");
            }
            if (string.IsNullOrEmpty(Nachname))
            {
                ViewBag.Message = string.Format("Der Nachname darf nicht leer sein.");
                return View("~/Views/Account/Register.cshtml");
            }
            if (string.IsNullOrEmpty(Password))
            {
                ViewBag.Message = string.Format("Das Passwort darf nicht leer sein.");
                return View("~/Views/Account/Register.cshtml");
            }
            if (string.IsNullOrEmpty(PasswordCheck))
            {
                ViewBag.Message = string.Format("Das Passwort bestätigen darf nicht leer sein.");
                return View("~/Views/Account/Register.cshtml");
            }

            // check if domain allowed
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
            // Check if already exist
            var userCount = _dbContext.Users.Where(b => b.Email == Email).Count();
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
                    Console.WriteLine("Passwörter stimmen überein");
                    if (PasswortPruefen(Password) == false)
                    {
                        ViewBag.Message = string.Format("Passwort zu schwach. Passwort muss mindestens 1 Kleinbuchstabe, 1 Grossbuchstabe, 1 Zahl und 1 Sonderzeichen enthalten. Ausserdem muss das Passwort minimum 8 Zeichen lange sein.");
                        return View("~/Views/Account/Register.cshtml");
                    }
                    Random rnd = new Random();
                    int VerificationKey = rnd.Next(100000, 1000000);

                    var newUser = new User();

                    newUser.Vorname = Vorname;
                    newUser.Nachname = Nachname;
                    newUser.Email = Email;
                    // prueft ob eine Klasse zugewiesen werden kann.
                    if (klasseCode != 0)
                    {
                        var klasseCount = _dbContext.Klasses.Where(b => b.KlassenInviteCode == klasseCode).Count();
                        if (klasseCount != 0)
                        {
                            Klasse klasse = _dbContext.Klasses.Where(b => b.KlassenInviteCode == klasseCode).FirstOrDefault();
                            newUser.Klasse = klasse;
                        }
                        else
                        {
                            ViewBag.Message = string.Format("Klasse mit diesem Invite-Code ist nicht vorhanden: " + klasseCode);
                            return View("~/Views/Account/Register.cshtml");
                        }
                    }

                    newUser.PasswordHash = hashPassword(Password);
                    newUser.VerificationKey = VerificationKey;
                    newUser.VerificationStatus = 0;
                    if (berufsbildner == "on") // true
                    {
                        newUser.Role = "berufsbildner";
                    }
                    if (berufsbildner != "on" && klasseCode == 0) // true
                    {
                        newUser.Role = "lehrer";
                    }
                    if (checkMailSubdomain(Email) == "edu" || klasseCode != 0)
                    {
                        newUser.Role = "schueler";
                    }

                    _dbContext.Users.Add(newUser);
                    _dbContext.SaveChanges();

                    HttpContext.Session.SetString("_RegisterEmail", Email);

                    Console.Write("newUser: ");

                    Console.WriteLine("Email -> VerificationKey: " + VerificationKey);
                    sendVerificationEmail(Email, VerificationKey, Vorname + " " + Nachname, "erstVerifizierung");
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

        public IActionResult Logout()
        {
            HttpContext.Session.SetInt32("_UserID", -1);
            HttpContext.Session.SetString("_UserRole", "");
            TempData["UserID"] = null;
            return View("../Home/Index");
        }

        private IActionResult GetLandingPage()
        {
            string Role = HttpContext.Session.GetString("_UserRole");
            if (Role.Equals("admin"))
            {
                return View("../Admin/Index");
            }
            else if (Role.Equals("lehrer"))
            {
                return View("../Lehrer/Index");
            }
            else if (Role.Equals("schueler"))
            {
                //return View("../Noten/Index");
                return RedirectToAction("Index", "Noten");
            }
            else if (Role.Equals("berufsbildner"))
            {
                //return View("../Berufsbildner/Index");
                return RedirectToAction("Index", "Berufsbildner");
            }
            return View();
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
                    ViewBag.Message = string.Format("Ungültiger Verifizierungscode, versuchen Sie es erneut.");
                    return View("Login");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("User mit Email: '" + Email + "' exisitert noch nicht");
            }

            ViewBag.Message = string.Format("Sie wurden erfolgreich verifiziert, aktuell muss Sie noch ein Administrator freischalten.");
            return View("Login");
        }

        public static string hashPassword(string Password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Password);
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public IActionResult profile()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            if (userId != 0)
            {
                return View(getSpecificUser(userId));
            }
            else
            {
                ViewBag.Message = string.Format("Sie müssen angemeldet sein, um das Profil zu öffnen.");
                return View("~/Views/Account/Message.cshtml");
            }
        }

        public IActionResult changeRole()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            string currentUserRole = HttpContext.Session.GetString("_UserRole");
            if (userId != 0)
            {
                User user = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (user.isAdmin == true)
                {
                    if (currentUserRole == "lehrer" || currentUserRole == "berufsbildner" || currentUserRole == "schueler")
                    {
                        HttpContext.Session.SetString("_UserRole", "admin");
                        Console.WriteLine("New Userrole: " + "admin (isAdmin)");

                        return RedirectToAction("Index", "Admin");
                    }
                    else if (currentUserRole == "admin")
                    {
                        HttpContext.Session.SetString("_UserRole", user.Role);
                        Console.WriteLine("New Userrole: " + user.Role);
                        if (user.Role.Equals("lehrer"))
                        {
                            return View("../Lehrer/Index");
                        }
                        else if (user.Role.Equals("schueler"))
                        {
                            return RedirectToAction("Index", "Noten");
                        }
                        else if (user.Role.Equals("berufsbildner"))
                        {
                            return RedirectToAction("Index", "Berufsbildner");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Undefinierter Fehler ist aufgetreten, sie haben eine Rolle die nicht existiert, melden Sie sich beim Administrator mit dieser Fehlermeldung.");
                        ViewBag.Message = string.Format("Undefinierter Fehler ist aufgetreten, sie haben eine Rolle die nicht existiert, melden Sie sich beim Administrator mit dieser Fehlermeldung.");
                        return View("~/Views/Account/Message.cshtml");
                    }
                }
                else
                {
                    ViewBag.Message = string.Format("Sie haben keine Berechtigung für diese Aktion.");
                    return View("~/Views/Account/Message.cshtml");
                }
            }
            else
            {
                ViewBag.Message = string.Format("Sie müssen angemeldet sein, um diese Aktion zu machen.");
                return View("~/Views/Account/Message.cshtml");
            }
        }

        public List<User> getSpecificUser(int Id)
        {
            return _dbContext.Users.Where(u => u.Id == Id).ToList();
        }

        public async Task<IActionResult> ProfilSpeichern(string vorname, string nachname, string email, string firma, string lehrmeisterEmail, string passwort, string passwortCheck)
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");

            // Check if fields are empty
            if (string.IsNullOrEmpty(vorname))
            {
                ViewBag.Message = string.Format("Der Vorname darf nicht leer sein.");
                return View("profile", getSpecificUser(userId));
            }
            if (string.IsNullOrEmpty(nachname))
            {
                ViewBag.Message = string.Format("Der Nachname darf nicht leer sein.");
                return View("profile", getSpecificUser(userId));
            }
            if (string.IsNullOrEmpty(firma))
            {
                ViewBag.Message = string.Format("Die Firma darf nicht leer sein.");
                return View("profile", getSpecificUser(userId));
            }
            if (string.IsNullOrEmpty(lehrmeisterEmail))
            {
                ViewBag.Message = string.Format("Die Email ihres Lehrmeisters darf nicht leer sein.");
                return View("profile", getSpecificUser(userId));
            }
            if (string.IsNullOrEmpty(passwort))
            {
                ViewBag.Message = string.Format("Das Passwort darf nicht leer sein.");
                return View("profile", getSpecificUser(userId));
            }
            if (string.IsNullOrEmpty(passwortCheck))
            {
                ViewBag.Message = string.Format("Das Passwortbestätigen-Feld darf nicht leer sein.");
                return View("profile", getSpecificUser(userId));
            }



            User user = _dbContext.Users.FirstOrDefault(b => b.Id == userId);
            user.Vorname = vorname;
            user.Nachname = nachname;
            //user.Email = email;
            user.Firma = firma;
            user.LehrmeisterEmail = lehrmeisterEmail;
            _dbContext.SaveChanges();

            if (passwort != null && passwortCheck != null)
            {
                if (PasswortPruefen(passwort) == false)
                {
                    ViewBag.Message = string.Format("Passwort entspricht nicht den Anforderungen.");
                    return View("profile", getSpecificUser(userId));
                }
                if (passwort == passwortCheck)
                {
                    string passwordhash = hashPassword(passwort);
                    user.PasswordHash = passwordhash;
                    _dbContext.SaveChanges();
                }
                else
                {
                    ViewBag.Message = string.Format("Passwort und Passwort-Bestätigen ist unterschiedlich");
                    return View("profile", getSpecificUser(userId));
                }
            }


            ViewBag.Message = string.Format("Angaben wurden gespeichert");
            return View("profile", getSpecificUser(userId));
        }

        public IActionResult PasswordForgotten()
        {
            return View();
        }

        public IActionResult PasswordForgottenVerify(string email)
        {
            Random rnd = new Random();
            int VerificationKey = rnd.Next(100000, 1000000);

            HttpContext.Session.SetString("_VerifyEmail", email);

            User user = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();
            user.PasswordForgottenVerifyKey = VerificationKey;
            user.PasswordForgottenValidFrom = DateTime.Now;
            _dbContext.SaveChanges();

            Console.WriteLine("Email -> Passwort-Zurücksetz-Code: " + VerificationKey);
            sendVerificationEmail(email, VerificationKey, user.Vorname + " " + user.Nachname, "passwordChange");
            ViewBag.Message = string.Format("Sie haben eine Stunde Zeit um ihr Passwort zurückzusetzen, danach müssen Sie einen neuen Code anfordern.");
            return View("~/Views/Account/PasswordForgottenVerify.cshtml");
        }

        public IActionResult PasswordForgottenCheckVerifyKey(int verifyCode)
        {
            string email = HttpContext.Session.GetString("_VerifyEmail");
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddHours(1);

            User user = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user.PasswordForgottenValidFrom < dateTime)
            {
                if (user.PasswordForgottenVerifyKey == verifyCode)
                {
                    return View("~/Views/Account/SetNewPassword.cshtml");
                }
            }

            ViewBag.Message = string.Format("Der Verifizierungs-Code ist Falsch.");
            return View("~/Views/Account/Message.cshtml");
        }

        public IActionResult SetNewPassword(string passwort, string passwortCheck)
        {
            string email = HttpContext.Session.GetString("_VerifyEmail");
            User user = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();

            if (PasswortPruefen(passwort) == false)
            {
                ViewBag.Message = string.Format("Passwort entspricht nicht den Anforderungen.");
                return View("SetNewPassword");
            }

            if (passwort != null && passwortCheck != null)
            {
                if (passwort == passwortCheck)
                {
                    string passwordhash = hashPassword(passwort);
                    user.PasswordHash = passwordhash;
                    _dbContext.SaveChanges();
                    ViewBag.Message = string.Format("Passwort wurde erfolgreich gespeichert.");
                    sendVerificationEmail(email, 0, user.Vorname + " " + user.Nachname, "successfullyPasswordChange");
                    return View("Login");
                }
                else
                {
                    ViewBag.Message = string.Format("Passwort und Passwort-Bestätigen ist unterschiedlich");
                    return View("SetNewPassword");
                }
            }
            else
            {
                ViewBag.Message = string.Format("Bitte füllen Sie beide Passwortfelder aus.");
                return View("SetNewPassword");
            }
        }

        public IActionResult VerifyEmailLater()
        {
            return View("~/Views/Account/VerifyEmailLaater.cshtml");
        }

        public IActionResult VerifyEmailLaterRegenreate(string email)
        {
            HttpContext.Session.SetString("_RegisterEmail", email);
            Random rnd = new Random();
            int VerificationKey = rnd.Next(100000, 1000000);

            User user = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();
            user.VerificationKey = VerificationKey;
            _dbContext.SaveChanges();

            Console.WriteLine("Email -> VerificationKeyLater: " + VerificationKey);
            sendVerificationEmail(email, VerificationKey, user.Vorname + " " + user.Nachname, "lastVerifizierung");

            return View("~/Views/Account/RegisterCheck.cshtml");
        }

        public static void sendVerificationEmail(string toEmail, int verificationKey, string name, string mailType)
        {
            string message = "";

            if (mailType == "erstVerifizierung")
            {
                message = "<h1>Grüezi" + name + ",</h1>" +
                "Bitte geben Sie diesen Verifizierungscode bei der Registrierung ein, um sich fertig zu registrieren.<br><br><br>" +
                "Verifizierungscode: <b>" + verificationKey + " </b><br><br><br>" +
                "Das NoVe-Team";
            }
            else if (mailType == "lastVerifizierung")
            {
                message = "<h1>Grüezi" + name + ",</h1>" +
                "Sie haben einen neuen Verifizierungscode angefordert, um sich fertig zu registrieren.<br><br><br>" +
                "<b>Verifizierungscode: " + verificationKey + "<b><b><b>" +
                "Das NoVe-Team";
            }
            else if (mailType == "passwordChange")
            {
                message = "<h1>Grüezi" + name + ",</h1>" +
                "Um Ihr Passwort zu ändern, geben sie diesen Verifizierungscode ein. Beachten Sie, dass dieser Code nur eine Stunde gültig ist.<br><br><br>" +
                "<b>Verifizierungscode: " + verificationKey + "<b><b><b>" +
                "Das NoVe-Team";
            }
            else if (mailType == "successfullyPasswordChange")
            {
                message = "<h1>Grüezi" + name + ",</h1>" +
                "Sie haben Ihr Passwort erfogreich geändert.<br><br><br>" +
                "Das NoVe-Team";
            }
            else if (mailType == "adminAccepted")
            {
                message = "<h1>Grüezi" + name + ",</h1>" +
                "Ihr Administrator hat sie angenommen, sie können jetzt NoVe beitreten.<br><br><br>" +
                "Das NoVe-Team";
            }

            MailingController.sendEmail(toEmail, message);
        }

        public static bool PasswortPruefen(string password)
        {
            string regex = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$";
            bool status = Regex.Match(password, regex).Success;
            return status;
        }
    }
}