using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Entity;
using OnlineStore.Extension;
using OnlineStore.Models;
using System.Linq;
using System.Threading.Tasks;
using static OnlineStore.Service.AccountService;

namespace OnlineStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICustomExtension _customExtension;
        private readonly IAccountService _accountService;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context, ICustomExtension customExtension,
            IAccountService accountService)
        {
            _logger = logger;
            _context = context;
            _customExtension = customExtension;
            _accountService = accountService;
        }

        // GET: User
        public IActionResult Index()
        {
            var role = _accountService.RoleList();
            var users = _context.Users.ToList();

            foreach (var item in users)
            {
                item.RoleName = (role.Where(r => r.Id == item.RoleId).Select(r => r.RoleName)).FirstOrDefault();
            }

            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var roleList = _accountService.RoleList();
            ViewBag.RoleList = roleList;
            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            var user = new User();
            user.Active = true; //set Active to be checked
            user.RoleId = 0;

            ViewBag.RoleList = _accountService.RoleListItem();
            return View(user);
        }



        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Password,Email,RoleId,Active")] User user)
        {
            if (user.RoleId == 0)
            {
                ModelState.AddModelError("RoleId", "Role is mandatory");
            }

            if (ModelState.IsValid)
            {
                var existUsers = _context.Users.Where(u => u.Email == user.Email).Select(u => u).ToList();

                if (existUsers.Count == 0)
                {
                    user.Password = _customExtension.Encrypt(user.Password.Trim());
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Email", "User email alredy exist.!");
                }
            }

            ViewBag.RoleList = _accountService.RoleListItem();
            return View(user);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            user.Active = true;
            user.RoleId = 2;
            user.Password = _customExtension.Encrypt(user.Password.Trim());
            if (user.Email != "" && user.Password != "" && user.Name != "")
            {
                var existUsers = _context.Users.Where(u => u.Email == user.Email).Select(u => u).ToList();

                if (existUsers.Count == 0)
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Email", "User email alredy exist.!");
                }
            }
            ViewBag.RoleList = _accountService.RoleListItem();
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                user.Password = _customExtension.Decrypt(user.Password.Trim());
            }

            ViewBag.RoleList = _accountService.RoleListItem();
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Password,Email,RoleId,Active")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            if (user.RoleId == 0)
            {
                ModelState.AddModelError("RoleId", "Role is mandatory");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Password = _customExtension.Encrypt(user.Password.Trim());
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoleList = _accountService.RoleListItem();
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.RoleList = _accountService.RoleListItem();
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
