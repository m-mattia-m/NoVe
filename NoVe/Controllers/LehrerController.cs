using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

using NoVe.Models;

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



        private List<User> getUsersFromClass()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");

            // Set Class - only for testing
            //Klasse klasse1 = _dbContext.Klasses.Where(k => k.Id == 2).FirstOrDefault();
            //User user1 = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            //user1.Klasse = klasse1;
            //_dbContext.SaveChanges();

            // Set Student Class - only for testing
            //Klasse klasse1 = _dbContext.Klasses.Where(k => k.Id == 2).FirstOrDefault();
            //User user1 = _dbContext.Users.Where(u => u.Id == 11).FirstOrDefault();
            //user1.Klasse = klasse1;
            //_dbContext.SaveChanges();

            User currentUser = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            Klasse klasse = _dbContext.Klasses.Where(k => k.Id == currentUser.Klasse.Id).FirstOrDefault();

            List<User> users = new List<User>(klasse.Users.Count);

            foreach (User user in klasse.Users) {
                if (user.Role == "schueler") {
                    users.Add(user);
                }
            }
            return users;
        }

        private List<Klasse> getSpecificClass()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            User currentUser = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            Klasse klasse = _dbContext.Klasses.Where(k => k.Id == currentUser.Klasse.Id).FirstOrDefault();

            List<Klasse> klasses = new List<Klasse>();
            klasses.Add(klasse);

            return klasses;
        }

        public async Task<IActionResult> ZeitSpeichern(DateTime startdatum, DateTime enddatum)
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");

            User currentUser = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            Klasse klasse = _dbContext.Klasses.Where(k => k.Id == currentUser.Klasse.Id).FirstOrDefault();

            klasse.Startdatum = startdatum;
            klasse.EndDatum = enddatum;
            _dbContext.SaveChanges();

            return View("SchuelerListe", getUsersFromClass());
        }


    }
}
