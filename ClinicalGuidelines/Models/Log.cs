//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalGuidelines.Models
{
    public class Log
    {
        public int Id { get; set; }
        [Index("MethodCalled")]
        [StringLength(255)]
        public string MethodCalled { get; set; }
        [StringLength(255)]
        public string RequestUri { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Response { get; set; }
        [StringLength(50)]
        public string SamAccountName { get; set; }
        [Index("DateTimeCalled")]
        public DateTime DateTimeCalled { get; set; }
    }
}