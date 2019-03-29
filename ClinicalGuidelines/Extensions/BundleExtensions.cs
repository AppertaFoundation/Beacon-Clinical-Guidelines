//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Web.Optimization;

namespace ClinicalGuidelines.Extensions
{
    internal static class BundleExtensions
    {
        public static Bundle WithLastVersionNumber(this Bundle sb)
        {
            sb.Transforms.Add(new VersionNumberBundleTransform());
            return sb;
        }

        public class VersionNumberBundleTransform : IBundleTransform
        {
            public void Process(BundleContext context, BundleResponse response)
            {
                foreach (var file in response.Files)
                {
                    file.IncludedVirtualPath = string.Concat(file.IncludedVirtualPath, "?v=1.3.6");
                }
            }
        }
    }
}