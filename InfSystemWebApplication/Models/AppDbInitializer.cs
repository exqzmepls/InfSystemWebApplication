using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InfSystemWebApplication.Models
{
    public class AppDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // создаем роли
            var admin = new IdentityRole { Name = "admin" };
            var favored_user = new IdentityRole { Name = "favored_user" };
            var user = new IdentityRole { Name = "user" };

            // добавляем роли в бд
            roleManager.Create(admin);
            roleManager.Create(favored_user);
            roleManager.Create(user);

            // создаем пользователей
            var defaultAdmin = new ApplicationUser { Email = "admin@mail.ru", UserName = "admin@mail.ru", EmployeeId = 0 };
            var result = userManager.Create(defaultAdmin, "123_Qwe");

            // если создание пользователя прошло успешно
            if (result.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(defaultAdmin.Id, admin.Name);
                userManager.AddToRole(defaultAdmin.Id, favored_user.Name);
                userManager.AddToRole(defaultAdmin.Id, user.Name);
            }

            base.Seed(context);
        }
    }
}