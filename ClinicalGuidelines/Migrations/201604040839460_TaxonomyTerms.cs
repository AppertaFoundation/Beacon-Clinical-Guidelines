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
    
    public partial class TaxonomyTerms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_Term",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TermArticleRevisions",
                c => new
                    {
                        Term_Id = c.Int(nullable: false),
                        ArticleRevision_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Term_Id, t.ArticleRevision_Id })
                .ForeignKey("dbo.ClinicalGuidelinesApp_Term", t => t.Term_Id, cascadeDelete: true)
                .ForeignKey("dbo.ClinicalGuidelinesApp_ArticleRevision", t => t.ArticleRevision_Id, cascadeDelete: true)
                .Index(t => t.Term_Id)
                .Index(t => t.ArticleRevision_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TermArticleRevisions", "ArticleRevision_Id", "dbo.ClinicalGuidelinesApp_ArticleRevision");
            DropForeignKey("dbo.TermArticleRevisions", "Term_Id", "dbo.ClinicalGuidelinesApp_Term");
            DropIndex("dbo.TermArticleRevisions", new[] { "ArticleRevision_Id" });
            DropIndex("dbo.TermArticleRevisions", new[] { "Term_Id" });
            DropTable("dbo.TermArticleRevisions");
            DropTable("dbo.ClinicalGuidelinesApp_Term");
        }
    }
}
