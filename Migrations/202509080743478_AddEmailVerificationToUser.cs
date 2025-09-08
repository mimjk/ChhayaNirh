namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmailVerificationToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsEmailVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "EmailVerificationCode", c => c.String());
            AddColumn("dbo.Users", "EmailVerificationExpiry", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "EmailVerificationExpiry");
            DropColumn("dbo.Users", "EmailVerificationCode");
            DropColumn("dbo.Users", "IsEmailVerified");
        }
    }
}
