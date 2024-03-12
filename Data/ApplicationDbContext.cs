using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SiteFilms.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<Person, IdentityRole, string>(options)
    {
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<Country> Countrys { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Video> Videos { get; set; } = null!;
        public DbSet<Serial> Serials { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Person>().HasData(
                    new Person
                    {
                        Email = "example@gmail.com",
                        EmailConfirmed = true,
                        UserName = "example@gmail.com",
                        NormalizedUserName = "example@gmail.com",
                        NormalizedEmail = "example@gmail.com",
                        LockoutEnd = null,
                        PasswordHash = "AQAAAAIAAYagAAAAEBORKlBrnhukkilXT3gdo23ZiemfM8z8g+UvnM/LaZOkV19FsiUniEmEujgUkuKpDg==",
                        SecurityStamp = "NOASWVD2WL6WE4FYH4PS6UWX62FIWGDH",
                        ConcurrencyStamp = "56a50561-e866-4b71-b9d9-df41065f258d",
                        TwoFactorEnabled = false,
                        AccessFailedCount = 0,
                        LockoutEnabled = true,
                        PhoneNumber = null,
                        PhoneNumberConfirmed = false,
                        Id = Guid.NewGuid().ToString(),
                        DateBorn = DateTime.UtcNow.AddYears(-30),
                    });

            builder.Entity<Message>().HasData(new Message());
        }
    }
}
