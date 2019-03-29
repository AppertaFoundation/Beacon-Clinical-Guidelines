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
    
    public partial class SimplifyFileModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", "ArticleRevisionId", "dbo.ClinicalGuidelinesApp_ArticleRevision");
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", "FileId", "dbo.ClinicalGuidelinesApp_File");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", new[] { "ArticleRevisionId" });
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", new[] { "FileId" });
            AddColumn("dbo.ClinicalGuidelinesApp_File", "Server", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_File", "ArticleRevision_Id", c => c.Int());
            CreateIndex("dbo.ClinicalGuidelinesApp_File", "ArticleRevision_Id");
            AddForeignKey("dbo.ClinicalGuidelinesApp_File", "ArticleRevision_Id", "dbo.ClinicalGuidelinesApp_ArticleRevision", "Id");
            DropTable("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_ArticleRevisionFiles",
                c => new
                    {
                        ArticleRevisionId = c.Int(nullable: false),
                        FileId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ArticleRevisionId, t.FileId });
            
            DropForeignKey("dbo.ClinicalGuidelinesApp_File", "ArticleRevision_Id", "dbo.ClinicalGuidelinesApp_ArticleRevision");
            DropIndex("dbo.ClinicalGuidelinesApp_File", new[] { "ArticleRevision_Id" });
            DropColumn("dbo.ClinicalGuidelinesApp_File", "ArticleRevision_Id");
            DropColumn("dbo.ClinicalGuidelinesApp_File", "Server");
            CreateIndex("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", "FileId");
            CreateIndex("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", "ArticleRevisionId");
            AddForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", "FileId", "dbo.ClinicalGuidelinesApp_File", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevisionFiles", "ArticleRevisionId", "dbo.ClinicalGuidelinesApp_ArticleRevision", "Id", cascadeDelete: true);
        }
    }
}
