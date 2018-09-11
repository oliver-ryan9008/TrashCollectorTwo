namespace TrashCollector2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IgrationOurthFay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "IsConfirmed", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "IsConfirmed");
        }
    }
}
