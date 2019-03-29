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
    
    public partial class TemplateRefactoringStarted : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClinicalGuidelinesApp_TemplateItem", "TemplateId", "dbo.ClinicalGuidelinesApp_Template");
            DropIndex("dbo.ClinicalGuidelinesApp_TemplateItem", new[] { "TemplateId" });
            AddColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "Structure", c => c.String(unicode: false));
            AddColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "Template", c => c.Int(nullable: false));
            DropTable("dbo.ClinicalGuidelinesApp_TemplateItem");
            DropTable("dbo.ClinicalGuidelinesApp_Template");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ClinicalGuidelinesApp_Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Archived = c.Boolean(nullable: false),
                        ArchiveDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClinicalGuidelinesApp_TemplateItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateId = c.Int(nullable: false),
                        BlockType = c.Int(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        Alignment = c.Int(nullable: false),
                        Archived = c.Boolean(nullable: false),
                        ArchiveDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "Template");
            DropColumn("dbo.ClinicalGuidelinesApp_ArticleRevision", "Structure");
            CreateIndex("dbo.ClinicalGuidelinesApp_TemplateItem", "TemplateId");
            AddForeignKey("dbo.ClinicalGuidelinesApp_TemplateItem", "TemplateId", "dbo.ClinicalGuidelinesApp_Template", "Id", cascadeDelete: true);
        }
    }
}
