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

        public IActionResult KlassenVerwalten()
        {
            return View();//todo klassen mitgeben
        }

        public IActionResult Berufe()
        {
            return View(getBerufe());
        }

        public IActionResult BerufEdit(int ID)
        {
            //int ID = 2; 
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

        public IActionResult Kompetenzbereiche(int ID)
        {
            HttpContext.Session.SetInt32("_BerufID", ID);
            Beruf beruf = _dbContext.Berufs.Where(b => b.Id == ID).FirstOrDefault();

            return View(getKompetenzbereiche(ID));
        }

        public IActionResult KompetenzbereicheBack()
        {
            int id = (int)HttpContext.Session.GetInt32("_BerufID");
            return View("Kompetenzbereiche", getKompetenzbereiche(id));
        }

        public IActionResult Bestaetigen(int ID)
        {

            User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
            user.AdminVerification = 1;
            _dbContext.SaveChanges();

            return View("Index", getUnconfirmedUsers());
        }

        public IActionResult KlasseLoeschen(int ID)
        {

            Klasse klasse = _dbContext.Klasses.FirstOrDefault(k => k.Id == ID);
            _dbContext.Remove(klasse);
            _dbContext.SaveChanges();

            return View("Index", getUnconfirmedUsers());
        }

        public async Task<IActionResult> AblehnenAsync(int ID)
        {
            User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return View("Index", getUnconfirmedUsers());
        }

        public async Task<IActionResult> BenutzerLoeschen(int ID)
        {
            User user = _dbContext.Users.FirstOrDefault(u => u.Id == ID);
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return View("AlleBenutzer", getAllUsers());
        }

        private List<User> getUnconfirmedUsers()
        {

            return _dbContext.Users.Where(u => u.AdminVerification == 0).ToList();

        }

        private List<Kompetenzbereich> getSpecificKompetenzbereich(int Id)
        {

            return _dbContext.Kompetenzbereichs.Where(u => u.Id == Id).ToList();

        }

        private List<User> getAllUsers()
        {

            return _dbContext.Users.Where(u => u.AdminVerification == 1).ToList();

        }

        private List<Beruf> getBerufe()
        {
            return _dbContext.Berufs.ToList();
        }

        private List<Kompetenzbereich> getKompetenzbereiche(int id)
        {
            return _dbContext.Kompetenzbereichs.Where(k => k.BerufId == id).ToList();
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
            var kompetenzbereichCount = _dbContext.Kompetenzbereichs.Where(b => b.Name == name).Count();


            if (kompetenzbereichCount < 1)
            {
                int berufId = (int)HttpContext.Session.GetInt32("_BerufID");

                Kompetenzbereich kompetenzbereich = new Kompetenzbereich();
                kompetenzbereich.Name = name;
                kompetenzbereich.Gewichtung = gewichtung;
                kompetenzbereich.Rundung = rundung;
                kompetenzbereich.BerufId = berufId;

                _dbContext.Kompetenzbereichs.Add(kompetenzbereich);
                _dbContext.SaveChanges();
                return View("Kompetenzbereiche", getKompetenzbereiche(berufId));
            }
            ViewBag.Message = string.Format("Es existiert schon ein Beruf mit diesem Namen.");
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
    }
}