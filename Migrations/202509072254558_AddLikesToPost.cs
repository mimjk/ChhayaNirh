namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLikesToPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Likes", "Post_Id", c => c.Int());
            CreateIndex("dbo.Likes", "Post_Id");
            AddForeignKey("dbo.Likes", "Post_Id", "dbo.Posts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Likes", "Post_Id", "dbo.Posts");
            DropIndex("dbo.Likes", new[] { "Post_Id" });
            DropColumn("dbo.Likes", "Post_Id");
        }
    }
}
