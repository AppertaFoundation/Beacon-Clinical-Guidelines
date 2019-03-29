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
    
    public partial class MultipleDepartmentsPerSubSectionTab : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_SubSectionTabDepartments",
                c => new
                    {
                        SubSectionTabId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubSectionTabId, t.DepartmentId })
                .ForeignKey("dbo.ClinicalGuidelinesApp_SubSectionTab", t => t.SubSectionTabId, cascadeDelete: true)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Department", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.SubSectionTabId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_SubSectionTabDepartments", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropForeignKey("dbo.ClinicalGuidelinesApp_SubSectionTabDepartments", "SubSectionTabId", "dbo.ClinicalGuidelinesApp_SubSectionTab");
            DropIndex("dbo.ClinicalGuidelinesApp_SubSectionTabDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.ClinicalGuidelinesApp_SubSectionTabDepartments", new[] { "SubSectionTabId" });
            DropTable("dbo.ClinicalGuidelinesApp_SubSectionTabDepartments");
        }
    }
}
