namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddElectricityBillToPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "ElectricityBillPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "ElectricityBillPath");
        }
    }
}
