using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

using NoVe.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Globalization;

namespace NoVe.Controllers
{
    public class LehrerController : Controller
    {
        private readonly DatabaseHelper _dbContext;
        public LehrerController(DatabaseHelper dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SchuelerListe()
        {
            return View(getUsersFromClass());
        }

        public IActionResult SetZeit()
        {
            return View(getSpecificClass());
        }

        public IActionResult KlasseBack()
        {
            return View("SchuelerListe", getUsersFromClass());
        }

        public IActionResult ZeitBack()
        {
            return View("Index", Index());
        }

        //private List<User> getUsersFromClass()
        //{
        //    int userId = (int)HttpContext.Session.GetInt32("_UserID");

        //    // Create Class - only for testing
        //    //Klasse klasse1 = _dbContext.Klasses.Where(k => k.Id == 1).FirstOrDefault();
        //    //User user1 = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
        //    //user1.Klasse = klasse1;
        //    //_dbContext.SaveChanges();

        //    //Set Student into Class - only for testing
        //    //Klasse klasse2 = _dbContext.Klasses.Where(k => k.Id == 1).FirstOrDefault();
        //    //User user2 = _dbContext.Users.Where(u => u.Id == 5).FirstOrDefault();
        //    //user2.Klasse = klasse2;
        //    //_dbContext.SaveChanges();

        //    List<User> currentUser = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).ToList();
        //    Klasse klasse = _dbContext.Klasses.Include(x => x.Users).Where(k => k.Id == currentUser[0].Klasse.Id).FirstOrDefault();

        //    List<User> users = new List<User>(klasse.Users.Count);

        //    foreach (User user in klasse.Users)
        //    {
        //        if (user.Role == "schueler")
        //        {
        //            users.Add(user);
        //        }
        //    }
        //    return users;
        //}

        private List<Klasse> getSpecificClass()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            List<User> currentUser = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).ToList();
            Klasse klasse = _dbContext.Klasses.Where(k => k.Id == currentUser[0].Klasse.Id).FirstOrDefault();

            List<Klasse> klasses = new List<Klasse>();
            klasses.Add(klasse);

            return klasses;
        }

        public async Task<IActionResult> ZeitSpeichern(string startdatum, string enddatum)
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");

