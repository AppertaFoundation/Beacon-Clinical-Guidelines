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
    
    public partial class DepartmentContainers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "Container", c => c.Boolean(nullable: false));
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ContainerId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ContainerId");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "Container");
        }
    }
}
