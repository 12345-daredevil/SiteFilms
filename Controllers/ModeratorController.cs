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
    public class ModeratorController : Controller
    {
        readonly ApplicationDbContext _db;

        public ModeratorController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            byte countOnPage = 5;
            var count = await _db.Videos.AsNoTracking().CountAsync(); 
            var videos = await _db.Videos
                .AsNoTracking()
                .Include(x => x.Country)
                .Include(x => x.Genre)
                .Take(countOnPage)
                .ToListAsync();

            var action = ControllerContext.ActionDescriptor.ActionName;
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var pageCount = count % countOnPage == 0 ? count / countOnPage : count / countOnPage + 1;

            var catalog = new CatalogView(videos, 0, pageCount, countOnPage, controller, action);

            return View("Index", catalog);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CatalogView view)
        {
            view ??= new CatalogView();
            var videos = await _db.Videos
                .AsNoTracking()
                .Where(x => !x.FlagCheck)
                .Include(x => x.Country)
                .Include(x => x.Genre)
                .Skip(view.PageIndex * view.CountOnPage)
                .Take(view.CountOnPage)
                .ToListAsync();

            var count = CatalogView.CountListVideo(_db, view);

            view.PageCount = count / view.CountOnPage + (count % view.CountOnPage == 0 ? 0 : 1);
            if (view.PageIndex >= view.PageCount || view.PageIndex < 0) view.PageIndex = 0;

            view.Videos = CatalogView.GetListVideo(_db, view);

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
