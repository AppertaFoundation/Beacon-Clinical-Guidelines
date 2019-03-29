//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.DTOs
{
    public class FileDto
    {
        public Guid Id { get; set; }
        public FileType Type { get; set; }
        public string Host { get; set; }
        public string Extension { get; set; }
        public string UploadedBy { get; set; }
        public string OriginalFileName { get; set; }
        public double OriginalFileSize { get; set; }
    }
}