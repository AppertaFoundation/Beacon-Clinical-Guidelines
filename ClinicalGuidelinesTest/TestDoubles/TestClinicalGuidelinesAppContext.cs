//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Data.Entity;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelinesTest.TestDoubles
{
    public class TestClinicalGuidelinesAppContext : IClinicalGuidelinesAppContext
    {
        public TestClinicalGuidelinesAppContext()
        {
            Departments = new TestDepartmentDbSet();
            Users = new TestUserDbSet();
            StaticPhoneNumbers = new TestStaticPhoneNumberDbSet();
            Articles = new TestArticleDbSet();
            ArticleRevisions = new TestArticleRevisionDbSet();
            ArticleStructureTemplates = new TestArticleStructureTemplateDbSet();
            SubSectionTabs = new TestSubSectionTabDbSet();
            Terms = new TestTermDbSet();
            Logs = new TestLogDbSet();
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

        public int SaveChanges()
        {
            return 0;
        }

        public void MarkDepartmentAsModified(Department department) { }
        public void MarkUserAsModified(User user) { }
        public void MarkStaticPhoneNumberAsModified(StaticPhoneNumber staticPhoneNumber) { }
        public void MarkArticleAsModified(Article article) { }
        public void MarkArticleRevisionAsModified(ArticleRevision articleRevision) { }
        public void MarkArticleStructureTemplateAsModified(ArticleStructureTemplate articleStructureTemplate) { }
        public void MarkSubSectionTabAsModified(SubSectionTab subSectionTab) { }
        public void MarkTermAsModified(Term term) { }
        public void MarkLogAsModified(Log log) { }

        public void Dispose() { }
    }
}
