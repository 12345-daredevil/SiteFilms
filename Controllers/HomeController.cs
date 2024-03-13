using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SiteFilms.Data;
using SiteFilms.Models;
using SiteFilms.ViewsModel;
using System.Diagnostics;

namespace SiteFilms.Controllers
{
    public class HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<Person> user) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        readonly ApplicationDbContext _db = db;
        readonly UserManager<Person> _userManager = user;

        public IActionResult Index() => View();

        public async Task<IActionResult> Catalog()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User) ?? new();

            var catalog = new CatalogView()
            {
                MyAction = ControllerContext.ActionDescriptor.ActionName,
                MyController = ControllerContext.ActionDescriptor.ControllerName,
                UserId = user.Id,
            };

            var count = CatalogView.CountListVideo(_db, catalog);

            catalog.PageCount = count / catalog.CountOnPage + (count % catalog.CountOnPage == 0 ? 0 : 1);
            catalog.Videos = CatalogView.GetListVideo(_db, catalog);

            CatalogView.Country = await _db.Countrys.AsNoTracking().ToArrayAsync();
            CatalogView.Genge = await _db.Genres.AsNoTracking().ToArrayAsync();

            return View("Catalog", catalog);
        }

        [HttpPost]
        public async Task<IActionResult> Catalog(CatalogView? view)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User) ?? new Person();
            view ??= new CatalogView();
            view.UserId = user.Id;

            var count = CatalogView.CountListVideo(_db, view);

            view.PageCount = count / view.CountOnPage + (count % view.CountOnPage == 0 ? 0 : 1);
            if (view.PageIndex >= view.PageCount || view.PageIndex < 0) view.PageIndex = 0;

            view.Videos = CatalogView.GetListVideo(_db, view);
            view.UserId = user.Id;

            return View("Catalog", view);
        }

        [Authorize]
        public async Task<IActionResult> AddVideo()
        {
            var country = await _db.Countrys.AsNoTracking().ToArrayAsync();
            var genre = await _db.Genres.AsNoTracking().ToArrayAsync();
            return View("AddVideo", new VideoView(new Video(), country, genre));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddVideo(VideoView view)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null) return View("AddVideo", view);

                var ob = new Video(view.Video.Name, view.Video.Description, view.Video.CountryId,
                    view.Video.GenreId, view.Video.TimeVideo, view.Video.AgeRestriction, view.Video.MakeDate, user.Id);

                await _db.Videos.AddAsync(ob);
                await _db.SaveChangesAsync();
                return View("_Errors", new Error("Success!", "Video save."));
            }
            return View("AddVideo", view);
        }

        public async Task<IActionResult> EditVideo(uint Id)
        {
            var video = await _db.Videos
                .AsNoTracking()
                .Include(x => x.Country)
                .Include(x => x.Genre)
                .FirstAsync(x => x.Id == Id);

            var country = await _db.Countrys.AsNoTracking().ToArrayAsync();
            var genre = await _db.Genres.AsNoTracking().ToArrayAsync();
            return View("EditVideo", new VideoView(video, country, genre));
        }

        [HttpPost]
        public async Task<IActionResult> EditVideo(VideoView newVideo)
        {
            var oldVideo = await _db.Videos.FirstAsync(x => x.Id == newVideo.Video.Id);
            oldVideo.EditVideo(newVideo.Video);
            _db.Videos.Update(oldVideo);
            await _db.SaveChangesAsync();

            return View("_Errors", new Error("Success!", "Video save."));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVideo(VideoView newVideo)
        {
            var delete = await _db.Videos.FirstAsync(x => x.Id == newVideo.Video.Id);
            _db.Videos.Remove(delete);
            await _db.SaveChangesAsync();
            return View("_Errors", new Error("Success!", "Video was deleted."));
        }

        public async Task<IActionResult> ShowVideo(uint Id) 
            => View("ShowVideo", await _db.Videos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id) ?? new Video());


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
