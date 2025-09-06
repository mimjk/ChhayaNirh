namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncPostAndCommentModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        CommentText = c.String(nullable: false, maxLength: 1000),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.PostId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PostType = c.String(),
                        Area = c.String(nullable: false),
                        HouseType = c.String(),
                        Bedrooms = c.Int(),
                        Bathrooms = c.Int(),
                        RentAmount = c.Decimal(precision: 18, scale: 2),
                        Budget = c.Decimal(precision: 18, scale: 2),
                        Amenities = c.String(),
                        Description = c.String(),
                        MediaPaths = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        LikeCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Comments", "PostId", "dbo.Posts");
            DropForeignKey("dbo.Posts", "UserId", "dbo.Users");
            DropIndex("dbo.Posts", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "PostId" });
            DropTable("dbo.Posts");
            DropTable("dbo.Comments");
        }
    }
}
