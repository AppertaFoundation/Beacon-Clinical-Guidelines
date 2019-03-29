//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;

namespace ClinicalGuidelines.DTOs
{
    public class DepartmentTotalsDto
    {
        public int OverallTotal { get; set; }
        public int ContactsTotal { get; set; }
        public int SubSectionTabsTotal { get; set; }
        public List<DepartmentSubSectionTotalsDto> SubSections { get; set; } 
    }
}