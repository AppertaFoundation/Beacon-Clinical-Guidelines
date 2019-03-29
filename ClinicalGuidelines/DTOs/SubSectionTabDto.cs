//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using ClinicalGuidelines.Models;
using System;

namespace ClinicalGuidelines.DTOs
{
    public class SubSectionTabDto
    {
        public int Id { get; set; }
        public Guid _id { get; set; }
        public int? DepartmentId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Live { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchiveDateTime { get; set; }
        public Department Department { get; set; }
    }
}