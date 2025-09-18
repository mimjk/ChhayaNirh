namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsVerifiedToPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "IsVerified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "IsVerified");
        }
    }
}
