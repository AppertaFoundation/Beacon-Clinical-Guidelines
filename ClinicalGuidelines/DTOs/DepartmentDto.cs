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
    public class DepartmentDto
    {
        public int Id { get; set; }
        public Guid _id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool Archived { get; set; }
        public bool Live { get; set; }
        public bool Container { get; set; }
        public int? ContainerId { get; set; }
        public string MainColour { get; set; }
        public string SubColour { get; set; }
        public string BackgroundColour { get; set; }
        public string SideColourVariationOne { get; set; }
        public string SideColourVariationTwo { get; set; }
        public string SideColourVariationThree { get; set; }
        public string SideColourVariationFour { get; set; }
        public string SideColourVariationFive { get; set; }
        public List<ArticleRevisionDto> TopArticles { get; set; }
        public List<UserDto> KeyContacts { get; set; }
        public List<SubSectionTabDto> SubSectionTabs { get; set; }
    }
}