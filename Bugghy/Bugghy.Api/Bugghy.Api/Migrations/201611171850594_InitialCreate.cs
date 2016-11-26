namespace AdMaiora.Bugghy.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        AttachmentId = c.Int(nullable: false, identity: true),
                        MessageId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        FileName = c.String(),
                        FileUrl = c.String(),
                        CreationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AttachmentId)
                .Index(t => t.MessageId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Gimmicks",
                c => new
                    {
                        GimmickId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Owner = c.String(),
                        ImageUrl = c.String(),
                        CreationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.GimmickId);
            
            CreateTable(
                "dbo.Issues",
                c => new
                    {
                        IssueId = c.Int(nullable: false, identity: true),
                        GimmickId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Sender = c.String(),
                        Code = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(),
                        ReplyDate = c.DateTime(),
                        ClosedDate = c.DateTime(),
                        IsClosed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IssueId)
                .Index(t => t.GimmickId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        IssueId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Sender = c.String(),
                        Content = c.String(),
                        PostDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MessageId)
                .Index(t => t.IssueId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Password = c.String(),
                        LoginDate = c.DateTime(),
                        LastActiveDate = c.DateTime(),
                        AuthAccessToken = c.String(),
                        AuthExpirationDate = c.DateTime(),
                        Ticket = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {            
            DropIndex("dbo.Messages", new[] { "UserId" });
            DropIndex("dbo.Messages", new[] { "IssueId" });
            DropIndex("dbo.Issues", new[] { "UserId" });
            DropIndex("dbo.Issues", new[] { "GimmickId" });
            DropIndex("dbo.Attachments", new[] { "UserId" });
            DropIndex("dbo.Attachments", new[] { "MessageId" });
            DropTable("dbo.Users");
            DropTable("dbo.Messages");
            DropTable("dbo.Issues");
            DropTable("dbo.Gimmicks");
            DropTable("dbo.Attachments");
        }
    }
}
