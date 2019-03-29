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
    public class SubSectionTab
    {
        public int Id { get; set; }
        [Index("PouchDb_id")]
        public Guid _id { get; set; }
        public int? DepartmentId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Live { get; set; }
        public Department Department { get; set; }
        public List<Department> Departments { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchiveDateTime { get; set; }
    }
}