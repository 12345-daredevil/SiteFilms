using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using SiteFilms.Data;
using SiteFilms.Models;
using SiteFilms.ViewsModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SiteFilms.Controllers
{
    public class HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<Person> user, IMemoryCache cache) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        readonly ApplicationDbContext _db = db;
        readonly UserManager<Person> _userManager = user;
        readonly IMemoryCache _cache = cache;

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

            catalog.Videos = await CatalogView.GetListVideo(_db, catalog);
            catalog.Country = await MyÑacheModel.GetCacheCountry(_db, _cache);
            catalog.Genge = await MyÑacheModel.GetCacheGenre(_db, _cache);

            return View("Catalog", catalog);
        }

        [HttpPost]
        public async Task<IActionResult> Catalog(CatalogView view)
        {
            if(view.UserId == null)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User) ?? new Person();
                view.UserId = user.Id;
            }

            if (view.PageIndex >= view.PageCount || view.PageIndex < 0) view.PageIndex = 0;

            view.Videos = await CatalogView.GetListVideo(_db, view);
            view.Country = await MyÑacheModel.GetCacheCountry(_db, _cache);
            view.Genge = await MyÑacheModel.GetCacheGenre(_db, _cache);

            return View("Catalog", view);
        }

        [Authorize]
        public async Task<IActionResult> AddVideo()
        {
            var country = await MyÑacheModel.GetCacheCountry(_db, _cache);
            var genre = await MyÑacheModel.GetCacheGenre(_db, _cache);
            return View("AddVideo", new VideoView(new Video(), country.ToArray(), genre.ToArray()));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddVideo(VideoView view, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                view.Countries = await MyÑacheModel.GetCacheCountry(_db, _cache);
                view.Genres = await MyÑacheModel.GetCacheGenre(_db, _cache);

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null) return View("AddVideo", view);

                var ob = new Video(view.Video.Name, view.Video.Description, view.Video.CountryId,
                    view.Video.GenreId, view.Video.TimeVideo, view.Video.AgeRestriction, view.Video.MakeDate, user.Id);

                if (file != null)
                {
                    if (file.ContentType != "image/jpeg" && file.ContentType != "image/png" && file.ContentType != "image/webp")
                        return View("_Errors", new Error("Error!", "Please choose a JPEG, PNG or WEBP image."));


                    if (file.Length > (512 * 1024))
                        return View("_Errors", new Error("Error!", "File size cannot exceed 512 Kb."));

                    byte[] p1;
                    using (var fs1 = file.OpenReadStream())
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                    view.Video.Skin = p1;
                }

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

            var country = await MyÑacheModel.GetCacheCountry(_db, _cache);
            var genre = await MyÑacheModel.GetCacheGenre(_db, _cache);
            return View("EditVideo", new VideoView(video, country, genre));
        }

        [HttpPost]
        public async Task<IActionResult> EditVideo(VideoView newVideo, IFormFile file)
        {
            var oldVideo = await _db.Videos.FirstAsync(x => x.Id == newVideo.Video.Id);
            newVideo.Countries = await MyÑacheModel.GetCacheCountry(_db, _cache);
            newVideo.Genres = await MyÑacheModel.GetCacheGenre(_db, _cache);

            if (file != null)
            {
                if (file.ContentType != "image/jpeg" && file.ContentType != "image/png" && file.ContentType != "image/webp")
                    return View("_Errors", new Error("Error!", "Please choose a JPEG, PNG or WEBP image."));


                if (file.Length > (512 * 1024))
                    return View("_Errors", new Error("Error!", "File size cannot exceed 512 Kb."));

                byte[] p1;
                using (var fs1 = file.OpenReadStream())
                using (var ms1 = new MemoryStream())
                {
                    fs1.CopyTo(ms1);
                    p1 = ms1.ToArray();
                }
                oldVideo.Skin = p1;
            }

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
            => View("ShowVideo", await _db.Videos
                .AsNoTracking()
                .Include(x => x.Country)
                .Include(x => x.Genre)
                .FirstOrDefaultAsync(x => x.Id == Id) ?? new Video());


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
