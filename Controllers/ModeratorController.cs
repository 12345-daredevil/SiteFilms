using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SiteFilms.Data;
using SiteFilms.Models;
using SiteFilms.ViewsModel;

namespace SiteFilms.Controllers
{
    [Authorize(Roles = "moderator,admin")]
    public class ModeratorController(ApplicationDbContext db, UserManager<Person> user, IMemoryCache cache) : Controller
    {
        readonly ApplicationDbContext _db = db;
        readonly UserManager<Person> _userManager = user;
        readonly IMemoryCache _cache = cache;

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User) ?? new();

            var catalog = new CatalogView()
            {
                MyAction = ControllerContext.ActionDescriptor.ActionName,
                MyController = ControllerContext.ActionDescriptor.ControllerName,
                UserId = user.Id,
                Moderator = true,
            };

            catalog.Videos = await CatalogView.GetListVideo(_db, catalog);
            catalog.Country = await MyСacheModel.GetCacheCountry(_db, _cache);
            catalog.Genge = await MyСacheModel.GetCacheGenre(_db, _cache);

            return View("Index", catalog);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CatalogView view)
        {
            if (view.UserId == null)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User) ?? new Person();
                view.UserId = user.Id;
            }

            if (view.PageIndex >= view.PageCount || view.PageIndex < 0) view.PageIndex = 0;

            view.Videos = await CatalogView.GetListVideo(_db, view);
            view.Country = await MyСacheModel.GetCacheCountry(_db, _cache);
            view.Genge = await MyСacheModel.GetCacheGenre(_db, _cache);

            return View("Index", view);
        }

        #region Country
        public async Task<IActionResult> ShowCountry()
        {
            var show = await MyСacheModel.GetCacheCountry(_db, _cache);
            return View("ShowCountry", show.OrderBy(x => x.Name).ToList());
        }

        public async Task<IActionResult> DeleteCountry(uint Id)
        {
            await _db.Countrys.Where(x => x.Id == Id).ExecuteDeleteAsync();
            return View("_Errors", new Error("Success!", "Country was deleted.", "Moderator"));
        }

        public async Task<IActionResult> AddCountry(string name)
        {
            if (string.IsNullOrEmpty(name) || await _db.Countrys.AsNoTracking().AnyAsync(x => x.Name == name)) 
                return RedirectToAction("ShowCountry");

            await _db.Countrys.AddAsync(new Country(name));
            await _db.SaveChangesAsync();

            return View("_Errors", new Error("Success!", "Country was add.", "Moderator"));
        }
        #endregion

        #region Genre
        public async Task<IActionResult> ShowGenre()
        {
            var show = await MyСacheModel.GetCacheGenre(_db, _cache);
            return View("ShowGenre", show.OrderBy(x => x.Name).ToList());
        }

        public async Task<IActionResult> DeleteGenre(uint Id)
        {
            await _db.Genres.Where(x => x.Id == Id).ExecuteDeleteAsync();
            return View("_Errors", new Error("Success!", "Genre was deleted.", "Moderator"));
        }

        public async Task<IActionResult> AddGenre(string name)
        {
            if (string.IsNullOrEmpty(name) || await _db.Genres.AsNoTracking().AnyAsync(x => x.Name == name))
                return RedirectToAction("ShowGenre");

            await _db.Genres.AddAsync(new Genre(name));
            await _db.SaveChangesAsync();

            return View("_Errors", new Error("Success!", "Genre was add.", "Moderator"));
        }
        #endregion
    }
}
