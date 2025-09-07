namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNIDVerificationToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "NIDDocumentPath", c => c.String());
            AddColumn("dbo.Users", "IsVerified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsVerified");
            DropColumn("dbo.Users", "NIDDocumentPath");
        }
    }
}
