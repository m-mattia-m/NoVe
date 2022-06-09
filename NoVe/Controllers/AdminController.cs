using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

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
            return View(getUnconfirmedUsers());
        }

        public IActionResult AlleBenutzer()
        {
            return View(getAllUsers());
        }

        public IActionResult BenutzerArchiv()
        {
          return View(getAllUsersArchiv());
        }

        public IActionResult Domains()
        {
          return View(getAllDomains());
        }

        public IActionResult KlassenVerwalten()
        {
            return View(getAllKlassen());
        }

        public IActionResult klassenEdit(int ID)
        {
          HttpContext.Session.SetInt32("_KlassenID", ID);
          return View(getSpecificKlasse(ID));
        }

        public IActionResult KlassenArchiv()
        {
          return View(getAllKlassenAchive());
        }

        public IActionResult Berufe()
        {
            return View(getBerufe());
        }

        public IActionResult KlassenHinzufuegen()
        {
          return View();
        }

        public IActionResult AddKlasse(string KlasseName, System.DateTime EingabedatumStart, System.DateTime EingabedatumEnde, string KlassenlehrerEmail, string BerufName)
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

        public IActionResult EditKlasse(string KlasseName, string EingabedatumStart, string EingabedatumEnde, string KlassenlehrerEmail, string BerufName)
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
        
        public int createKey()
        {
          Random rnd = new Random();
          int VerificationKey = rnd.Next(100000, 1000000);
          int keys = _dbContext.Klasses.Where(k => k.KlassenInviteCode == VerificationKey).Count();
          if (keys != 0)
          {
            return createKey();
          } else
          {
            return VerificationKey;
          }
        }

        public IActionResult BerufEdit(int ID)
        {
            HttpContext.Session.SetInt32("_BerufID", ID);
            Beruf beruf = _dbContext.Berufs.Where(b => b.Id == ID).FirstOrDefault();
            ViewBag.Message = string.Format(beruf.Name);
            return View();
        }

        public IActionResult KompetenzbereichEdit(int ID)
        {
            HttpContext.Session.SetInt32("_KompetenzbereichID", ID);
            Kompetenzbereich kompetenzbereich = _dbContext.Kompetenzbereichs.Where(b => b.Id == ID).FirstOrDefault();
            return View(getSpecificKompetenzbereich(ID));
        }

        public IActionResult FachEdit(int ID)
        {
            HttpContext.Session.SetInt32("_FachID", ID);
            Fach fach = _dbContext.Fachs.Where(b => b.Id == ID).FirstOrDefault();
            return View(getSpecificFach(ID));
        }

        public IActionResult benutzerEdit(int ID)
        {
            HttpContext.Session.SetInt32("_UserEditID", ID);
            return View(getSpecificUser(ID));
        }

        public IActionResult BenutzerArchive(int ID)
        {
          User user = _dbContext.Users.Where(b => b.Id == ID).First();
          user.archived = true;
          _dbContext.SaveChanges();
          return View("AlleBenutzer", getAllUsers());
        }

        public IActionResult KlassenArchive(int ID)
        {
          Klasse klasse = _dbContext.Klasses.Where(b => b.Id == ID).First();
          klasse.archived = true;
          _dbContext.SaveChanges();
          return View("KlassenVerwalten", getAllKlassen());
        }


        public IActionResult BenutzerWiederherstellen(int ID)
        {
          User user = _dbContext.Users.Where(b => b.Id == ID).First();
          user.archived = false;
          _dbContext.SaveChanges();
          return View("BenutzerArchiv", getAllUsersArchiv());
        }
        public IActionResult KlasseWiederherstellen(int ID)
        {
          Klasse klasse = _dbContext.Klasses.Where(b => b.Id == ID).First();
          klasse.archived = false;
          _dbContext.SaveChanges();
          return View("KlassenArchiv", getAllKlassenAchive());
        }


        public IActionResult Kompetenzbereiche(int ID)
        {
            HttpContext.Session.SetInt32("_BerufID", ID);
            Beruf beruf = _dbContext.Berufs.Where(b => b.Id == ID).FirstOrDefault();

            return View(getKompetenzbereiche(ID));
        }

        public IActionResult Faecher(int ID)
        {
            HttpContext.Session.SetInt32("_KompetenzbereichID", ID);

            return View(getFaecher(ID));
        }

        public IActionResult KompetenzbereicheBack()
        {
            int id = (int)HttpContext.Session.GetInt32("_BerufID");
            return View("Kompetenzbereiche", getKompetenzbereiche(id));
        }

        public IActionResult FachBack()
        {
            int id = (int)HttpContext.Session.GetInt32("_KompetenzbereichID");
            return View("Faecher", getFaecher(id));
        }

        public IActionResult UserBack()
        {
            return View("AlleBenutzer", getAllUsers());
        }

        public IActionResult ToIndexBack()
        {
            return View("Index", getUnconfirmedUsers());
        }

        public IActionResult Bestaetigen(int ID)
        {

            User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
            user.AdminVerification = 1;
            _dbContext.SaveChanges();

            return View("BenutzerBestaetigen", getUnconfirmedUsers());
        }

        public IActionResult KlasseLoeschen(int ID)
        {
            Klasse klasse = _dbContext.Klasses.FirstOrDefault(k => k.Id == ID);
            _dbContext.Remove(klasse);
            _dbContext.SaveChanges();

            return View("KlassenVerwalten", getAllKlassen());
        }

        public async Task<IActionResult> AblehnenAsync(int ID)
        {
            User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return View("BenutzerBestaetigen", getUnconfirmedUsers());
        }

        public async Task<IActionResult> BenutzerLoeschen(int ID)
        {
            User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return View("AlleBenutzer", getAllUsers());
        }

        public async Task<IActionResult> DomainLoeschen(int ID)
        {
          Domains domain = _dbContext.Domains.FirstOrDefault(d => d.Id == ID);
          _dbContext.Domains.Remove(domain);
          _dbContext.SaveChanges();

          return View("Domains", getAllDomains());
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

        private List<User> getSpecificUser(int Id)
        {
            return _dbContext.Users.Where(u => u.Id == Id).ToList();
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

        public IActionResult SafeBerufe(string name) {
            var berufSameNameCount = _dbContext.Berufs.Where(b => b.Name == name).Count();

            Console.WriteLine("Anzahl der Berufe: " + berufSameNameCount);
            Console.WriteLine("neuer Berufs-Name:" + name);

            if (berufSameNameCount < 1) {
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

        public IActionResult SafeDomain(string name)
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

        public async Task<IActionResult> BerufLoeschen(int ID)
        {
            Beruf beruf = _dbContext.Berufs.FirstOrDefault(b => b.Id == ID);
            _dbContext.Berufs.Remove(beruf);
            _dbContext.SaveChanges();

            return View("Berufe", getBerufe());
        }

        public async Task<IActionResult> BerufBearbeiten(string name)
        {
            int id = (int)HttpContext.Session.GetInt32("_BerufID");
            Beruf beruf = _dbContext.Berufs.FirstOrDefault(b => b.Id == id);
            beruf.Name = name;
            _dbContext.SaveChanges();

            return View("Berufe", getBerufe());
        }

        public async Task<IActionResult> KompetenzbereichLoeschen(int ID)
        {
            int berufId = (int)HttpContext.Session.GetInt32("_BerufID");

            Kompetenzbereich kompetenzbereich = _dbContext.Kompetenzbereichs.FirstOrDefault(b => b.Id == ID);
            _dbContext.Kompetenzbereichs.Remove(kompetenzbereich);
            _dbContext.SaveChanges();

            return View("Kompetenzbereiche", getKompetenzbereiche(berufId));
        }

        public IActionResult SafeKompetenzbereich(string name, int gewichtung, double rundung)
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

        public async Task<IActionResult> KompetenzbereichBearbeiten(string name, int gewichtung, double rundung)
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

        public IActionResult SafeFach(string name, int gewichtung, double rundung)
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

        public async Task<IActionResult> FachLoeschen(int ID)
        {
            int kompetenzbereichId = (int)HttpContext.Session.GetInt32("_KompetenzbereichID");

            Fach fach = _dbContext.Fachs.FirstOrDefault(b => b.Id == ID);
            _dbContext.Fachs.Remove(fach);
            _dbContext.SaveChanges();

            return View("Faecher", getFaecher(kompetenzbereichId));
        }

        public async Task<IActionResult> FachBearbeiten(string name, int gewichtung, double rundung)
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

        public async Task<IActionResult> UserBearbeiten(string vorname, string nachname, string email, string firma, string role)
        {
            int id = (int)HttpContext.Session.GetInt32("_UserEditID");
            User user = _dbContext.Users.FirstOrDefault(b => b.Id == id);
            user.Vorname = vorname;
            user.Nachname = nachname;
            user.Email = email;
            user.Firma = firma;
            user.Role = role;
            _dbContext.SaveChanges();

            return View("AlleBenutzer", getAllUsers());
        }
    }
}
