//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿namespace ClinicalGuidelines.Models
{
    public enum FileType
    {
        Image,
        Document
    }

    public enum Server
    {
        Debug,
        Test,
        UAT,
        Live,
        NotRecognised
    }

    public enum StaticPhoneNumberType
    {
        Bleep,
        Landline,
        Mobile,
        Extension
    }
}