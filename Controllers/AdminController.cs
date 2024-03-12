using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteFilms.Data;
using SiteFilms.Services;
using SiteFilms.ViewsModel;
using System.Net.Mail;

namespace SiteFilms.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController(ApplicationDbContext db, UserManager<Person> userManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        readonly UserManager<Person> _userManager = userManager;
        readonly RoleManager<IdentityRole> _roleManager = roleManager;
        readonly ApplicationDbContext _db = db;

        public IActionResult Index() => View("Index", new string[] { "admin", "moderator" });

        #region Message
        public async Task<IActionResult> ShowMessage() => View("ShowMessage", await _db.Messages.AsNoTracking().FirstAsync());

        [HttpPost]
        public async Task<IActionResult> MessageSetting(Message message)
        {
            try
            {
                var old = await _db.Messages.FirstAsync();
                old.URL = message.URL;
                old.Title = message.Title;
                old.Email = message.Email;
                old.Password = message.Password;
                old.Port = message.Port;
                old.Login = message.Login;
                old.NameSite = message.NameSite;
                _db.Messages.Update(old);
                await _db.SaveChangesAsync();

                return View("_Errors", new Error("Success!", "Main setting message was changed."));
            }
            catch { return View("_Errors", new Error("Error!", "Error.")); }
        }

        [HttpPost]
        public async Task<IActionResult> MessageSend(Message send)
        {
            if (!MailAddress.TryCreate(send.Email, out var _)) return View("_Errors", new Error("Error!", "Error."));
            var old = await _db.Messages.FirstAsync();
            try
            {
                await EmailSender.SendEmailAsync(old, send.Email, "Success", "Text my message for example.");
            }
            catch
            {
                return View("_Errors", new Error("Error!", "Error."));
            }
            return View("_Errors", new Error("Success!", "Message was send."));
        }

        [HttpPost]
        public async Task<IActionResult> FlagSendMessage(bool flag)
        {
            try
            {
                await _db.Messages.ExecuteUpdateAsync(x => x.SetProperty(p => p.Flag, flag));
                return View("_Errors", new Error("Success!", "Auto message was changed."));
            }
            catch { return View("_Errors", new Error("Error!", "Error.")); }
        }
        #endregion

        #region Role
        public async Task<IActionResult> ShowRole(string role)
        {
            List<string?> users = [];
            var findRoles = await _db.Roles.FirstOrDefaultAsync(x => x.Name == role);
            if (findRoles != null)
            {
                var roles = await _db.UserRoles.Where(x => x.RoleId == findRoles.Id).ToListAsync();
                var list = roles.Select(x => x.UserId);
                users = await _db.Users.Where(x => list.Contains(x.Id)).Select(x => x.Email).ToListAsync();
            }

            ViewBag.Roles = role;

            return View("ShowRole", users);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string email, string role)
        {
            if (!MailAddress.TryCreate(email, out var _)) return View("_Errors", new Error("Error!", "Email is invalid."));

            if (await _roleManager.FindByNameAsync(role) == null)
                await _roleManager.CreateAsync(new IdentityRole(role));

            var myUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (myUser == null) return View("_Errors", new Error("Error!", "Not found."));
            await _userManager.AddToRoleAsync(myUser, role);
            await _db.SaveChangesAsync();

            return View("_Errors", new Error("Success!", "Role add successful!"));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string email, string role)
        {
            if (!MailAddress.TryCreate(email, out var _)) return View("_Errors", new Error("Error!", "Email is invalid."));

            var my = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByNameAsync(email);
            if (user == null || my?.Id == user.Id) return View("_Errors", new Error("Error!", "Error."));
            try
            {
                await _userManager.RemoveFromRolesAsync(user, new List<string> { role });
            }
            catch
            {
                return View("_Errors", new Error("Error!", "Error."));
            }
            return View("_Errors", new Error("Success!", "Role add successful!"));
        }
        #endregion
    }
}
