namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateUserModelForBdValidation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "NIDNumber", c => c.String());
            AddColumn("dbo.Users", "PresentAddress", c => c.String());
            AddColumn("dbo.Users", "DateOfBirth", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "PermanentAddress", c => c.String());
            DropColumn("dbo.Users", "NIDScanPath");
            DropColumn("dbo.Users", "ElectricityBillPath");
        }

        public override void Down()
        {
            AddColumn("dbo.Users", "ElectricityBillPath", c => c.String());
            AddColumn("dbo.Users", "NIDScanPath", c => c.String());
            DropColumn("dbo.Users", "DateOfBirth"); // Add this line
            DropColumn("dbo.Users", "PermanentAddress");
            DropColumn("dbo.Users", "PresentAddress");
            DropColumn("dbo.Users", "NIDNumber");
        }
    }
}