using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SiteFilms.Data;
using SiteFilms.ViewsModel;

namespace SiteFilms.Controllers
{
    [Authorize(Roles = "moderator,admin")]
    public class ModeratorController(ApplicationDbContext db, UserManager<Person> user) : Controller
    {
        readonly ApplicationDbContext _db = db;
        readonly UserManager<Person> _userManager = user;

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

            var count = await CatalogView.CountListVideo(_db, catalog);

            catalog.PageCount = count / catalog.CountOnPage + (count % catalog.CountOnPage == 0 ? 0 : 1);
            catalog.Videos = await CatalogView.GetListVideo(_db, catalog);

            if (CatalogView.Country.Length == 0)
                CatalogView.Country = await _db.Countrys.AsNoTracking().ToArrayAsync();

            if (CatalogView.Genge.Length == 0)
                CatalogView.Genge = await _db.Genres.AsNoTracking().ToArrayAsync();

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

            var count = await CatalogView.CountListVideo(_db, view);

            view.PageCount = count / view.CountOnPage + (count % view.CountOnPage == 0 ? 0 : 1);
            if (view.PageIndex >= view.PageCount || view.PageIndex < 0) view.PageIndex = 0;

            view.Videos = await CatalogView.GetListVideo(_db, view);

            if (CatalogView.Country.Length == 0)
                CatalogView.Country = await _db.Countrys.AsNoTracking().ToArrayAsync();

            if (CatalogView.Genge.Length == 0)
                CatalogView.Genge = await _db.Genres.AsNoTracking().ToArrayAsync();

            return View("Index", view);
        }

        #region Country
        public async Task<IActionResult> ShowCountry()
            => View("ShowCountry", await _db.Countrys.AsNoTracking().ToListAsync());

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
            => View("ShowGenre", await _db.Genres.AsNoTracking().ToListAsync());

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
