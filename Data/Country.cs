using System.ComponentModel.DataAnnotations;

namespace SiteFilms.Data
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Country() { }

        public Country(int id, string name) 
        {
            Id = id;
            Name = name;
        }

        public Country(string name) => Name = name;
    }
}
