//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;

namespace ClinicalGuidelines.DTOs
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public Guid _id { get; set; }
        public int RevisionId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
    }
}