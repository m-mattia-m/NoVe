using System;
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

            try
            {
                userRole = HttpContext.Session.GetString("_UserRole");
                Console.WriteLine("SessionRole: " + userRole);
                if (userRole == "schueler" || userRole == "berufsbildner" || userRole == "lehrer" || userRole == "admin")
                {
                    Console.WriteLine("UserID: " + (int)HttpContext.Session.GetInt32("_UserID"));
                    return View(listAllFaecherAndKompetenzbereiche());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fehler beim auslesen aus der Session");
            }

            ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
            return View("~/Views/Account/Message.cshtml");
            //return View("~/Views/Home/Index.cshtml");
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
            List<Kompetenzbereich> kompetenzbereiche = _dbContext.Kompetenzbereichs.Where(k => k.BerufId == user.Klasse.BerufId).ToList();
            List<FachKompetenzbereich> fachKompetenzbereiche = new List<FachKompetenzbereich>();
            Klasse klasse = _dbContext.Klasses.Where(k => k.Id == user.Klasse.Id).FirstOrDefault();

            DateTime localDate = DateTime.Now;
            int InEditTime = 0;

            if (klasse.Startdatum < localDate && klasse.EndDatum > localDate)
            {
                InEditTime = 1;
            }
            else
            {
                InEditTime = 0;
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
                fachKompetenzbereich.NotenwertKompetenzbereich = calcKompetenzbereichNote(kompetenzbereich.Id);

                fachKompetenzbereiche.Add(fachKompetenzbereich);
                i++;
            }

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
            return View(getSpecificNote(notenId, fachId, kompetenzbereichId));
        }

        private List<Note> getSpecificNote(int Id, int FachId, int KompetenzbereichId)
        {
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
                note.UserId = userId;
                _dbContext.Notes.Add(note);
                _dbContext.SaveChanges();
                int returnID = note.Id;
                HttpContext.Session.SetInt32("_NoteID", returnID);
                HttpContext.Session.SetInt32("_FachID", FachId);
                HttpContext.Session.SetInt32("_KompetenzbereichId", KompetenzbereichId);
                return _dbContext.Notes.Where(n => n.Id == returnID).ToList();
            }
        }

        public async Task<IActionResult> NotenBearbeiten(float notenwert)
        {
            int NoteId = (int)HttpContext.Session.GetInt32("_NoteID");
            Note note = _dbContext.Notes.FirstOrDefault(n => n.Id == NoteId);
            note.Notenwert = notenwert;
            note.StudentAlreadyChanged = 1;
            _dbContext.SaveChanges();

            return View("Index", listAllFaecherAndKompetenzbereiche());
        }

        public IActionResult NotenEditBack()
        {
            return View("Noten", listAllFaecherAndKompetenzbereiche());
        }

        public IActionResult NotenToKlasseBack()
        {
            //return View("Noten", listAllFaecherAndKompetenzbereiche());
            return RedirectToAction("SchuelerListe", "Lehrer");
        }

        public IActionResult NotenToLernendeBack()
        {
            //return View("SchuelerListe", SchuelerListe());
            return RedirectToAction("SchuelerListe", "Lehrer");
        }

        public float calcKompetenzbereichNote(int kompetenzbereichId)
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            List<Fach> faecher = _dbContext.Fachs.Where(f => f.KompetenzbereichId == kompetenzbereichId).ToList();
            float kompetenzbereichSchnitt = 0;

            foreach (Fach fach in faecher)
            {
                float notenWert = (float)getNoteFromFach(fach.Id, userId);
                float rundung = (float)fach.Rundung;
                float gerundeteNote = runden(notenWert, rundung);
                kompetenzbereichSchnitt = (float)(kompetenzbereichSchnitt + gerundeteNote * (40 / 100));
                var asdf = "";
            }

            return kompetenzbereichSchnitt;
        }

        public float getNoteFromFach(int fachId, int userId)
        {
            Note note = _dbContext.Notes.Where(n => n.FachId == fachId).Where(n => n.UserId == userId).FirstOrDefault();
            float notenwert = note.Notenwert;

            return notenwert;
        }

        public float runden(float note, float rundung)
        {
            float gerundeteNote = 0;
            if (rundung == 0.1F)
            {
                gerundeteNote = (float)Math.Round(note, 1);
            }
            else if (rundung == 0.5F)
            {
                gerundeteNote = (float)Math.Round(Math.Round(note * 2, 0) / 2, 1);
            }
            else if (rundung == 1F)
            {
                gerundeteNote = (float)Math.Round(note, 0);
            }

            return gerundeteNote;
        }
    }
}
