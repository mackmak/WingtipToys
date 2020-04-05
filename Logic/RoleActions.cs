using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace WingtipToys.Logic
{
    internal class RoleActions
    {
        internal void AddUserAndRole()
        {
            var appDbContext = new ApplicationDbContext();
            IdentityResult idRoleResult = null;
            IdentityResult idUserResult = null;

            var roleStore = new RoleStore<IdentityRole>(appDbContext);

            var roleManager = new RoleManager<IdentityRole>(roleStore);

            //Create the role "canEdit" if not created yet
            if (!roleManager.RoleExists("canEdit"))
            {
                idRoleResult = roleManager.Create(new IdentityRole { Name = "canEdit"});
            }

            var userStore = new UserStore<ApplicationUser>(appDbContext);

            var userManager = new UserManager<ApplicationUser>(userStore);
            var applicationUser = new ApplicationUser
            {
                UserName = "userWhoCanEdit@wingtiptoys.com",
                Email = "userWhoCanEdit@wingtiptoys.com"
            };

            idUserResult = userManager.Create(applicationUser, "Pa$$word1");

            //If user was created, add it to the "canEdit" role
            var userId = userManager.FindByEmail("userWhoCanEdit@wingtiptoys.com").Id;
            if (!userManager.IsInRole(userId, "canEdit"))
            {
                idUserResult = userManager.AddToRole(userId, "canEdit");
            }
        }
    }
}