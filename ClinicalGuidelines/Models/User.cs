//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalGuidelines.Models
{
    
    public class User
    {
        public int Id { get; set; }
        [Index("PouchDb_id")]
        public Guid _id { get; set; }
        [StringLength(25)]
        [Index("Username", IsUnique = true)]
        public string UserName { get; set; }
        [StringLength(25)]
        public string Forename { get; set; }
        [StringLength(25)]
        public string Surname { get; set; }
        [StringLength(100)]
        public string JobTitle { get; set; }
        [StringLength(100)]
        public string EmailAddress { get; set; }
        [StringLength(12)]
        public string PhoneNumber { get; set; }
        public bool SiteAdmin { get; set; }
        public bool LeadClinician { get; set; }
        public bool ContentApprover { get; set; }
        public bool ContentEditor { get; set; }
        public bool Contact { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Department> AdministationDepartments { get; set; }
        public ICollection<Department> AppDepartments { get; set; }
        public ICollection<ArticleRevision> FavouriteArticles { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchiveDateTime { get; set; }
    }
}