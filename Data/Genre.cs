using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace SiteFilms.Data
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Genre() { }

        public Genre(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Genre(string name) => Name = name;


    }
}
