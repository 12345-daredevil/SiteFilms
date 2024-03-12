using Microsoft.AspNetCore.Identity;

namespace SiteFilms.Data
{
    public class Person : IdentityUser
    {
        public DateTime DateBorn { get; set; } 

        public Person() { }
        public Person(DateTime date) => DateBorn = date.Date;

    }
}
