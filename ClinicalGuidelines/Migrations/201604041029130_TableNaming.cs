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
    
    public partial class TableNaming : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TermArticleRevisions", newName: "ClinicalGuidelinesApp_ArticleRevisionTerms");
            RenameColumn(table: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", name: "Term_Id", newName: "TermId");
            RenameColumn(table: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", name: "ArticleRevision_Id", newName: "ArticleRevisionId");
            RenameIndex(table: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", name: "IX_ArticleRevision_Id", newName: "IX_ArticleRevisionId");
            RenameIndex(table: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", name: "IX_Term_Id", newName: "IX_TermId");
            DropPrimaryKey("dbo.ClinicalGuidelinesApp_ArticleRevisionTerms");
            AddPrimaryKey("dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", new[] { "ArticleRevisionId", "TermId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ClinicalGuidelinesApp_ArticleRevisionTerms");
            AddPrimaryKey("dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", new[] { "Term_Id", "ArticleRevision_Id" });
            RenameIndex(table: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", name: "IX_TermId", newName: "IX_Term_Id");
            RenameIndex(table: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", name: "IX_ArticleRevisionId", newName: "IX_ArticleRevision_Id");
            RenameColumn(table: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", name: "ArticleRevisionId", newName: "ArticleRevision_Id");
            RenameColumn(table: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", name: "TermId", newName: "Term_Id");
            RenameTable(name: "dbo.ClinicalGuidelinesApp_ArticleRevisionTerms", newName: "TermArticleRevisions");
        }
    }
}
