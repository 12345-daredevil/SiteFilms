using SiteFilms.Data;

namespace SiteFilms.ViewsModel
{
    public class CatalogView
    {
        public List<Video>? Videos { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public int CountOnPage { get; set; } = 10;
        public string? UserId { get; set; }

        public CatalogView() { }

        public CatalogView(List<Video> list, int index, int count, int countOnPage)
        {
            Videos = list;
            PageIndex = index;
            PageCount = count;
            CountOnPage = countOnPage;
        }

        public CatalogView(List<Video> list, int index, int count, int countOnPage, string userId) 
        {
            Videos = list;
            PageIndex = index;
            PageCount = count;
            CountOnPage = countOnPage;
            UserId = userId;
        }
    }
}
