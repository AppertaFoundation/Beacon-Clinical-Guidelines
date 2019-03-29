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
    public class ArticleRevision
    {
        public int Id { get; set; }
        //Revision Viewable Properties
        [Index("ArticleId")]
        public int ArticleId { get; set; }
        [Index("SubSectionTabId")]
        public int? SubSectionTabId { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Title { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Structure { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Body { get; set; }
        //Revision Options
        [Index("Live")]
        public bool Live { get; set; }
        [Index("Approval")]
        public bool Approval { get; set; }
        public bool ShowGallery { get; set; }
        public string ApprovalComments { get; set; }
        public string ApprovalBy { get; set; }
        public bool Rejected { get; set; }
        [Index("ApprovalDateTime")]
        public DateTime? ApprovalDateTime { get; set; }
        [Index("Public")]
        public bool Public { get; set; }
        [Index("ReviewDateTime")]
        public DateTime? ReviewDateTime { get; set; }
        [Index("StartDateTime")]
        public DateTime? StartDateTime { get; set; }
        [Index("ExpiryDateTime")]
        public DateTime? ExpiryDateTime { get; set; }
        //Revision User Information
        public int UserId { get; set; }
        public int? Template { get; set; }
        public User RevisionAuthor { get; set; }
        public DateTime RevisionDateTime { get; set; }
        public List<Department> Departments { get; set; }
        public SubSectionTab SubSectionTab { get; set; }
        public List<Term> Terms { get; set; } 
        public string Files { get; set; } 
    }
}