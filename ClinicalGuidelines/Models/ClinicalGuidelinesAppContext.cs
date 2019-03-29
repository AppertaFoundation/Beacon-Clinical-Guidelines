//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Data.Entity;

namespace ClinicalGuidelines.Models
{
    public class ClinicalGuidelinesAppContext : DbContext, IClinicalGuidelinesAppContext
    {
        public ClinicalGuidelinesAppContext() : base("name=ClinicalGuidelinesAppContext")
        {
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Types().Configure(entity => entity.ToTable("ClinicalGuidelinesApp_" + entity.ClrType.Name));

            //The table naming doesn't follow through with EF created tables so we don't let EF create the table itself and do it manually
            //1. Relationship between article revisions and taxonomy terms
            modelBuilder.Entity<ArticleRevision>()
                .HasMany(x => x.Terms)
                .WithMany(x => x.ArticleRevisions)
                .Map(
                    x => {
                        x.MapLeftKey("ArticleRevisionId");
                        x.MapRightKey("TermId");
                        x.ToTable("ClinicalGuidelinesApp_ArticleRevisionTerms");
                    }
                );

            modelBuilder.Entity<ArticleRevision>()
                .HasMany(x => x.Departments)
                .WithMany(x => x.ArticleRevisions)
                .Map(
                    x => {
                        x.MapLeftKey("ArticleRevisionId");
                        x.MapRightKey("DepartmentId");
                        x.ToTable("ClinicalGuidelinesApp_ArticleRevisionDepartments");
                    }
                );

            modelBuilder.Entity<User>()
                .HasMany(x => x.AppDepartments)
                .WithMany(x => x.Users)
                .Map(
                    x => {
                        x.MapLeftKey("UserId");
                        x.MapRightKey("DepartmentId");
                        x.ToTable("ClinicalGuidelinesApp_UserDepartments");
                    }
                );
            
            modelBuilder.Entity<User>()
                .HasMany(x => x.AdministationDepartments)
                .WithMany(x => x.AdministationUsers)
                .Map(
                    x => {
                        x.MapLeftKey("UserId");
                        x.MapRightKey("DepartmentId");
                        x.ToTable("ClinicalGuidelinesApp_UserAdministrationDepartments");
                    }
                );

            modelBuilder.Entity<SubSectionTab>()
                .HasMany(x => x.Departments)
                .WithMany(x => x.SubSectionTabs)
                .Map(
                    x => {
                        x.MapLeftKey("SubSectionTabId");
                        x.MapRightKey("DepartmentId");
                        x.ToTable("ClinicalGuidelinesApp_SubSectionTabDepartments");
                    }
                );
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StaticPhoneNumber> StaticPhoneNumbers { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleRevision> ArticleRevisions { get; set; }
        public DbSet<ArticleStructureTemplate> ArticleStructureTemplates { get; set; }
        public DbSet<SubSectionTab> SubSectionTabs { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<Log> Logs { get; set; }

        public void MarkDepartmentAsModified(Department department)
        {
            Entry(department).State = EntityState.Modified;
        }
        public void MarkStaticPhoneNumberAsModified(StaticPhoneNumber staticPhoneNumber)
        {
            Entry(staticPhoneNumber).State = EntityState.Modified;
        }
        public void MarkUserAsModified(User user)
        {
            Entry(user).State = EntityState.Modified;
        }
        public void MarkArticleAsModified(Article article)
        {
            Entry(article).State = EntityState.Modified;
        }
        public void MarkArticleRevisionAsModified(ArticleRevision articleRevision)
        {
            Entry(articleRevision).State = EntityState.Modified;
        }
        public void MarkArticleStructureTemplateAsModified(ArticleStructureTemplate articleStructureTemplate)
        {
            Entry(articleStructureTemplate).State = EntityState.Modified;
        }
        public void MarkSubSectionTabAsModified(SubSectionTab subSectionTab)
        {
            Entry(subSectionTab).State = EntityState.Modified;
        }
        public void MarkTermAsModified(Term term)
        {
            Entry(term).State = EntityState.Modified;
        }
        public void MarkLogAsModified(Log log)
        {
            Entry(log).State = EntityState.Modified;
        }
    }
}