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
    
    public partial class ArticleRevisionToDepartmentManyToMany : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevision", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", new[] { "DepartmentId" });
            CreateTable(
                "dbo.ClinicalGuidelinesApp_ArticleRevisionDepartments",
                c => new
                    {
                        ArticleRevisionId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ArticleRevisionId, t.DepartmentId })
                .ForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevision", t => t.ArticleRevisionId, cascadeDelete: true)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Department", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.ArticleRevisionId)
                .Index(t => t.DepartmentId);
            
            DropColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "DepartmentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "DepartmentId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevisionDepartments", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevisionDepartments", "ArticleRevisionId", "dbo.ClinicalGuidelinesApp_ArticleRevision");
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevisionDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevisionDepartments", new[] { "ArticleRevisionId" });
            DropTable("dbo.ClinicalGuidelinesApp_ArticleRevisionDepartments");
            CreateIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "DepartmentId");
            AddForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevision", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department", "Id", cascadeDelete: true);
        }
    }
}
