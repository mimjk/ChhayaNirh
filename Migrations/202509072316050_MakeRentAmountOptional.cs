namespace ChhayaNirh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeRentAmountOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Posts", "RentAmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Posts", "RentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
