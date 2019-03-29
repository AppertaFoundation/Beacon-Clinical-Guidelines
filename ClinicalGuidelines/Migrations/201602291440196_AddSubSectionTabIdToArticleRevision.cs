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
    
    public partial class AddSubSectionTabIdToArticleRevision : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "SubSectionTabId", c => c.Int());
            CreateIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "SubSectionTabId", name: "SubSectionTabId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "SubSectionTabId");
            DropColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "SubSectionTabId");
        }
    }
}
