//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Web.Http;

namespace ClinicalGuidelines.Controllers
{
    [RoutePrefix("api/alive")]
    public class AliveApiController : ApiController
    {
        [Route("ping")]
        public IHttpActionResult GetAlive()
        {
            return Json("achu");
        }
    }
}