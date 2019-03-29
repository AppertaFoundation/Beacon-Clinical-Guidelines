//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;

namespace ClinicalGuidelines.DTOs
{
    public class RelatedArticlesDto
    {
        public int Id { get; set; }
        public Guid _id { get; set; }
        public List<DepartmentDto> Departments { get; set; }
        public int SubSectionTabId { get; set; }
        public string Name { get; set; }
        public TermDto Term { get; set; }
    }
}