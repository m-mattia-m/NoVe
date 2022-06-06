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
            Console.WriteLine("UserID: " + (int)HttpContext.Session.GetInt32("_UserID"));
            return View(listAllFaecherAndKompetenzbereiche());
        }

        private List<FachKompetenzbereich> listAllFaecherAndKompetenzbereiche()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            User user = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).FirstOrDefault();
            List<Kompetenzbereich> kompetenzbereiche = _dbContext.Kompetenzbereichs.Where(k => k.BerufId == user.Klasse.BerufId).ToList();
            List<FachKompetenzbereich> fachKompetenzbereiche = new List<FachKompetenzbereich>();


            int i = 0;
            foreach (Kompetenzbereich kompetenzbereich in kompetenzbereiche) {
                FachKompetenzbereich fachKompetenzbereich = new FachKompetenzbereich();
                fachKompetenzbereich.Kompetenzbereich = kompetenzbereich;
                fachKompetenzbereich.NoteView = GetNoteView(userId, kompetenzbereich.Id);
                fachKompetenzbereich.Id = i;
                fachKompetenzbereich.UserId = userId;


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
            if (faecher.Count() != 0) {
                foreach (Fach fach in faecher)
                {
                    var notenwertindex = notenFromDB.FindIndex(f => f.FachId == fach.Id);
                    NoteView noteView = new NoteView();
                    noteView.Id = i;
                    noteView.UserId = userId;
                    noteView.FachId = fach.Id;
                    noteView.Fachname = fach.Name;
                    noteView.Notenwert = (notenFromDB.Count != 0) ? (notenFromDB.FindIndex(f => f.FachId == fach.Id) == 1) ? notenFromDB[notenFromDB.FindIndex(f => f.FachId == fach.Id)].Notenwert : 0 : 0;
                    noteView.Noteid = (notenFromDB.Count != 0) ? (notenFromDB.FindIndex(f => f.FachId == fach.Id) == 1) ? notenFromDB[notenFromDB.FindIndex(f => f.FachId == fach.Id)].Id : 0 : -1;
                    noteView.Semester = (notenFromDB.Count != 0) ? (notenFromDB.FindIndex(f => f.FachId == fach.Id) == 1) ? notenFromDB[notenFromDB.FindIndex(f => f.FachId == fach.Id)].Semester : 0 : -1;
                    noteView.StudentAlreadyChanged = (notenFromDB.Count != 0) ? (notenFromDB.FindIndex(f => f.FachId == fach.Id) == 1) ? notenFromDB[notenFromDB.FindIndex(f => f.FachId == fach.Id)].StudentAlreadyChanged : 0 : 0;

                    notenViews.Add(noteView);
                    i++;
                }
            }
            
            return notenViews;
        }

        // if notenId == -1 -> es existiert noch keine Note
        public IActionResult NotenEdit(int notenId, int fachId, int kompetenzbereichId)
        {
            HttpContext.Session.SetInt32("_NoteID", notenId);
            HttpContext.Session.SetInt32("_FachID", fachId);
            HttpContext.Session.SetInt32("_KompetenzbereichId", kompetenzbereichId);
            return View(getSpecificNote(notenId));
        }

        private List<Note> getSpecificNote(int Id)
        {

            int noteCount = _dbContext.Notes.Where(n => n.Id == Id).ToList().Count;
            if (noteCount != 0)
            {
                return _dbContext.Notes.Where(n => n.Id == Id).ToList();
            }

            Note note = new Note();
            _dbContext.Notes.Add(note);
            _dbContext.SaveChanges();
            int returnID = note.Id;
            HttpContext.Session.SetInt32("_NoteID", Id);
            return _dbContext.Notes.Where(n => n.Id == returnID).ToList();
        }

        public async Task<IActionResult> NotenBearbeiten(float notenwert)
        {
            int NoteId = (int)HttpContext.Session.GetInt32("_NoteID");
            int FachId = (int)HttpContext.Session.GetInt32("_FachID");
            int KompetenzbereichId = (int)HttpContext.Session.GetInt32("_KompetenzbereichId");
            int userId = (int)HttpContext.Session.GetInt32("_UserID");

            // if NotenId -1 -> es existiert noch keine Note
            if (NoteId == -1)
            {
                Note note = new Note();
                note.Notenwert = notenwert;
                note.FachId = FachId;
                note.FachbereichId = KompetenzbereichId;
                note.UserId = userId;
                if (HttpContext.Session.GetString("_UserRole") == "schueler")
                {
                    note.StudentAlreadyChanged = 1;
                }
                else
                {
                    note.StudentAlreadyChanged = 0;
                }
                _dbContext.Notes.Add(note);
                _dbContext.SaveChanges();
            }
            else {
                Note note = _dbContext.Notes.FirstOrDefault(n => n.Id == NoteId);
                note.Notenwert = notenwert;
                note.FachId = FachId;
                note.FachbereichId = KompetenzbereichId;
                note.UserId = userId;
                if ("schueler" == HttpContext.Session.GetString("_UserRole"))
                {
                    note.StudentAlreadyChanged = 1;
                }
                else
                {
                    note.StudentAlreadyChanged = 0;
                }
                _dbContext.Notes.Add(note);
                _dbContext.SaveChanges();
            }


            return View("Index", listAllFaecherAndKompetenzbereiche());
        }
    }
}
