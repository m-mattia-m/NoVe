﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NoVe.Models;
using Microsoft.EntityFrameworkCore;

namespace NoVe.Controllers
{
    public class NotenController : Controller
    {
        private readonly DatabaseHelper _dbContext;
        public NotenController(DatabaseHelper dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            string userRole;
            userRole = HttpContext.Session.GetString("_UserRole");
            Console.WriteLine("SessionRole: " + userRole);
            if (userRole == "schueler" || userRole == "berufsbildner" || userRole == "lehrer" || userRole == "admin")
            {
                Console.WriteLine("UserID: " + (int)HttpContext.Session.GetInt32("_UserID"));

                List<FachKompetenzbereich> fachKompetenzbereiche = listAllFaecherAndKompetenzbereiche();
                if (fachKompetenzbereiche == null)
                {
                    ViewBag.Message = string.Format("Du bist keiner Klasse zugeordnet, melde dich bei einer Klasse an");
                    return View("~/Views/Account/Message.cshtml");
                }
                return View(fachKompetenzbereiche);
            }
            ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
            return View("~/Views/Account/Message.cshtml");
        }

        private List<FachKompetenzbereich> listAllFaecherAndKompetenzbereiche()
        {
            string userRole = HttpContext.Session.GetString("_UserRole");
            Console.WriteLine("SessionRole: " + userRole);
            int userId = -1;
            if (userRole == "lehrer" || userRole == "berufsbildner")
            {
                userId = (int)HttpContext.Session.GetInt32("_StudentID");
            }
            else
            {
                userId = (int)HttpContext.Session.GetInt32("_UserID");
            }

            User user = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).FirstOrDefault();
            if (user.Klasse == null)
            {
                return null;
            }
            List<Kompetenzbereich> kompetenzbereiche = _dbContext.Kompetenzbereichs.Where(k => k.BerufId == user.Klasse.BerufId).ToList();
            List<FachKompetenzbereich> fachKompetenzbereiche = new List<FachKompetenzbereich>();
            Klasse klasse = _dbContext.Klasses.Where(k => k.Id == user.Klasse.Id).FirstOrDefault();
            double Gesamtnote = 0;
            double GewichtungWoNochKeineNote = 0;

            DateTime localDate = DateTime.Now;
            int InEditTime = 0;

            if (userRole == "schueler")
            {
                if (klasse.Startdatum < localDate && klasse.EndDatum > localDate)
                {
                    InEditTime = 1;
                }
                else
                {
                    InEditTime = 0;
                }
            }

            int i = 0;
            foreach (Kompetenzbereich kompetenzbereich in kompetenzbereiche)
            {
                FachKompetenzbereich fachKompetenzbereich = new FachKompetenzbereich();
                fachKompetenzbereich.Id = i;
                fachKompetenzbereich.UserId = userId;
                fachKompetenzbereich.UserRole = HttpContext.Session.GetString("_UserRole");
                fachKompetenzbereich.UserName = user.Vorname + " " + user.Nachname;
                fachKompetenzbereich.Startdatum = klasse.Startdatum;
                fachKompetenzbereich.EndDatum = klasse.EndDatum;
                fachKompetenzbereich.InEditTime = InEditTime;
                fachKompetenzbereich.Kompetenzbereich = kompetenzbereich;
                fachKompetenzbereich.NoteView = GetNoteView(userId, kompetenzbereich.Id);
                fachKompetenzbereich.NotenwertKompetenzbereich = runden(calcKompetenzbereichNote(kompetenzbereich.Id), kompetenzbereich.Rundung);

                fachKompetenzbereiche.Add(fachKompetenzbereich);

                // Hier wird die Gesammtnote aller Kompetenzbereiche berechnet
                if (fachKompetenzbereich.NotenwertKompetenzbereich == 0)
                {
                    GewichtungWoNochKeineNote = GewichtungWoNochKeineNote + kompetenzbereich.Gewichtung;
                }
                else
                {
                    Gesamtnote = (double)(Gesamtnote + fachKompetenzbereich.NotenwertKompetenzbereich * kompetenzbereich.Gewichtung / 100);
                }

                i++;
            }

            // --------------------------

            //kompetenzbereichSchnitt = (double)(kompetenzbereichSchnitt + kompetenzbereichSchnitt * gewichtungWoNochKeineNote / 100);
            double gewichtungWoNote = 100 - GewichtungWoNochKeineNote;
            double GesamtnoteZusammen = (double)(100 / gewichtungWoNote * Gesamtnote);


