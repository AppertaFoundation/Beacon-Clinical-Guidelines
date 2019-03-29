//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Web.Http.Results;
using ClinicalGuidelines.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClinicalGuidelinesTest.Controllers
{
    [TestClass]
    public class AliveApiControllerTest
    {
        private AliveApiController _controller;

        private void GetAliveApiController()
        {
            _controller = new AliveApiController();
        }

        [TestInitialize]
        public void SetUp()
        {
            GetAliveApiController();
        }

        [TestMethod]
        public void TestGetAlive()
        {
            var result = _controller.GetAlive() as JsonResult<string>;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JsonResult<string>));
            Assert.AreEqual("achu", result.Content);
        }
    }
}
