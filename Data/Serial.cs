namespace SiteFilms.Data
{
    public class Serial : Video
    {
        public ushort CountSeries { get; set; }
        public byte CountSeasons { get; set; }

        public Serial() { }

        //public Serial(string name, string description, Country country, ushort timeVideo, 
        //    byte ageRestriction, DateTime makeDate, ushort countSeries, Genre genre, byte countSeasons) :
        //    base(name, description, country, genre, timeVideo, ageRestriction, makeDate)
        //{
        //    CountSeries = countSeries;
        //    CountSeasons = countSeasons;
        //}




    }
}
