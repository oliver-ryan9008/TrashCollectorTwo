namespace TrashCollector2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IgrationMayEcondSay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "UserId");
        }
    }
}
