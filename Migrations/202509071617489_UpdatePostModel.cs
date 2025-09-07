namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePostModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Posts", "PostType", c => c.String(nullable: false));
            AlterColumn("dbo.Posts", "RentAmount", c => c.Decimal(nullable: true, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Posts", "RentAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Posts", "PostType", c => c.String());
        }
    }
}
