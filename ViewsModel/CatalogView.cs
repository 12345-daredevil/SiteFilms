﻿using Microsoft.EntityFrameworkCore;
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

        public static int CountListVideo(ApplicationDbContext _db, CatalogView view)
        {
            var abc = Filter(view.SelectGenre, view.SelectCountry, view.UserId);
            return _db.Videos.AsNoTracking().Where(abc).Count();
        }

        public static List<Video> GetListVideo(ApplicationDbContext _db, CatalogView view)
        {
            List<Video> videos = [];

            var abc = Filter(view.SelectGenre, view.SelectCountry, view.UserId);

            return _db.Videos
                .AsNoTracking()
                .Where(abc)
                .AsQueryable()
                .Include(x => x.Country)
                .Include(x => x.Genre)
                .Skip(view.PageIndex * view.CountOnPage)
                .Take(view.CountOnPage)
                .ToList();
        }

        static Func<Video, bool> Filter(int selectGenre, int selectCountry, string? userId)
        {
            Func<Video, bool> abc = x => x.TimeVideo >= 0;

            var moder = true;
            if (!moder)
            {
                abc = x => x.FlagCheck;
                if (userId != null)
                    abc += x => x.AspNetUsersId == userId;
            }

            if (selectGenre != 0)
                abc += x => x.GenreId == selectGenre;

            if (selectCountry != 0)
                abc += x => x.CountryId == selectCountry;

            return abc;
        }
    }
}
