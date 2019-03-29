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
    
    public partial class MoreChangesToFilesMethod : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_File", "ArticleRevision_Id", "dbo.ClinicalGuidelinesApp_ArticleRevision");
            DropIndex("dbo.ClinicalGuidelinesApp_File", new[] { "ArticleRevision_Id" });
            AddColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "Files", c => c.String());
            DropTable("dbo.ClinicalGuidelinesApp_File");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_File",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Extension = c.String(),
                        Server = c.String(),
                        UploadedBy = c.String(),
                        ArticleRevision_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "Files");
            CreateIndex("dbo.ClinicalGuidelinesApp_File", "ArticleRevision_Id");
            AddForeignKey("dbo.ClinicalGuidelinesApp_File", "ArticleRevision_Id", "dbo.ClinicalGuidelinesApp_ArticleRevision", "Id");
        }
    }
}
