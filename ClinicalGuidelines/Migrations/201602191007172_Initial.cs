//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
namespace ClinicalGuidelines.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_ArticleRevision",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        SubTitle = c.String(maxLength: 100),
                        Abstract = c.String(maxLength: 255),
                        Body = c.String(unicode: false),
                        Live = c.Boolean(nullable: false),
                        Public = c.Boolean(nullable: false),
                        ReviewDateTime = c.DateTime(),
                        StartDateTime = c.DateTime(),
                        ExpiryDateTime = c.DateTime(),
                        UserId = c.Int(nullable: false),
                        RevisionDateTime = c.DateTime(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Department", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.ClinicalGuidelinesApp_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.Live, name: "Live")
                .Index(t => t.Public, name: "Public")
                .Index(t => t.ReviewDateTime, name: "ReviewDateTime")
                .Index(t => t.StartDateTime, name: "StartDateTime")
                .Index(t => t.ExpiryDateTime, name: "ExpiryDateTime")
                .Index(t => t.UserId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_Department",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        ShortName = c.String(maxLength: 20),
                        Archived = c.Boolean(nullable: false),
                        ArchiveDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 25),
                        Forename = c.String(maxLength: 25),
                        Surname = c.String(maxLength: 25),
                        JobTitle = c.String(maxLength: 100),
                        EmailAddress = c.String(maxLength: 100),
                        PhoneNumber = c.String(maxLength: 12),
                        SiteAdmin = c.Boolean(nullable: false),
                        LeadClinician = c.Boolean(nullable: false),
                        ContentEditor = c.Boolean(nullable: false),
                        Contact = c.Boolean(nullable: false),
                        DepartmentId = c.Int(),
                        Archived = c.Boolean(nullable: false),
                        ArchiveDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Department", t => t.DepartmentId)
                .Index(t => t.UserName, unique: true, name: "Username")
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_Article",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleRevisionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevision", t => t.ArticleRevisionId, cascadeDelete: true)
                .Index(t => t.ArticleRevisionId);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_SubSectionTab",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(),
                        Name = c.String(maxLength: 100),
                        Body = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Department", t => t.DepartmentId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_TemplateItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateId = c.Int(nullable: false),
                        BlockType = c.Int(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        Alignment = c.Int(nullable: false),
                        Archived = c.Boolean(nullable: false),
                        ArchiveDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Template", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Archived = c.Boolean(nullable: false),
                        ArchiveDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_TemplateItem", "TemplateId", "dbo.ClinicalGuidelinesApp_Template");
            DropForeignKey("dbo.ClinicalGuidelinesApp_SubSectionTab", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropForeignKey("dbo.ClinicalGuidelinesApp_Article", "ArticleRevisionId", "dbo.ClinicalGuidelinesApp_ArticleRevision");
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevision", "UserId", "dbo.ClinicalGuidelinesApp_User");
            DropForeignKey("dbo.ClinicalGuidelinesApp_User", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevision", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropIndex("dbo.ClinicalGuidelinesApp_TemplateItem", new[] { "TemplateId" });
            DropIndex("dbo.ClinicalGuidelinesApp_SubSectionTab", new[] { "DepartmentId" });
            DropIndex("dbo.ClinicalGuidelinesApp_Article", new[] { "ArticleRevisionId" });
            DropIndex("dbo.ClinicalGuidelinesApp_User", new[] { "DepartmentId" });
            DropIndex("dbo.ClinicalGuidelinesApp_User", "Username");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", new[] { "DepartmentId" });
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", new[] { "UserId" });
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "ExpiryDateTime");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "StartDateTime");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "ReviewDateTime");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "Public");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "Live");
            DropTable("dbo.ClinicalGuidelinesApp_Template");
            DropTable("dbo.ClinicalGuidelinesApp_TemplateItem");
            DropTable("dbo.ClinicalGuidelinesApp_SubSectionTab");
            DropTable("dbo.ClinicalGuidelinesApp_Article");
            DropTable("dbo.ClinicalGuidelinesApp_User");
            DropTable("dbo.ClinicalGuidelinesApp_Department");
            DropTable("dbo.ClinicalGuidelinesApp_ArticleRevision");
        }
    }
}
