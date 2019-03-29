//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Data.Entity;
using ClinicalGuidelines.Controllers;

namespace ClinicalGuidelines.Models
{
    public interface IClinicalGuidelinesAppContext : IDisposable
    {
        DbSet<Department> Departments { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<StaticPhoneNumber> StaticPhoneNumbers { get; set; }
        DbSet<Article> Articles { get; set; }
        DbSet<ArticleRevision> ArticleRevisions { get; set; }
        DbSet<ArticleStructureTemplate> ArticleStructureTemplates { get; set; }
        DbSet<SubSectionTab> SubSectionTabs { get; set; }
        DbSet<Term> Terms { get; set; }
        DbSet<Log> Logs { get; set; }

        int SaveChanges();
        void MarkDepartmentAsModified(Department department);
        void MarkUserAsModified(User user);
        void MarkStaticPhoneNumberAsModified(StaticPhoneNumber staticPhoneNumber);
        void MarkArticleAsModified(Article article);
        void MarkArticleRevisionAsModified(ArticleRevision articleRevision);
        void MarkArticleStructureTemplateAsModified(ArticleStructureTemplate articleStructureTemplate);
        void MarkSubSectionTabAsModified(SubSectionTab subSectionTab);
        void MarkTermAsModified(Term term);
        void MarkLogAsModified(Log log);
    }
}
