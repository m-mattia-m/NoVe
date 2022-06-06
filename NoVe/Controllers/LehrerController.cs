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

        private List<User> getUsersFromClass()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");

            // Create Class - only for testing
            //Klasse klasse1 = _dbContext.Klasses.Where(k => k.Id == 1).FirstOrDefault();
            //User user1 = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            //user1.Klasse = klasse1;
            //_dbContext.SaveChanges();

            //Set Student into Class - only for testing
            Klasse klasse2 = _dbContext.Klasses.Where(k => k.Id == 1).FirstOrDefault();
            User user2 = _dbContext.Users.Where(u => u.Id == 5).FirstOrDefault();
            user2.Klasse = klasse2;
            _dbContext.SaveChanges();

            List<User> currentUser = _dbContext.Users.Include(x => x.Klasse).Where(u => u.Id == userId).ToList();
            Klasse klasse = _dbContext.Klasses.Include(x => x.Users).Where(k => k.Id == currentUser[0].Klasse.Id).FirstOrDefault();

            List<User> users = new List<User>(klasse.Users.Count);

            foreach (User user in klasse.Users)
            {
                if (user.Role == "schueler")
                {
                    users.Add(user);
                }
            }

            return users;
        }

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


    }
}