            List<User> currentUser = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).ToList();
            Klasse klasse = _dbContext.Klasses.Where(k => k.Id == currentUser[0].Klasse.Id).FirstOrDefault();

            Regex rgDate = new Regex(@"(\d{1,2})\.(\d{1,2})\.(\d{4})");
            Regex rgTime = new Regex(@"([0-9][0-9]:[0-9][0-9])");

            // Format Startzeit
            MatchCollection startdatumArray = rgDate.Matches(startdatum);
            MatchCollection startzeitArray = rgTime.Matches(startdatum);
            string startzeitBackslash = startdatumArray[0].ToString().Replace(".", "/");
            string startdatumString = startzeitBackslash + " " + startzeitArray[0].ToString();

            // Format Endzeit
            MatchCollection enddatumArray = rgDate.Matches(enddatum);
            MatchCollection endzeitArray = rgTime.Matches(enddatum);
            string endzeitBackslash = enddatumArray[0].ToString().Replace(".", "/");
            string enddatumString = endzeitBackslash + " " + endzeitArray[0].ToString();

            klasse.Startdatum = DateTime.ParseExact(startdatumString, "dd/MM/yyyy HH:mm", null);
            klasse.EndDatum = DateTime.ParseExact(enddatumString, "dd/MM/yyyy HH:mm", null);
            _dbContext.SaveChanges();

            return View("Index", Index());
        }


        public IActionResult schuelerLoeschen(int ID)
        {
            User user = _dbContext.Users.Include(k => k.Klasse).FirstOrDefault(u => u.Id == ID);
            //Klasse klasse = _dbContext.Klasses.Include(u => u.Users).FirstOrDefault(k => k.Id == user.Klasse.Id);
            //klasse.Users.Remove(user);
            user.Klasse = null;
            _dbContext.SaveChanges();

            return View("SchuelerListe", getUsersFromClass());
        }

        public IActionResult notenEinsehen(int ID)
        {

            HttpContext.Session.SetInt32("_StudentID", ID);
            return RedirectToAction("Index", "Noten");
        }

        private List<UserWithMark> getUsersFromClass()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            User currentUser = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).FirstOrDefault();
            Klasse klasse = _dbContext.Klasses.Include(x => x.Users).Where(k => k.Id == currentUser.Klasse.Id).FirstOrDefault();

            List<UserWithMark> lerndeneListWithMarks = new List<UserWithMark>(klasse.Users.Count);

            foreach (User user in klasse.Users)
            {
                if (user.Role == "schueler")
                {
                    UserWithMark userWithMark = new UserWithMark();
                    userWithMark.Id = user.Id;
                    userWithMark.Email = user.Email;
                    userWithMark.Vorname = user.Vorname;
                    userWithMark.Nachname = user.Nachname;
                    userWithMark.Klasse = user.Klasse;
                    userWithMark.Role = user.Role;
                    userWithMark.LehrmeisterEmail = user.LehrmeisterEmail;
                    userWithMark.Firma = user.Firma;
                    userWithMark.archived = user.archived;
                    userWithMark.NotenWert = GesamtNote(user.Id);

                    lerndeneListWithMarks.Add(userWithMark);
                }
            }

            return lerndeneListWithMarks;
        }

        //private List<Klasse> getSpecificClass()
        //{
        //    int userId = (int)HttpContext.Session.GetInt32("_UserID");
        //    List<User> currentUser = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).ToList();
        //    Klasse klasse = _dbContext.Klasses.Where(k => k.Id == currentUser[0].Klasse.Id).FirstOrDefault();

        //    List<Klasse> klasses = new List<Klasse>();
        //    klasses.Add(klasse);

        //    return klasses;
        //}

        //public async Task<IActionResult> ZeitSpeichern(string startdatum, string enddatum)
        //{
        //    int userId = (int)HttpContext.Session.GetInt32("_UserID");

        //    List<User> currentUser = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).ToList();
        //    Klasse klasse = _dbContext.Klasses.Where(k => k.Id == currentUser[0].Klasse.Id).FirstOrDefault();

        //    Regex rgDate = new Regex(@"(\d{1,2})\.(\d{1,2})\.(\d{4})");
        //    Regex rgTime = new Regex(@"([0-9][0-9]:[0-9][0-9])");

        //    // Format Startzeit
        //    MatchCollection startdatumArray = rgDate.Matches(startdatum);
        //    MatchCollection startzeitArray = rgTime.Matches(startdatum);
        //    string startzeitBackslash = startdatumArray[0].ToString().Replace(".", "/");
        //    string startdatumString = startzeitBackslash + " " + startzeitArray[0].ToString();

        //    // Format Endzeit
        //    MatchCollection enddatumArray = rgDate.Matches(enddatum);
        //    MatchCollection endzeitArray = rgTime.Matches(enddatum);
        //    string endzeitBackslash = enddatumArray[0].ToString().Replace(".", "/");
        //    string enddatumString = endzeitBackslash + " " + endzeitArray[0].ToString();

        //    klasse.Startdatum = DateTime.ParseExact(startdatumString, "dd/MM/yyyy HH:mm", null);
        //    klasse.EndDatum = DateTime.ParseExact(enddatumString, "dd/MM/yyyy HH:mm", null);
        //    _dbContext.SaveChanges();

        //    return View("Index", Index());
        //}


        //public IActionResult schuelerLoeschen(int ID)
        //{
        //    User user = _dbContext.Users.Include(k => k.Klasse).FirstOrDefault(u => u.Id == ID);
        //    //Klasse klasse = _dbContext.Klasses.Include(u => u.Users).FirstOrDefault(k => k.Id == user.Klasse.Id);
        //    //klasse.Users.Remove(user);
        //    user.Klasse = null;
        //    _dbContext.SaveChanges();

        //    return View("SchuelerListe", getUsersFromClass());
        //}

        public double GesamtNote(int userId)
        {

            User user = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).FirstOrDefault();
            List<Kompetenzbereich> kompetenzbereiche = _dbContext.Kompetenzbereichs.Where(k => k.BerufId == user.Klasse.BerufId).ToList();
            double Gesamtnote = 0;
            double GewichtungWoNochKeineNote = 0;

            int i = 0;
            foreach (Kompetenzbereich kompetenzbereich in kompetenzbereiche)
            {
                // Hier wird die Gesammtnote aller Kompetenzbereiche berechnet
                double NotenwertKompetenzbereich = NotenController.runden(calcKompetenzbereichNote(userId, kompetenzbereich.Id), kompetenzbereich.Rundung);
                if (NotenwertKompetenzbereich == 0)
                {
                    GewichtungWoNochKeineNote = GewichtungWoNochKeineNote + kompetenzbereich.Gewichtung;
                }
                else
                {
                    Gesamtnote = (double)(Gesamtnote + NotenwertKompetenzbereich * kompetenzbereich.Gewichtung / 100);
                }

                i++;
            }

            int GewichtungDieBenotetWurde = (int)(100 - GewichtungWoNochKeineNote);
            double GesamtnoteZusammen = 0;
            if (GewichtungDieBenotetWurde != 0)
            {
                GesamtnoteZusammen = 100 / GewichtungDieBenotetWurde * Gesamtnote;
            }
            return GesamtnoteZusammen;
        }

        // Whole Method is the same as in NotenController
        // 1. Change runden() -> NotenController.runden()
        // 2. Change [param] calcKompetenzbereichNote(int kompetenzbereichId)  -> [param] calcKompetenzbereichNote(int userId, int kompetenzbereichId)
        public double calcKompetenzbereichNote(int userId, int kompetenzbereichId)
        {
            //int userId = (int)HttpContext.Session.GetInt32("_UserID");
            List<Fach> faecher = _dbContext.Fachs.Where(f => f.KompetenzbereichId == kompetenzbereichId).ToList();
            double kompetenzbereichSchnitt = 0;
            double gewichtungWoNochKeineNote = 0;

            foreach (Fach fach in faecher)
            {
                double notenWert = getNoteFromFach(fach.Id, userId);

                if (notenWert == 0)
                {
                    gewichtungWoNochKeineNote = gewichtungWoNochKeineNote + fach.Gewichtung;
                }
                else
                {
                    double rundung = fach.Rundung;
                    double gerundeteNote = NotenController.runden(notenWert, rundung);
                    kompetenzbereichSchnitt = (double)(kompetenzbereichSchnitt + gerundeteNote * fach.Gewichtung / 100);
                }
            }
            kompetenzbereichSchnitt = (double)(kompetenzbereichSchnitt + kompetenzbereichSchnitt * gewichtungWoNochKeineNote / 100);
            return kompetenzbereichSchnitt;
        }

        // Whole Method is the same as in NotenController
        public double getNoteFromFach(int fachId, int userId)
        {
            Note note = _dbContext.Notes.Where(n => n.FachId == fachId).Where(n => n.UserId == userId).FirstOrDefault();

            double notenwert = 0;
            if (note != null)
            {
                notenwert = note.Notenwert;
            }

            return notenwert;
        }

    }
}
