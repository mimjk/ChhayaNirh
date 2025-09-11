namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRentTypeToPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Rent_Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Rent_Type");
        }
    }
}
