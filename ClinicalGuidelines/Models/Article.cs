//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalGuidelines.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Index("PouchDb_id")]
        public Guid _id { get; set; }
        public int ArticleRevisionId { get; set; }
        [DefaultValue(0)]
        public int HitCount { get; set; }
        public ArticleRevision CurrentArticleRevision { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchiveDateTime { get; set; }
    }
}