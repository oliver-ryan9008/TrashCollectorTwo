namespace TrashCollector2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IgrationMayIrdThay : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Customers");
            DropPrimaryKey("dbo.Employees");
            AlterColumn("dbo.Customers", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Employees", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Customers", "UserId");
            AddPrimaryKey("dbo.Employees", "UserId");
            DropColumn("dbo.Customers", "Id");
            DropColumn("dbo.Employees", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Customers", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Employees");
            DropPrimaryKey("dbo.Customers");
            AlterColumn("dbo.Employees", "UserId", c => c.String());
            AlterColumn("dbo.Customers", "UserId", c => c.String());
            AddPrimaryKey("dbo.Employees", "Id");
            AddPrimaryKey("dbo.Customers", "Id");
        }
    }
}
