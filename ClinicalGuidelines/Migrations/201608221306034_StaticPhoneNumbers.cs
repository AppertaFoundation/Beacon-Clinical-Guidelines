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
    
    public partial class StaticPhoneNumbers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_StaticPhoneNumber",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Title = c.String(),
                        PhoneNumber = c.String(),
                        DepartmentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClinicalGuidelinesApp_Department", t => t.DepartmentId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_StaticPhoneNumber", "DepartmentId", "dbo.ClinicalGuidelinesApp_Department");
            DropIndex("dbo.ClinicalGuidelinesApp_StaticPhoneNumber", new[] { "DepartmentId" });
            DropTable("dbo.ClinicalGuidelinesApp_StaticPhoneNumber");
        }
    }
}
