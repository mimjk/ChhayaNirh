using System.Data.Entity.Migrations;

public partial class AddIsBannedToUser : DbMigration
{
    public override void Up()
    {
        AddColumn("dbo.Users", "IsBanned", c => c.Boolean(nullable: false, defaultValue: false));
    }

    public override void Down()
    {
        DropColumn("dbo.Users", "IsBanned");
    }
}