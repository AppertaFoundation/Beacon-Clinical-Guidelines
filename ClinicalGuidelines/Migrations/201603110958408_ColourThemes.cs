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
    
    public partial class ColourThemes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeMainColour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSubColour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeBackgroundColour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationOne", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationTwo", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationThree", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationFour", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationFive", c => c.String());
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "Hue");
            DropColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "Hue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClinicalGuidelinesApp_SubSectionTab", "Hue", c => c.String());
            AddColumn("dbo.ClinicalGuidelinesApp_Department", "Hue", c => c.String());
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationFive");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationFour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationThree");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationTwo");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSideColourVariationOne");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeBackgroundColour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeSubColour");
            DropColumn("dbo.ClinicalGuidelinesApp_Department", "ThemeMainColour");
        }
    }
}
