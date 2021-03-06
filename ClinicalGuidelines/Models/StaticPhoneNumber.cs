//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalGuidelines.Models
{
    public class StaticPhoneNumber
    {
        public int Id { get; set; }
        [Index("PouchDb_id")]
        public Guid _id { get; set; }
        public StaticPhoneNumberType Type { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}