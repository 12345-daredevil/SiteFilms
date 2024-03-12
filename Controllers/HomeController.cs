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
            var user = await _userManager.GetUserAsync(HttpContext.User);
            user ??= new();
            byte countOnPage = 5;
            var count = await _db.Videos
                .AsNoTracking()
                .Where(x => x.FlagCheck || x.AspNetUsersId == user.Id)
                .CountAsync();

            var videos = await _db.Videos
                .AsNoTracking()
                .Where(x => x.FlagCheck || x.AspNetUsersId == user.Id)
                .Include(x => x.Country)
                .Include(x => x.Genre)
                .Take(countOnPage)
                .ToListAsync();

            var action = ControllerContext.ActionDescriptor.ActionName;
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var pageCount = count / countOnPage + (count % countOnPage == 0 ? 0 : 1);

            var catalog = new CatalogView(videos, 0, pageCount, countOnPage, controller, action, user.Id);

            return View("Catalog", catalog);
        }

        [HttpPost]
        public async Task<IActionResult> Catalog(CatalogView? view)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User) ?? new Person();

            view ??= new CatalogView();

            var count = await _db.Videos
                .AsNoTracking()
                .Where(x => x.FlagCheck || x.AspNetUsersId == user.Id)
                .CountAsync();

            view.PageCount = count / view.CountOnPage + (count % view.CountOnPage == 0 ? 0 : 1);

            if (view.PageIndex >= view.PageCount || view.PageIndex < 0) view.PageIndex = 0;

            var videos = await _db.Videos
                .AsNoTracking()
                .Where(x => x.FlagCheck || x.AspNetUsersId == user.Id)
                .Include(x => x.Country)
                .Include(x => x.Genre)
                .Skip(view.PageIndex * view.CountOnPage)
                .Take(view.CountOnPage)
                .ToListAsync();

            view.Videos = videos;
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
