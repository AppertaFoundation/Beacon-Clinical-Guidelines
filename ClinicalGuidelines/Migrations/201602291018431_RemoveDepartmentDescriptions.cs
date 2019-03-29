//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
namespace ClinicalGuidelines.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDepartmentDescriptions : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "Description", c => c.String(unicode: false));
        }
    }
}
