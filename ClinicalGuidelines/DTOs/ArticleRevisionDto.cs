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
    public class ArticleRevisionDto
    {
        public int Id { get; set; }
        public Guid _id { get; set; }
        public int RevisionId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Body { get; set; }
        public object Structure { get; set; }
        public DateTime? RevisionDateTime { get; set; }
        public DateTime? ExpiryDateTime { get; set; }
        public int? Viewed { get; set; }
        public int UserId { get; set; }
        public int? Template { get; set; }
        public List<DepartmentDto> Departments { get; set; }
        public int? SubSectionTabId { get; set; }
        public List<TermDto> Terms { get; set; } 
        public List<RelatedArticlesDto> RelatedArticleDtos { get; set; }
        public object Files { get; set; }
        public bool Live { get; set; }
        public bool ShowGallery { get; set; }
        public bool Approval { get; set; }
        public bool Rejected { get; set; }
        public string ApprovalComments { get; set; }
        public bool Archived { get; set; }
    }
}