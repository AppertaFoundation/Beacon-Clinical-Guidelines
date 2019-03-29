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
    
    public partial class SubSectionTabChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "Icon", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "Hue", c => c.String());
            DropColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "Body");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "Body", c => c.String(unicode: false));
            DropColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "Hue");
            DropColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "Icon");
        }
    }
}
