using SiteFilms.Data;

namespace SiteFilms.ViewsModel
{
    public class VideoView
    {
        public Video Video { get; set; } = new Video();
        public Country[] Countries { get; set; } = [];
        public Genre[] Genres { get; set; } = [];

        public VideoView() { }

        public VideoView(Video video, Country[] countries, Genre[] genres)
        {
            Video = video;
            Countries = countries;
            Genres = genres;
        }
    }
}
