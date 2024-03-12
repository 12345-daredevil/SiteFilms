namespace SiteFilms.ViewsModel
{
    public class Error
    {
        public string Title { get; set; } = "Error!";
        public string Message { get; set; } = "Error...";
        public string? NameController { get; set; }

        public Error() { }

        public Error(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public Error(string title, string message, string nameController)
        {
            Title = title;
            Message = message;
            NameController = nameController;
        }
    }
}
