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
    
    public partial class DepartmentThemeCannotBeNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_Department", "ThemeId", "dbo.ClinicalGuidelinesApp_Theme");
            DropIndex("dbo.ClinicalGuidelinesApp_Department", new[] { "ThemeId" });
            AlterColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeId", c => c.Int(nullable: false));
            CreateIndex("dbo.ClinicalGuidelinesApp_Department", "ThemeId");
            AddForeignKey("dbo.ClinicalGuidelinesApp_Department", "ThemeId", "dbo.ClinicalGuidelinesApp_Theme", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_Department", "ThemeId", "dbo.ClinicalGuidelinesApp_Theme");
            DropIndex("dbo.ClinicalGuidelinesApp_Department", new[] { "ThemeId" });
            AlterColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeId", c => c.Int());
            CreateIndex("dbo.ClinicalGuidelinesApp_Department", "ThemeId");
            AddForeignKey("dbo.ClinicalGuidelinesApp_Department", "ThemeId", "dbo.ClinicalGuidelinesApp_Theme", "Id");
        }
    }
}
