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
    public class SearchResultDto
    {
        public bool CrossDepartmentSearch { get; set; }
        public List<ArticleRevisionDto> ArticleResults { get; set; }
        public List<UserDto> ContactResults { get; set; }
        public List<StaticPhoneNumberDto> StaticPhoneNumberResults { get; set; }
    }
}