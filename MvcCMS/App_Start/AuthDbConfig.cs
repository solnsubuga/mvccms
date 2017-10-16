using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCMS.Data;
using MvcCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.App_Start
{
  public class AuthDbConfig
    {
        public static async Task RegisterAdminAsync()
        {
            
            using(var users = new UserRepository())
            {
                var user =  await users.GetUserByNameAsync("admin");
                
                if(user == null)
                {
                    var adminUser = new CmsUser
                    {
                        UserName = "admin",
                        Email = "admin@cms.com",
                        DisplayName = "Administartor"
                    };

                   await users.CreateAsync(adminUser, "Passw0rd1234!");
                }
            }

            using(var roles = new RoleRepository())
            {
                if(roles.GetRoleByNameAsync("admin") == null)
                {
                    await roles.CreateAsync(new IdentityRole("admin"));
                }
                if (roles.GetRoleByNameAsync("editor") == null)
                {
                    await roles.CreateAsync(new IdentityRole("editor"));
                }
                if (roles.GetRoleByNameAsync("author") == null)
                {
                    await roles.CreateAsync(new IdentityRole("author"));
                }
            }
        }
    }
}
