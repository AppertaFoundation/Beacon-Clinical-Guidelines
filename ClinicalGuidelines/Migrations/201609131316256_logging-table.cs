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
    
    public partial class loggingtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_Log",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MethodCalled = c.String(maxLength: 255),
                        RequestUri = c.String(maxLength: 255),
                        Response = c.String(unicode: false),
                        SamAccountName = c.String(maxLength: 50),
                        DateTimeCalled = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.MethodCalled, name: "MethodCalled")
                .Index(t => t.DateTimeCalled, name: "DateTimeCalled");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ClinicalGuidelinesApp_Log", "DateTimeCalled");
            DropIndex("dbo.ClinicalGuidelinesApp_Log", "MethodCalled");
            DropTable("dbo.ClinicalGuidelinesApp_Log");
        }
    }
}
