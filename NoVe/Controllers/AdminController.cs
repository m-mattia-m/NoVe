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
                if (userRole == "Admin")
                {
                    return View(getUnconfirmedUsers());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fehler beim auslesen aus der Session");
            }


            return View("Home/View");
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

    private List<User> getAllUsers()
    {

      return _dbContext.Users.Where(u => u.AdminVerification == 1).ToList();

    }
  }
}
