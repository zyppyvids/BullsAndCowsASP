using Data;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Web.Models.Users;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyGameDb _context;

        public UsersController()
        {
            _context = new MyGameDb();
        }

        // GET: Users
        public async Task<IActionResult> Index(UsersIndexViewModel model)
        {
            List<UsersViewModel> items = await _context.Users.Select(u => new UsersViewModel()
            {
                Id = u.Id,
                Username = u.Username,
                GamesWon = u.GamesWon
            }).ToListAsync();

            //Gets only 25 of the best players
            model.Items = items.OrderBy(i => i.GamesWon).Take(25).ToList();

            return View(model);
        }
    }
}
