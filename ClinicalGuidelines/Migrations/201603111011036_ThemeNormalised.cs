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
    
    public partial class ThemeNormalised : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_Theme",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
            
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeId", c => c.Int());
            CreateIndex("dbo.ClinicalGuidelinesApp_Department", "ThemeId");
            AddForeignKey("dbo.ClinicalGuidelinesApp_Department", "ThemeId", "dbo.ClinicalGuidelinesApp_Theme", "Id");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeMainColour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSubColour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeBackgroundColour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationOne");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationTwo");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationThree");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationFour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationFive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationFive", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationFour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationThree", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationTwo", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationOne", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeBackgroundColour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSubColour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeMainColour", c => c.String());
            DropForeignKey("dbo.ClinicalGuidelinesApp_Department", "ThemeId", "dbo.ClinicalGuidelinesApp_Theme");
            DropIndex("dbo.ClinicalGuidelinesApp_Department", new[] { "ThemeId" });
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeId");
            DropTable("dbo.ClinicalGuidelinesApp_Theme");
        }
    }
}
