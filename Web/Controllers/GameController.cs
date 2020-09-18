using Data;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Game;

namespace Web.Controllers
{
    public class GameController : Controller
    {
        private readonly MyGameDb _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        List<int> numSelection = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        string guessKey;
        

        public GameController(IHttpContextAccessor httpContextAccessor)
        {
            _context = new MyGameDb();
            _httpContextAccessor = httpContextAccessor;

            guessKey = GetGuessKey();
        }

        //GET: /Index
        public IActionResult Index()
        {
            if (SessionExtensions.Get<int>(_session, "LoggedIn") != 1)
            {
                return Redirect("/Home");
            }
            else
            {
                GameViewModel model = new GameViewModel();
                model.Guess = "1234";

                return View(model);
            }
        }

        //POST: /Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(GameViewModel model)
        {
            if (SessionExtensions.Get<int>(_session, "LoggedIn") != 1)
            {
                return Redirect("/Home");
            }
            else
            {
                if (model.Guess.ToCharArray().Count() < 4)
                {
                    ModelState.AddModelError("Guess", "Your guess should be 4 letters long!");

                    return View(model);
                }
                else if(model.Guess.ToCharArray().Distinct().Count() < 4)
                {
                    ModelState.AddModelError("Guess", "Your guess should have different numbers in it!");

                    return View(model);
                }
                else
                {
                    if (model.Guess == guessKey)
                    {
                        string currentUsername = CookieExtensions.GetCookie(Request, "Username");
                        User currentUser = _context.Users.FirstOrDefault(x => x.Username == currentUsername);

                        currentUser.GamesWon++;

                        try
                        {
                            _context.Update(currentUser);
                            _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return NotFound();
                        }

                        ModelState.AddModelError("Guess", "You win!");

                        return View(model);
                    }
                    else
                    {
                        int currentBulls = 0;
                        int currentCows = 0;

                        for (int charIndex = 0; charIndex < 4; charIndex++)
                        {
                            char checkable = model.Guess[charIndex];
                            if (guessKey.Contains(checkable))
                            {
                                if (guessKey[charIndex] == checkable)
                                {
                                    currentBulls++;
                                }
                                else
                                {
                                    currentCows++;
                                }
                            }
                        }

                        ModelState.AddModelError("Guess", $"{currentCows} COWS and {currentBulls} BULLS - Try Again!");

                        return View(model);
                    }
                }
            }
        }

        // GET: Game/Restart
        public IActionResult Restart()
        {
            if (SessionExtensions.Get<int>(_session, "LoggedIn") != 1)
            {
                return Redirect("/Home");
            }
            else
            {
                SessionExtensions.Set<string>(_session, "GuessKey", "");

                return Redirect("/Game");
            }
        }

        private string GetGuessKey()
        {
            Random randomGen = new Random();
            string guessKey = SessionExtensions.Get<string>(_session, "GuessKey");

            if (string.IsNullOrEmpty(guessKey))
            {
                for (int numSelectionRep = 0; numSelectionRep < 4; numSelectionRep++)
                {
                    int selectionIndex = randomGen.Next(0, numSelection.Count());

                    guessKey += numSelection[selectionIndex].ToString();
                    numSelection.RemoveAt(selectionIndex);
                }

                SessionExtensions.Set<string>(_session, "GuessKey", guessKey);
            }

            return guessKey;
        }
    }
}