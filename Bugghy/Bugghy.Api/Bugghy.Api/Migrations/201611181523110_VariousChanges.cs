namespace AdMaiora.Bugghy.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VariousChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Messages", new[] { "Sender" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Messages", "Sender");
        }
    }
}
