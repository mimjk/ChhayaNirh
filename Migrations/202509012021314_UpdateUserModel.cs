namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "FullName", c => c.String(nullable: false));
            AddColumn("dbo.Users", "Phone", c => c.String(nullable: false));
            AddColumn("dbo.Users", "Password", c => c.String(nullable: false));
            AddColumn("dbo.Users", "UserType", c => c.String(nullable: false));
            AddColumn("dbo.Users", "NIDScanPath", c => c.String());
            AddColumn("dbo.Users", "ElectricityBillPath", c => c.String());
            AddColumn("dbo.Users", "CreatedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
            DropColumn("dbo.Users", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Users", "CreatedAt");
            DropColumn("dbo.Users", "ElectricityBillPath");
            DropColumn("dbo.Users", "NIDScanPath");
            DropColumn("dbo.Users", "UserType");
            DropColumn("dbo.Users", "Password");
            DropColumn("dbo.Users", "Phone");
            DropColumn("dbo.Users", "FullName");
        }
    }
}
