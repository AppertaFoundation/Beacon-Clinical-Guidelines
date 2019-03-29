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
    
    public partial class PouchIdOnSubsectionTabs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "_id", c => c.Guid(nullable: false));
            CreateIndex("dbo.ClinicalGuidelinesApp_SubSectionTab", "_id", name: "PouchDb_id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ClinicalGuidelinesApp_SubSectionTab", "PouchDb_id");
            DropColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "_id");
        }
    }
}
