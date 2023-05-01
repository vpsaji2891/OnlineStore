using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Models;
using System.Collections.Generic;

namespace OnlineStore.Service
{
    public class AccountService
    {
        public interface IAccountService
        {
            public User IsUserValid(string username, string password);

            public List<Role> RoleList();

            public IEnumerable<SelectListItem> RoleListItem();
        }

    }
}
