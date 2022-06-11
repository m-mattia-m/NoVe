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
    public class BerufsbildnerController : Controller
    {
        private readonly DatabaseHelper _dbContext;
        public BerufsbildnerController(DatabaseHelper dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("_UserRole") == "berufsbildner")
            {
                return View("Lernende", getLernende());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult Lernende()
        {
            if (HttpContext.Session.GetString("_UserRole") == "berufsbildner")
            {
                return View(getLernende());
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult notenEinsehen(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "berufsbildner")
            {
                HttpContext.Session.SetInt32("_StudentID", ID);
                int studentID = (int)HttpContext.Session.GetInt32("_StudentID");
                Console.WriteLine("Noten einsehen von: " + studentID);

                return RedirectToAction("Index", "Noten");
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        private List<UserWithMark> getLernende()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            User currentUser = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();

            List<User> lernendelist = _dbContext.Users.Include(x => x.Klasse).Where(u => u.LehrmeisterEmail == currentUser.Email).ToList();
            List<UserWithMark> lerndeneListWithMarks = new List<UserWithMark>(lernendelist.Count);

            foreach (User user in lernendelist)
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
