namespace AdminPanel.DataAccessLayer.AdminPanelMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LessProperties : DbMigration
    {
        public override void Up()
        {
            DropColumn("library.Product", "Nosek");
            DropColumn("library.Product", "Numeracja");
            DropColumn("library.Product", "Obcas");
            DropColumn("library.Product", "Ocieplenie");
            DropColumn("library.Product", "Wyściółka");
            DropColumn("library.Product", "Zapięcie");
        }
        
        public override void Down()
        {
            AddColumn("library.Product", "Zapięcie", c => c.String(nullable: false));
            AddColumn("library.Product", "Wyściółka", c => c.String(nullable: false));
            AddColumn("library.Product", "Ocieplenie", c => c.String(nullable: false));
            AddColumn("library.Product", "Obcas", c => c.String(nullable: false));
            AddColumn("library.Product", "Numeracja", c => c.String(nullable: false));
            AddColumn("library.Product", "Nosek", c => c.String(nullable: false));
        }
    }
}
