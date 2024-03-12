using System.ComponentModel.DataAnnotations;

namespace  SiteFilms.Data
{
    public class Message
    {
        [Key]
        public short Id { get; set; } = 1;
        public string? URL { get; set; } = "localhost";
        public int Port { get; set; } = 25;

        [Required(ErrorMessage = "Email")]
        [EmailAddress(ErrorMessage = "EmailError")]
        [Display(Name = "Email")]
        public string? Email { get; set; } = string.Empty;
        public string? Login { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? Title { get; set; } = "Administration";
        public string? NameSite { get; set; } = string.Empty;
        public bool Flag { get; set; } = false;

        public Message() { }
    }
}
