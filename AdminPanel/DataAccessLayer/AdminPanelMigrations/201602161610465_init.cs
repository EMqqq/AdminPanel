namespace AdminPanel.DataAccessLayer.AdminPanelMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "library.Category",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "library.Product",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false),
                        Desc = c.String(),
                        Material = c.String(nullable: false),
                        NormalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(),
                        CategoryId = c.Int(nullable: false),
                        ColorId = c.Int(nullable: false),
                        Nosek = c.String(nullable: false),
                        Numeracja = c.String(nullable: false),
                        Obcas = c.String(nullable: false),
                        Ocieplenie = c.String(nullable: false),
                        Wyściółka = c.String(nullable: false),
                        Zapięcie = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("library.Category", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("library.Color", t => t.ColorId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.ColorId);
            
            CreateTable(
                "library.Color",
                c => new
                    {
                        ColorId = c.Int(nullable: false, identity: true),
                        ColorName = c.String(nullable: false),
                        FilePathId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ColorId)
                .ForeignKey("library.FilePath", t => t.FilePathId, cascadeDelete: true)
                .Index(t => t.FilePathId);
            
            CreateTable(
                "library.FilePath",
                c => new
                    {
                        FilePathId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        FileType = c.Int(nullable: false),
                        ProductId = c.Int(),
                        ColorId = c.Int(),
                    })
                .PrimaryKey(t => t.FilePathId)
                .ForeignKey("library.Product", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "library.Supplier",
                c => new
                    {
                        SupplierId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransportTime = c.String(nullable: false),
                        DeliveryMethodId = c.Int(nullable: false),
                        FilePathId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierId)
                .ForeignKey("library.DeliveryMethod", t => t.DeliveryMethodId, cascadeDelete: true)
                .ForeignKey("library.FilePath", t => t.FilePathId, cascadeDelete: true)
                .Index(t => t.DeliveryMethodId)
                .Index(t => t.FilePathId);
            
            CreateTable(
                "library.DeliveryMethod",
                c => new
                    {
                        DeliveryMethodId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DeliveryMethodId);
            
            CreateTable(
                "library.Size",
                c => new
                    {
                        SizeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SizeId);
            
            CreateTable(
                "library.SizeProduct",
                c => new
                    {
                        Size_SizeId = c.Int(nullable: false),
                        Product_ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Size_SizeId, t.Product_ProductId })
                .ForeignKey("library.Size", t => t.Size_SizeId, cascadeDelete: true)
                .ForeignKey("library.Product", t => t.Product_ProductId, cascadeDelete: true)
                .Index(t => t.Size_SizeId)
                .Index(t => t.Product_ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("library.SizeProduct", "Product_ProductId", "library.Product");
            DropForeignKey("library.SizeProduct", "Size_SizeId", "library.Size");
            DropForeignKey("library.Product", "ColorId", "library.Color");
            DropForeignKey("library.Supplier", "FilePathId", "library.FilePath");
            DropForeignKey("library.Supplier", "DeliveryMethodId", "library.DeliveryMethod");
            DropForeignKey("library.FilePath", "ProductId", "library.Product");
            DropForeignKey("library.Color", "FilePathId", "library.FilePath");
            DropForeignKey("library.Product", "CategoryId", "library.Category");
            DropIndex("library.SizeProduct", new[] { "Product_ProductId" });
            DropIndex("library.SizeProduct", new[] { "Size_SizeId" });
            DropIndex("library.Supplier", new[] { "FilePathId" });
            DropIndex("library.Supplier", new[] { "DeliveryMethodId" });
            DropIndex("library.FilePath", new[] { "ProductId" });
            DropIndex("library.Color", new[] { "FilePathId" });
            DropIndex("library.Product", new[] { "ColorId" });
            DropIndex("library.Product", new[] { "CategoryId" });
            DropTable("library.SizeProduct");
            DropTable("library.Size");
            DropTable("library.DeliveryMethod");
            DropTable("library.Supplier");
            DropTable("library.FilePath");
            DropTable("library.Color");
            DropTable("library.Product");
            DropTable("library.Category");
        }
    }
}
