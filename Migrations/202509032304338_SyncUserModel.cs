namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncUserModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Phone", c => c.String(nullable: false, maxLength: 11));
        }

        public override void Down()
        {

            AlterColumn("dbo.Users", "Phone", c => c.String(nullable: false));
        }
    }
}
