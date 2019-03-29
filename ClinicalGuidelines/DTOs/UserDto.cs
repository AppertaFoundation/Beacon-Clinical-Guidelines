//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public Guid _id { get; set; }
        public string DisplayName { get; set; }
        public string SamAccountName { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool SiteAdmin { get; set; }
        public bool LeadClinician { get; set; }
        public bool ContentApprover { get; set; }
        public bool ContentEditor { get; set; }
        public bool Contact { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DepartmentDto Department { get; set; }
        public ICollection<ArticleRevisionDto> FavouriteArticles { get; set; }
        public IEnumerable<DepartmentDto> AdministationDepartments { get; set; }
        public Server CurrentServer { get; set; }
    }
}