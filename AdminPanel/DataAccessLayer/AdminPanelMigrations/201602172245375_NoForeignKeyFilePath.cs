namespace AdminPanel.DataAccessLayer.AdminPanelMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoForeignKeyFilePath : DbMigration
    {
        public override void Up()
        {
            DropColumn("library.FilePath", "ColorId");
        }
        
        public override void Down()
        {
            AddColumn("library.FilePath", "ColorId", c => c.Int());
        }
    }
}
