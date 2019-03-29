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
    
    public partial class UserDepartmentOneToMany : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_UserAdministrationDepartments",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.DepartmentId })
                .ForeignKey("dbo.ClinicalGuidelinesApp_User", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Department", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_UserDepartments",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.DepartmentId })
                .ForeignKey("dbo.ClinicalGuidelinesApp_User", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Department", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_UserDepartments", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropForeignKey("dbo.ClinicalGuidelinesApp_UserDepartments", "UserId", "dbo.ClinicalGuidelinesApp_User");
            DropForeignKey("dbo.ClinicalGuidelinesApp_UserAdministrationDepartments", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropForeignKey("dbo.ClinicalGuidelinesApp_UserAdministrationDepartments", "UserId", "dbo.ClinicalGuidelinesApp_User");
            DropIndex("dbo.ClinicalGuidelinesApp_UserDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.ClinicalGuidelinesApp_UserDepartments", new[] { "UserId" });
            DropIndex("dbo.ClinicalGuidelinesApp_UserAdministrationDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.ClinicalGuidelinesApp_UserAdministrationDepartments", new[] { "UserId" });
            DropTable("dbo.ClinicalGuidelinesApp_UserDepartments");
            DropTable("dbo.ClinicalGuidelinesApp_UserAdministrationDepartments");
        }
    }
}