            // --------------------------

            //int GewichtungDieBenotetWurde = (int)(100 - GewichtungWoNochKeineNote);
            //double GesamtnoteZusammen = 0;
            //if (GewichtungDieBenotetWurde != 0)
            //{
            //    GesamtnoteZusammen = 100 / GewichtungDieBenotetWurde * Gesamtnote;
            //}
            ViewBag.Message = string.Format((GesamtnoteZusammen == 0) ? "noch keine Noten" : Math.Round(GesamtnoteZusammen, 2).ToString());
            return fachKompetenzbereiche;
        }

        private List<NoteView> GetNoteView(int userId, int kompetenzbereichId)
        {
            List<Fach> faecher = _dbContext.Fachs.Where(k => k.KompetenzbereichId == kompetenzbereichId).ToList();
            List<Note> notenFromDB = _dbContext.Notes.Where(k => k.UserId == userId).Where(k => k.FachbereichId == kompetenzbereichId).ToList();
            List<NoteView> notenViews = new List<NoteView>();

            int i = 0;
            if (faecher.Count() != 0)
            {
                foreach (Fach fach in faecher)
                {
                    NoteView noteView = new NoteView();
                    noteView.Id = i;
                    noteView.UserId = userId;
                    noteView.FachId = fach.Id;
                    noteView.Fachname = fach.Name;
                    // check if Note exist
                    Note currentNote = _dbContext.Notes.Where(n => n.UserId == userId && n.FachId == fach.Id).FirstOrDefault();
                    int currentNoteCount = _dbContext.Notes.Where(n => n.UserId == userId && n.FachId == fach.Id).Count();
                    int countNoteFromFach = notenFromDB.Where(n => n.Id == fach.Id).Count();
                    if (currentNoteCount != 0)
                    {
                        noteView.Noteid = currentNote.Id;
                        noteView.Notenwert = currentNote.Notenwert;
                        noteView.Semester = currentNote.Semester;
                        noteView.StudentAlreadyChanged = currentNote.StudentAlreadyChanged;
                    }
                    else
                    {
                        noteView.Noteid = -1;
                        noteView.Notenwert = 0;
                        noteView.Semester = -1;
                        noteView.StudentAlreadyChanged = 0;
                    }
                    notenViews.Add(noteView);
                }
            }
            return notenViews;
        }

