using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SiteFilms.Data;
using System.Collections.Generic;

namespace SiteFilms.ViewsModel
{
    public class CatalogView
    {
        public int[] AllCountOnPage { get; set; } = new int[] { 5, 10, 20 };
        static public Country[] Country { get; set; } = [];
        static public Genre[] Genge { get; set; } = [];
        public int SelectCountry { get; set; } = 0;
        public int SelectGenre { get; set; } = 0;
        public List<Video>? Videos { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageCount { get; set; } = 0;
        public int CountOnPage { get; set; } = 5;
        public string? UserId { get; set; }
        public string? MyController { get; set; }
        public string? MyAction { get; set; }
        public bool Moderator { get; set; } = false;

        public CatalogView() { }

        public CatalogView(List<Video> list, int index, int count, int countOnPage, string controller, string action)
        {
            Videos = list;
            PageIndex = index;
            PageCount = count;
            CountOnPage = countOnPage;
            MyAction = action;
            MyController = controller;
        }

        public CatalogView(List<Video> list, int index, int count, int countOnPage, string controller, string action, string userId) :
            this(list, index, count, countOnPage, controller, action)
        {
            UserId = userId;
        }

        public static async Task<int> CountListVideo(ApplicationDbContext _db, CatalogView view)
        {
            var abc = Filter(view.SelectGenre, view.SelectCountry, view.UserId, view.Moderator);

            var list = await _db.Videos
                .AsNoTracking()
                .ToListAsync();

            if (abc != null)
                foreach (var val in abc)
                    list = list.Where(val).ToList();

            return list.Count;
        }

        public static async Task<List<Video>> GetListVideo(ApplicationDbContext _db, CatalogView view)
        {
            var abc = Filter(view.SelectGenre, view.SelectCountry, view.UserId, view.Moderator);

            var list = await _db.Videos
                .AsNoTracking()
                .Include(x => x.Country)
                .Include(x => x.Genre)
                .ToListAsync();

            if (abc != null)
                foreach (var val in abc)
                    list = list.Where(val).ToList();

            return list
                .Skip(view.PageIndex * view.CountOnPage)
                .Take(view.CountOnPage)
                .ToList();
        }

        static List<Func<Video, bool>> Filter(int selectGenre, int selectCountry, string? userId, bool moderator)
        {
            List<Func<Video, bool>> abc = [];
                
                abc.Add(x => moderator ? !x.FlagCheck : x.FlagCheck || x.AspNetUsersId == userId);

            if (selectGenre != 0 && selectCountry == 0)
                abc.Add(x => x.GenreId == selectGenre);

            if (selectGenre == 0 && selectCountry != 0)
                abc.Add(x => x.CountryId == selectCountry);

            if (selectGenre != 0 && selectCountry != 0)
                abc.Add(x => x.CountryId == selectCountry && x.GenreId == selectGenre);

            return abc;
        }
    }
}
