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
    
    public partial class ArticlesApproval : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "Approval", c => c.Boolean(nullable: false));
            CreateIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "Approval", name: "Approval");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ClinicalGuidelinesApp_ArticleRevision", "Approval");
            DropColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "Approval");
        }
    }
}
