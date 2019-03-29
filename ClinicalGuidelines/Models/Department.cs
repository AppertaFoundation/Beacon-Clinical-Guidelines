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
    public class Department
    {
        public int Id { get; set; }
        [Index("PouchDb_id")]
        public Guid _id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(50)]
        public string ShortName { get; set; }
        public string MainColour { get; set; }
        public string SubColour { get; set; }
        public string BackgroundColour { get; set; }
        public string SideColourVariationOne { get; set; }
        public string SideColourVariationTwo { get; set; }
        public string SideColourVariationThree { get; set; }
        public string SideColourVariationFour { get; set; }
        public string SideColourVariationFive { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<User> AdministationUsers { get; set; }
        public ICollection<SubSectionTab> SubSectionTabs { get; set; }
        public ICollection<ArticleRevision> ArticleRevisions { get; set; }
        public bool Archived { get; set; }
        public bool Live { get; set; }
        public bool Container { get; set; }
        public int? ContainerId { get; set; }
        public DateTime? ArchiveDateTime { get; set; }
    }
}