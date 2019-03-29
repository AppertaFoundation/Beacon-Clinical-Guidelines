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
    
    public partial class MoveThemeIntoDepartmentAgain : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_Department", "ThemeId", "dbo.ClinicalGuidelinesApp_Theme");
            DropIndex("dbo.ClinicalGuidelinesApp_Department", new[] { "ThemeId" });
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "MainColour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "SubColour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "BackgroundColour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationOne", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationTwo", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationThree", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationFour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationFive", c => c.String());
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeId");
            DropTable("dbo.ClinicalGuidelinesApp_Theme");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_Theme",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        MainColour = c.String(),
                        SubColour = c.String(),
                        BackgroundColour = c.String(),
                        SideColourVariationOne = c.String(),
                        SideColourVariationTwo = c.String(),
                        SideColourVariationThree = c.String(),
                        SideColourVariationFour = c.String(),
                        SideColourVariationFive = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeId", c => c.Int(nullable: false));
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationFive");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationFour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationThree");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationTwo");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "SideColourVariationOne");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "BackgroundColour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "SubColour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "MainColour");
            CreateIndex("dbo.ClinicalGuidelinesApp_Department", "ThemeId");
            AddForeignKey("dbo.ClinicalGuidelinesApp_Department", "ThemeId", "dbo.ClinicalGuidelinesApp_Theme", "Id", cascadeDelete: true);
        }
    }
}
