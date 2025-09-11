namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RealTimeChat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chats", "IsDelivered", c => c.Boolean(nullable: false));
            AddColumn("dbo.Chats", "DeliveredAt", c => c.DateTime());
            AddColumn("dbo.Chats", "ReadAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chats", "ReadAt");
            DropColumn("dbo.Chats", "DeliveredAt");
            DropColumn("dbo.Chats", "IsDelivered");
        }
    }
}
