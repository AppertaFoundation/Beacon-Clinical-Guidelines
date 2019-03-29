//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Linq;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelinesTest.TestDoubles
{
    class TestArticleDbSet : TestDbSet<Article>
    {
        public override Article Find(params object[] keyValues)
        {
            return this.SingleOrDefault(article => article.Id == (int)keyValues.Single());
        }
    }
}
