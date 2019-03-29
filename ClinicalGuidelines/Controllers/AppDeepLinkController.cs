//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.Controllers
{
    public class AppDeepLinkController : Controller
    {
        private readonly IClinicalGuidelinesAppContext _db = new ClinicalGuidelinesAppContext();
        private static readonly string[] SupportedMobileDevices = { "iphone", "android"};

        private static bool IsSupportedMobileDevice(string userAgent)
        {
            if (userAgent.IsNullOrWhiteSpace()) return false;

            userAgent = userAgent.ToLower();
            return SupportedMobileDevices.Any(x => userAgent.Contains(x));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GoToAppArticle(int articleId, int departmentId, int subSectionTabId)
        {
            var department = _db.Departments.SingleOrDefault(x => x.Id == departmentId);

            if (department == null) return null;

            return IsSupportedMobileDevice(Request.UserAgent) 
                ? new RedirectResult($"beacon://ionic.agiledev.beacon/loadArticle=true&articleId={articleId}&departmentId={departmentId}&subSectionTabId={subSectionTabId}") 
                : new RedirectResult($"/#/?containerId={department.ContainerId}&departmentId={departmentId}&subSectionTabId={subSectionTabId}&articleId={articleId}");
        }
    }
}