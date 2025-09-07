namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLatLongToPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Latitude", c => c.Double());
            AddColumn("dbo.Posts", "Longitude", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Longitude");
            DropColumn("dbo.Posts", "Latitude");
        }
    }
}