        // if notenId == -1 -> es existiert noch keine Note
        public IActionResult NotenEdit(int notenId, int fachId, int kompetenzbereichId)
        {
            string role = HttpContext.Session.GetString("_UserRole");
            if (role == "schueler" || role == "berufsbildner" || role == "lehrer" || role == "admin")
            {
                return View(getSpecificNote(notenId, fachId, kompetenzbereichId));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        private List<Note> getSpecificNote(int Id, int FachId, int KompetenzbereichId)
        {
            string role = HttpContext.Session.GetString("_UserRole");
            // Wenn noch keine Note exisitert, ist die Id die im Frontend gesetzt ist, -1 also wird automatisch eine neue hier im 'else' erstellt
            int noteCount = _dbContext.Notes.Where(n => n.Id == Id).ToList().Count;
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            if (noteCount != 0)
            {
                HttpContext.Session.SetInt32("_NoteID", Id);
                HttpContext.Session.SetInt32("_FachID", FachId);
                HttpContext.Session.SetInt32("_KompetenzbereichId", KompetenzbereichId);
                return _dbContext.Notes.Where(n => n.Id == Id).ToList();
            }
            else
            {
                Note note = new Note();
                note.FachbereichId = KompetenzbereichId;
                note.FachId = FachId;
                if (role == "lehrer")
                {
                    note.UserId = (int)HttpContext.Session.GetInt32("_StudentID"); ;
                }
                else
                {

                    note.UserId = userId;
                }
                _dbContext.Notes.Add(note);
                _dbContext.SaveChanges();
                int returnID = note.Id;
                HttpContext.Session.SetInt32("_NoteID", returnID);
                HttpContext.Session.SetInt32("_FachID", FachId);
                HttpContext.Session.SetInt32("_KompetenzbereichId", KompetenzbereichId);
                return _dbContext.Notes.Where(n => n.Id == returnID).ToList();
            }
        }

        public async Task<IActionResult> NotenBearbeiten(double notenwert)
        {
            if (notenwert > 6 || notenwert < 1)
            {
                int notenId = (int)HttpContext.Session.GetInt32("_NoteID");
                int fachId = (int)HttpContext.Session.GetInt32("_FachID");
                int kompetenzbereichId = (int)HttpContext.Session.GetInt32("_KompetenzbereichId");
                ViewBag.Message = string.Format("Die Note muss zwischen 1 und 6 liegen.");
                return View("NotenEdit", getSpecificNote(notenId, fachId, kompetenzbereichId));
            }



            string role = HttpContext.Session.GetString("_UserRole");
            if (role == "schueler" || role == "berufsbildner" || role == "lehrer" || role == "admin")
            {

                if (role == "lehrer")
                {
                    int NoteId = (int)HttpContext.Session.GetInt32("_NoteID");
                    Note note = _dbContext.Notes.FirstOrDefault(n => n.Id == NoteId);
                    note.Notenwert = notenwert;
                    note.StudentAlreadyChanged = 1;
                    _dbContext.SaveChanges();
                }
                else
                {
                    int NoteId = (int)HttpContext.Session.GetInt32("_NoteID");
                    Note note = _dbContext.Notes.FirstOrDefault(n => n.Id == NoteId);
                    note.Notenwert = notenwert;
                    note.StudentAlreadyChanged = 1;
                    _dbContext.SaveChanges();
                }


                List<FachKompetenzbereich> fachKompetenzbereiche = listAllFaecherAndKompetenzbereiche();
                if (fachKompetenzbereiche == null)
                {
                    ViewBag.Message = string.Format("Du bist keiner Klasse zugeordnet, melde dich bei einer Klasse an");
                    return View("~/Views/Account/Message.cshtml");
                }
                return View("Index", fachKompetenzbereiche);
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult NotenEditBack()
        {
            string role = HttpContext.Session.GetString("_UserRole");
            if (role == "schueler" || role == "berufsbildner" || role == "lehrer" || role == "admin")
            {
                List<FachKompetenzbereich> fachKompetenzbereiche = listAllFaecherAndKompetenzbereiche();
                if (fachKompetenzbereiche == null)
                {
                    ViewBag.Message = string.Format("Du bist keiner Klasse zugeordnet, melde dich bei einer Klasse an");
                    return View("~/Views/Account/Message.cshtml");
                }
                return View("Index", fachKompetenzbereiche);
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult NotenToKlasseBack()
        {
            string role = HttpContext.Session.GetString("_UserRole");
            if (role == "schueler" || role == "berufsbildner" || role == "lehrer" || role == "admin")
            {
                return RedirectToAction("SchuelerListe", "Lehrer");
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult NotenToLernendeBack()
        {
            string role = HttpContext.Session.GetString("_UserRole");
            if (role == "schueler" || role == "berufsbildner" || role == "lehrer" || role == "admin")
            {
                return RedirectToAction("Lernende", "Berufsbildner");
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public double calcKompetenzbereichNote(int kompetenzbereichId)
        {
            int userId = -1;
            string userRole = HttpContext.Session.GetString("_UserRole");
            if (userRole == "berufsbildner" || userRole == "lehrer")
            {
                userId = (int)HttpContext.Session.GetInt32("_StudentID");
            }
            else
            {
                userId = (int)HttpContext.Session.GetInt32("_UserID");
            }

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
                    double gerundeteNote = runden(notenWert, rundung);
                    kompetenzbereichSchnitt = (double)(kompetenzbereichSchnitt + gerundeteNote * fach.Gewichtung / 100);
                }
                var asdf = "";
            }
            //kompetenzbereichSchnitt = (double)(kompetenzbereichSchnitt + kompetenzbereichSchnitt * gewichtungWoNochKeineNote / 100);
            double gewichtungWoNote = 100 - gewichtungWoNochKeineNote;
            kompetenzbereichSchnitt = (double)(100 / gewichtungWoNote * kompetenzbereichSchnitt);
            if (Double.IsNaN(kompetenzbereichSchnitt))
            {
                return 0;
            }
            else
            {
                return kompetenzbereichSchnitt;
            }
        }

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

        public static double runden(double note, double rundung)
        {
            double gerundeteNote = 0;
            if (rundung == 0.1)
            {
                gerundeteNote = Math.Round(note, 1);
            }
            else if (rundung == 0.5)
            {
                gerundeteNote = Math.Round(Math.Round(note * 2, 0) / 2, 1);
            }
            else if (rundung == 1)
            {
                gerundeteNote = Math.Round(note, 0);
            }

            return gerundeteNote;
        }
    }
}
