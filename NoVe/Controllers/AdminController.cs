using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.EntityFrameworkCore;
using NoVe.Controllers;

namespace NoVe.Models
{
    //todo nur admin darf
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _dbContext;
        public AdminController(DatabaseHelper dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            string userRole;

            try
            {
                userRole = HttpContext.Session.GetString("_UserRole");
                Console.WriteLine("SessionRole: " + userRole);
                if (userRole == "admin")
                {
                    return View(getUnconfirmedUsers());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fehler beim auslesen aus der Session");
            }

            ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
            return View("~/Views/Admin/message.cshtml");
            //return View(getUnconfirmedUsers());
        }

        public IActionResult BenutzerBestaetigen()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View(getUnconfirmedUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult AlleBenutzer()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View(getAllUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult BenutzerArchiv()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View(getAllUsersArchiv());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult Domains()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View(getAllDomains());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult KlassenVerwalten()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View(getAllKlassen());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult klassenEdit(int ID)
        {
            HttpContext.Session.SetInt32("_KlassenID", ID);
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View(getSpecificKlasse(ID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult KlassenArchiv()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View(getAllKlassenAchive());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult Berufe()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View(getBerufe());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult KlassenHinzufuegen()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View();
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult AddKlasse(string KlasseName, System.DateTime EingabedatumStart, System.DateTime EingabedatumEnde, string KlassenlehrerEmail, string BerufName)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                var newKlasse = new Klasse();
                newKlasse.KlasseName = KlasseName;
                newKlasse.Startdatum = EingabedatumStart;
                newKlasse.EndDatum = EingabedatumEnde;
                newKlasse.KlassenInviteCode = createKey();

                _dbContext.Klasses.Add(newKlasse);
                _dbContext.SaveChanges();

                return View("KlassenVerwalten", getAllKlassen());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult EditKlasse(string KlasseName, string EingabedatumStart, string EingabedatumEnde, string KlassenlehrerEmail, string BerufName)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int Id = (int)HttpContext.Session.GetInt32("_KlassenID");
                var editKlasse = _dbContext.Klasses.Where(x => x.Id == Id).FirstOrDefault();

                var EingabedatumStartDate = DateTime.Parse(EingabedatumStart);
                var EingabedatumEndeDate = DateTime.Parse(EingabedatumEnde);

                editKlasse.KlasseName = KlasseName;
                editKlasse.Startdatum = EingabedatumStartDate;
                editKlasse.EndDatum = EingabedatumEndeDate;

                _dbContext.SaveChanges();
                return View("KlassenVerwalten", getAllKlassen());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public int createKey()
        {
            Random rnd = new Random();
            int VerificationKey = rnd.Next(100000, 1000000);
            int keys = _dbContext.Klasses.Where(k => k.KlassenInviteCode == VerificationKey).Count();
            if (keys != 0)
            {
                return createKey();
            }
            else
            {
                return VerificationKey;
            }
        }

        public IActionResult BerufEdit(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                HttpContext.Session.SetInt32("_BerufID", ID);
                Beruf beruf = _dbContext.Berufs.Where(b => b.Id == ID).FirstOrDefault();
                ViewBag.Message = string.Format(beruf.Name);
                return View();
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult KompetenzbereichEdit(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                HttpContext.Session.SetInt32("_KompetenzbereichID", ID);
                Kompetenzbereich kompetenzbereich = _dbContext.Kompetenzbereichs.Where(b => b.Id == ID).FirstOrDefault();
                return View(getSpecificKompetenzbereich(ID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult FachEdit(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                HttpContext.Session.SetInt32("_FachID", ID);
                Fach fach = _dbContext.Fachs.Where(b => b.Id == ID).FirstOrDefault();
                return View(getSpecificFach(ID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult benutzerEdit(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                HttpContext.Session.SetInt32("_UserEditID", ID);
                return View(getSpecificUser(ID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult BenutzerArchive(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int userid = (int)HttpContext.Session.GetInt32("_UserID");
                if (userid == ID)
                {
                    ViewBag.Message = string.Format("Sie können sich selbst nicht archivieren");
                    return View("AlleBenutzer", getAllUsers());
                }

                User user = _dbContext.Users.Where(b => b.Id == ID).First();
                user.archived = true;
                _dbContext.SaveChanges();
                return View("AlleBenutzer", getAllUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }

        public IActionResult KlassenArchive(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                Klasse klasse = _dbContext.Klasses.Where(b => b.Id == ID).First();
                klasse.archived = true;
                _dbContext.SaveChanges();
                return View("KlassenVerwalten", getAllKlassen());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }


        public IActionResult BenutzerWiederherstellen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                User user = _dbContext.Users.Where(b => b.Id == ID).First();
                user.archived = false;
                _dbContext.SaveChanges();
                return View("BenutzerArchiv", getAllUsersArchiv());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }
        public IActionResult KlasseWiederherstellen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                Klasse klasse = _dbContext.Klasses.Where(b => b.Id == ID).First();
                klasse.archived = false;
                _dbContext.SaveChanges();
                return View("KlassenArchiv", getAllKlassenAchive());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }


        public IActionResult Kompetenzbereiche(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                HttpContext.Session.SetInt32("_BerufID", ID);
                Beruf beruf = _dbContext.Berufs.Where(b => b.Id == ID).FirstOrDefault();
                return View(getKompetenzbereiche(ID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }

        public IActionResult Faecher(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                HttpContext.Session.SetInt32("_KompetenzbereichID", ID);
                return View(getFaecher(ID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult KompetenzbereicheBack()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int id = (int)HttpContext.Session.GetInt32("_BerufID");
                return View("Kompetenzbereiche", getKompetenzbereiche(id));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }

        public IActionResult FachBack()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int id = (int)HttpContext.Session.GetInt32("_KompetenzbereichID");
                return View("Faecher", getFaecher(id));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }

        public IActionResult UserBack()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View("AlleBenutzer", getAllUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult ToIndexBack()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View("Index", getUnconfirmedUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult Bestaetigen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
                user.AdminVerification = 1;
                _dbContext.SaveChanges();

                AccountController.sendVerificationEmail(user.Email, 0, user.Vorname + " " + user.Nachname, "adminAccepted");

                return View("BenutzerBestaetigen", getUnconfirmedUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }

        public IActionResult KlasseLoeschen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                Klasse klasse = _dbContext.Klasses.Include(x => x.Users).FirstOrDefault(k => k.Id == ID);
                if (klasse.Users.Count == 0)
                {
                    _dbContext.Remove(klasse);
                    _dbContext.SaveChanges();
                    return View("KlassenVerwalten", getAllKlassen());
                }
                else
                {
                    ViewBag.Message = string.Format("Es hat noch Personen in der Klasse, diese zuerst entfernen oder die Klasse archivieren.");
                    return View("KlassenVerwalten", getAllKlassen());
                }
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }

        public async Task<IActionResult> AblehnenAsync(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();

                return View("BenutzerBestaetigen", getUnconfirmedUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public async Task<IActionResult> BenutzerLoeschen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();

                return View("AlleBenutzer", getAllUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public async Task<IActionResult> DomainLoeschen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                Domains domain = _dbContext.Domains.FirstOrDefault(d => d.Id == ID);
                _dbContext.Domains.Remove(domain);
                _dbContext.SaveChanges();

                return View("Domains", getAllDomains());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        private List<User> getUnconfirmedUsers()
        {

            return _dbContext.Users.Where(u => u.AdminVerification == 0).ToList();

        }

        private List<Kompetenzbereich> getSpecificKompetenzbereich(int Id)
        {
            return _dbContext.Kompetenzbereichs.Where(u => u.Id == Id).ToList();
        }

        private List<Klasse> getAllKlassen()
        {
            return _dbContext.Klasses.Where(k => k.archived == false).ToList();
        }

        private List<Klasse> getAllKlassenAchive()
        {
            return _dbContext.Klasses.Where(k => k.archived == true).ToList();
        }

        private List<Fach> getSpecificFach(int Id)
        {
            return _dbContext.Fachs.Where(u => u.Id == Id).ToList();
        }

        public List<User> getSpecificUser(int Id)
        {
            List<User> users = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == Id).ToList();
            if (users[0].Klasse == null)
            {
                Klasse klasse = new Klasse();
                klasse.KlassenInviteCode = 0;
                users[0].Klasse = klasse;
            }
            return users;
        }

        private List<Klasse> getSpecificKlasse(int Id)
        {
            return _dbContext.Klasses.Where(k => k.Id == Id).ToList(); ;
        }

        private List<User> getAllUsers()
        {

            return _dbContext.Users.Where(u => u.AdminVerification == 1 && u.archived == false).ToList();

        }

        private List<User> getAllUsersArchiv()
        {

            return _dbContext.Users.Where(u => u.AdminVerification == 1 && u.archived == true).ToList();

        }

        private List<Domains> getAllDomains()
        {

            return _dbContext.Domains.ToList();

        }

        private List<Beruf> getBerufe()
        {
            return _dbContext.Berufs.ToList();
        }

        private List<Kompetenzbereich> getKompetenzbereiche(int id)
        {
            return _dbContext.Kompetenzbereichs.Where(k => k.BerufId == id).ToList();
        }

        private List<Fach> getFaecher(int id)
        {
            return _dbContext.Fachs.Where(k => k.KompetenzbereichId == id).ToList();
        }

        public IActionResult SafeBerufe(string name)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                var berufSameNameCount = _dbContext.Berufs.Where(b => b.Name == name).Count();

                Console.WriteLine("Anzahl der Berufe: " + berufSameNameCount);
                Console.WriteLine("neuer Berufs-Name:" + name);

                if (berufSameNameCount < 1)
                {
                    Beruf beruf = new Beruf();
                    beruf.Name = name;

                    Console.WriteLine("neuer Berufs-Name:" + beruf.Name);

                    _dbContext.Berufs.Add(beruf);
                    _dbContext.SaveChanges();
                    return View("Berufe", getBerufe());
                }
                ViewBag.Message = string.Format("Es existiert schon ein Beruf mit diesem Namen.");
                return View("~/Views/Admin/message.cshtml");
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult SafeDomain(string name)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                var domainCount = _dbContext.Domains.Where(b => b.AllowedDomains == name).Count();

                Console.WriteLine("Anzahl der Domains: " + domainCount);
                Console.WriteLine("neuer Domain-Name:" + name);

                if (domainCount < 1)
                {
                    Domains domain = new Domains();
                    domain.AllowedDomains = name;

                    Console.WriteLine("neuer Berufs-Name:" + domain.AllowedDomains);

                    _dbContext.Domains.Add(domain);
                    _dbContext.SaveChanges();
                    return View("Domains", getAllDomains());
                }
                ViewBag.Message = string.Format("Es existiert schon eine Domain mit diesem Namen.");
                return View("~/Views/Admin/message.cshtml");
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public async Task<IActionResult> BerufLoeschen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                Beruf beruf = _dbContext.Berufs.FirstOrDefault(b => b.Id == ID);
                _dbContext.Berufs.Remove(beruf);
                _dbContext.SaveChanges();

                return View("Berufe", getBerufe());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public async Task<IActionResult> BerufBearbeiten(string name)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int id = (int)HttpContext.Session.GetInt32("_BerufID");
                Beruf beruf = _dbContext.Berufs.FirstOrDefault(b => b.Id == id);
                beruf.Name = name;
                _dbContext.SaveChanges();

                return View("Berufe", getBerufe());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }

        public async Task<IActionResult> KompetenzbereichLoeschen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int berufId = (int)HttpContext.Session.GetInt32("_BerufID");

                Kompetenzbereich kompetenzbereich = _dbContext.Kompetenzbereichs.FirstOrDefault(b => b.Id == ID);
                _dbContext.Kompetenzbereichs.Remove(kompetenzbereich);
                _dbContext.SaveChanges();

                return View("Kompetenzbereiche", getKompetenzbereiche(berufId));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult SafeKompetenzbereich(string name, int gewichtung, double rundung)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int berufId = (int)HttpContext.Session.GetInt32("_BerufID");
                var kompetenzbereichCount = _dbContext.Kompetenzbereichs.Where(b => b.Name == name).Where(b => b.BerufId == berufId).Count();


                if (kompetenzbereichCount < 1)
                {
                    Kompetenzbereich kompetenzbereich = new Kompetenzbereich();
                    kompetenzbereich.Name = name;
                    kompetenzbereich.Gewichtung = gewichtung;
                    kompetenzbereich.Rundung = rundung;
                    kompetenzbereich.BerufId = berufId;

                    _dbContext.Kompetenzbereichs.Add(kompetenzbereich);
                    _dbContext.SaveChanges();
                    return View("Kompetenzbereiche", getKompetenzbereiche(berufId));
                }
                ViewBag.Message = string.Format("Es existiert schon ein Kompetenzbereich mit diesem Namen in diesem Beruf.");
                return View("~/Views/Admin/message.cshtml");
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public async Task<IActionResult> KompetenzbereichBearbeiten(string name, int gewichtung, double rundung)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int kompetenzbereichID = (int)HttpContext.Session.GetInt32("_KompetenzbereichID");
                int berufId = (int)HttpContext.Session.GetInt32("_BerufID");

                Kompetenzbereich kompetenzbereich = _dbContext.Kompetenzbereichs.FirstOrDefault(b => b.Id == kompetenzbereichID);
                kompetenzbereich.Name = name;
                kompetenzbereich.Gewichtung = gewichtung;
                kompetenzbereich.Rundung = rundung;
                _dbContext.SaveChanges();

                return View("Kompetenzbereiche", getKompetenzbereiche(berufId));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }

        public IActionResult SafeFach(string name, int gewichtung, double rundung)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int kompetenzbereichId = (int)HttpContext.Session.GetInt32("_KompetenzbereichID");
                var fachCount = _dbContext.Fachs.Where(b => b.Name == name).Where(b => b.KompetenzbereichId == kompetenzbereichId).Count();


                if (fachCount < 1)
                {
                    Fach fach = new Fach();
                    fach.Name = name;
                    fach.Gewichtung = gewichtung;
                    fach.Rundung = rundung;
                    fach.KompetenzbereichId = kompetenzbereichId;

                    _dbContext.Fachs.Add(fach);
                    _dbContext.SaveChanges();
                    return View("Faecher", getFaecher(kompetenzbereichId));
                }
                ViewBag.Message = string.Format("Es existiert schon ein Fach mit diesem Namen in diesem Kompetenzbereich.");
                return View("~/Views/Admin/message.cshtml");
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public async Task<IActionResult> FachLoeschen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int kompetenzbereichId = (int)HttpContext.Session.GetInt32("_KompetenzbereichID");

                Fach fach = _dbContext.Fachs.FirstOrDefault(b => b.Id == ID);
                _dbContext.Fachs.Remove(fach);
                _dbContext.SaveChanges();

                return View("Faecher", getFaecher(kompetenzbereichId));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public async Task<IActionResult> FachBearbeiten(string name, int gewichtung, double rundung)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int kompetenzbereichID = (int)HttpContext.Session.GetInt32("_KompetenzbereichID");
                int FachID = (int)HttpContext.Session.GetInt32("_FachID");

                Fach fach = _dbContext.Fachs.FirstOrDefault(b => b.Id == FachID);
                fach.Name = name;
                fach.Gewichtung = gewichtung;
                fach.Rundung = rundung;
                _dbContext.SaveChanges();

                return View("Faecher", getFaecher(kompetenzbereichID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public async Task<IActionResult> UserBearbeiten(string vorname, string nachname, string email, string firma, string role, int klassencode, string lehrmeisterEmail)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int editUserId = (int)HttpContext.Session.GetInt32("_UserEditID");
                User user = _dbContext.Users.FirstOrDefault(b => b.Id == editUserId);
                user.Vorname = vorname;
                user.Nachname = nachname;
                user.Email = email;
                user.Firma = firma;
                user.Role = role;
                user.LehrmeisterEmail = lehrmeisterEmail;
                if (klassencode != 0)
                {
                    int KlassenCount = _dbContext.Klasses.Where(k => k.KlassenInviteCode == klassencode).Count();
                    if (KlassenCount != 0)
                    {
                        Klasse Klasse = _dbContext.Klasses.Include(x => x.Users).Where(k => k.KlassenInviteCode == klassencode).FirstOrDefault();
                        user.Klasse = Klasse;
                    }
                    else
                    {
                        ViewBag.Message = string.Format("Keine Klasse mit diesem Einladungscode gefunden.");
                        return View("benutzerEdit", getSpecificUser(editUserId));
                    }
                }
                _dbContext.SaveChanges();

                ViewBag.Message = string.Format("Alles erfolgreich gespeichert.");
                return View("AlleBenutzer", getAllUsers());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult ToKompetenzbereichBack()
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                int id = (int)HttpContext.Session.GetInt32("_BerufID");
                return View("Kompetenzbereiche", getKompetenzbereiche(id));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }

        }
    }
}
