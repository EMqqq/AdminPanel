namespace AdminPanel.DataAccessLayer.IdentityContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "identity.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "identity.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("identity.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("identity.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "identity.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "identity.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("identity.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "identity.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("identity.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("identity.AspNetUserRoles", "UserId", "identity.AspNetUsers");
            DropForeignKey("identity.AspNetUserLogins", "UserId", "identity.AspNetUsers");
            DropForeignKey("identity.AspNetUserClaims", "UserId", "identity.AspNetUsers");
            DropForeignKey("identity.AspNetUserRoles", "RoleId", "identity.AspNetRoles");
            DropIndex("identity.AspNetUserLogins", new[] { "UserId" });
            DropIndex("identity.AspNetUserClaims", new[] { "UserId" });
            DropIndex("identity.AspNetUsers", "UserNameIndex");
            DropIndex("identity.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("identity.AspNetUserRoles", new[] { "UserId" });
            DropIndex("identity.AspNetRoles", "RoleNameIndex");
            DropTable("identity.AspNetUserLogins");
            DropTable("identity.AspNetUserClaims");
            DropTable("identity.AspNetUsers");
            DropTable("identity.AspNetUserRoles");
            DropTable("identity.AspNetRoles");
        }
    }
}
