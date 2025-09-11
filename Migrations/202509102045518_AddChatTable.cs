namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChatTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderId = c.Int(nullable: false),
                        ReceiverId = c.Int(nullable: false),
                        MessageText = c.String(nullable: false, maxLength: 1000),
                        SentAt = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.ReceiverId)
                .ForeignKey("dbo.Users", t => t.SenderId)
                .Index(t => t.SenderId)
                .Index(t => t.ReceiverId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Chats", "SenderId", "dbo.Users");
            DropForeignKey("dbo.Chats", "ReceiverId", "dbo.Users");
            DropIndex("dbo.Chats", new[] { "ReceiverId" });
            DropIndex("dbo.Chats", new[] { "SenderId" });
            DropTable("dbo.Chats");
        }
    }
}
