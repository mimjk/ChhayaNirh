namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostId = c.Int(nullable: false),
                        ReportedByUserId = c.Int(nullable: false),
                        ReportType = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 1000),
                        ReportedAt = c.DateTime(nullable: false),
                        IsResolved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reports");
        }
    }
}
