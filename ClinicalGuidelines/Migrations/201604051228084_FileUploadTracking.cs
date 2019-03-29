//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
namespace ClinicalGuidelines.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileUploadTracking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_File",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Extension = c.Int(nullable: false),
                        UploadedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_ArticleRevisionFiles",
                c => new
                    {
                        ArticleRevisionId = c.Int(nullable: false),
                        FileId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ArticleRevisionId, t.FileId })
                .ForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevision", t => t.ArticleRevisionId, cascadeDelete: true)
                .ForeignKey("dbo.ClinicalGuidelinesApp_File", t => t.FileId, cascadeDelete: true)
                .Index(t => t.ArticleRevisionId)
                .Index(t => t.FileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", "FileId", "dbo.ClinicalGuidelinesApp_File");
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", "ArticleRevisionId", "dbo.ClinicalGuidelinesApp_ArticleRevision");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", new[] { "FileId" });
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", new[] { "ArticleRevisionId" });
            DropTable("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles");
            DropTable("dbo.ClinicalGuidelinesApp_File");
        }
    }
}
