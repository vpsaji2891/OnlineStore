using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Entity;
using OnlineStore.Extension;
using OnlineStore.Models;
using System.Collections.Generic;
using System.Linq;
using static OnlineStore.Service.AccountService;

namespace OnlineStore.Repository
{
    public class AccountRepository : IAccountService
    {
        private readonly ICustomExtension _customeExtension;
        private ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context, ICustomExtension customeExtension)
        {
            _context = context;
            _customeExtension = customeExtension;
        }

        public User IsUserValid(string username, string password)
        {
            List<User> user = new List<User>();

            if (username.Trim() != null && password.Trim() != null)
            {
                password = _customeExtension.Encrypt(password);

                user = _context.Users.Where(u => u.Email == username && u.Password == password).Select(u => u).ToList<User>();

            }

            return user.FirstOrDefault<User>();
        }


        public List<Role> RoleList()
        {
            var role1 = new Role()
            {
                Id = 1,
                RoleName = "Admin"
            };

            var role2 = new Role()
            {
                Id = 2,
                RoleName = "User"
            };

            List<Role> roleList = new List<Role>();

            roleList.Add(role1);
            roleList.Add(role2);

            return roleList;
        }

        public IEnumerable<SelectListItem> RoleListItem()
        {
            var roleList = RoleList().OrderBy(o => o.RoleName).ToList(); ;
            var item = new Role()
            {
                Id = 0,
                RoleName = "Select Role"
            };

            roleList.Insert(0, item);

            var items = roleList.Select(role => new SelectListItem
            {
                Text = role.RoleName.ToString(),
                Value = role.Id.ToString()
            });

            return items;
        }

    }
}
