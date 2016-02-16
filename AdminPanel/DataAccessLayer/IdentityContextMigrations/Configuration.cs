namespace AdminPanel.DataAccessLayer.IdentityContextMigrations
{
    using Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AdminPanel.DataAccessLayer.IdentityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataAccessLayer\IdentityContextMigrations";
        }

        protected override void Seed(AdminPanel.DataAccessLayer.IdentityContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!(context.Roles.Any(r => r.Name == "Admin")))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!(context.Users.Any(u => u.UserName == "master")))
            {
                var user = new ApplicationUser { UserName = "master@gmail.com", Email = "master@gmail.com", EmailConfirmed = true };
                userManager.Create(user, "password"); //CHANGE THIS ASAP!
                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
