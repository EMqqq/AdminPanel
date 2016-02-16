using AdminPanel.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace AdminPanel.DataAccessLayer
{
    public class AdminPanelContext : DbContext
    {
        public AdminPanelContext() : base("name=AdminPanel") { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<FilePath> FilePaths { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("library");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<FilePath>().HasOptional(p => p.Color).WithOptionalPrincipal();
        }
    }
}