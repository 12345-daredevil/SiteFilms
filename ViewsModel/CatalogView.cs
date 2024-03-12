using SiteFilms.Data;
using System.Collections.Generic;

namespace SiteFilms.ViewsModel
{
    public class CatalogView
    {
        public int[] AllCountOnPage { get; set; } = new int[] { 5, 10, 20 };
        public List<Video>? Videos { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public int CountOnPage { get; set; } = 10;
        public string? UserId { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }

        public CatalogView() { }

        public CatalogView(List<Video> list, int index, int count, int countOnPage, string controller, string action)
        {
            Videos = list;
            PageIndex = index;
            PageCount = count;
            CountOnPage = countOnPage;
            Action = action;
            Controller = controller;
        }

        public CatalogView(List<Video> list, int index, int count, int countOnPage, string controller, string action, string userId) :
            this(list, index, count, countOnPage, controller, action)
        {
            UserId = userId;
        }
    }
}
