namespace TrashCollector2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IgrationMayIrstFay : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        FullName = c.String(),
                        StreetAddress = c.String(),
                        ZipCode = c.Int(nullable: false),
                        WeeklyPickupDay = c.String(),
                        OneTimePickupDate = c.DateTime(),
                        MoneyOwed = c.Int(),
                        StartOfDelayedPickup = c.DateTime(),
                        EndOfDelayedPickup = c.DateTime(),
                        IsOnHold = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        FullName = c.String(),
                        ZipCode = c.Int(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "UserRole", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UserRole");
            DropTable("dbo.Employees");
            DropTable("dbo.Customers");
        }
    }
}
