//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web.Http.Results;
using ClinicalGuidelines.Controllers;
using ClinicalGuidelines.Models;
using ClinicalGuidelinesTest.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClinicalGuidelinesTest.Controllers
{
    [TestClass]
    public class SubSectionTabApiControllerTest
    {
        private SubSectionTabApiController _controller;
        private readonly TestClinicalGuidelinesAppContext _context = new TestClinicalGuidelinesAppContext();

        private SubSectionTab _inMemorySubSectionTab;
        private SubSectionTab _inMemoryGlobalSubSectionTab;
        private Department _inMemoryDepartment;
        private User _inMemoryUser;

        private const int SubSectionTabId = 1;
        private const int GlobalSubSectionTabId = 2;
        private const int DepartmentId = 3;
        private const int UserId = 4;

        private const string AdminUser = "AdminTest";

        private void GetSubSectionTabApiController()
        {
            _controller = new SubSectionTabApiController(_context);
        }

        private SubSectionTab GetDemoSubSectionTab()
        {
            return new SubSectionTab()
            {
                Id = SubSectionTabId,
                _id = new Guid("46009b5b-d5b5-47e2-94eb-285d022b0cf7"),
                DepartmentId = DepartmentId,
                Name = "Test Subsection",
                Icon = "Test-Icon.png",
                Live = true
            };
        }

        private Department GetDemoDepartment()
        {
            return new Department()
            {
                Id = DepartmentId,
                _id = new Guid("51d895f8-29d0-49c1-878e-8972095be5c3"),
                Name = "Test Department",
                ShortName = "TEST",
                MainColour = "#fff",
                SubColour = "#ddd",
                BackgroundColour = "#eee",
                SideColourVariationOne = "#111",
                SideColourVariationTwo = "#222",
                SideColourVariationThree = "#333",
                SideColourVariationFour = "#444",
                SideColourVariationFive = "#555",
                Archived = false,
                ArchiveDateTime = null,
                Live = true
            };
        }
        private User GetDemoUser()
        {
            return new User()
            {
                Id = UserId,
                _id = new Guid("75600435-14c0-43e0-bcde-1dc27f75012b"),
                UserName = AdminUser,
                Archived = false,
                Contact = true,
                LeadClinician = true,
                SiteAdmin = true,
                ContentEditor = true
            };
        }

        private void AddDemoToInMemoryContext()
        {
            _inMemorySubSectionTab = GetDemoSubSectionTab();
            _inMemoryGlobalSubSectionTab = GetDemoSubSectionTab();
            _inMemoryGlobalSubSectionTab.Id = GlobalSubSectionTabId;
            _inMemoryGlobalSubSectionTab.DepartmentId = null;
            _inMemoryDepartment = GetDemoDepartment();
            _inMemoryUser = GetDemoUser();
            _context.SubSectionTabs.Add(_inMemorySubSectionTab);
            _context.SubSectionTabs.Add(_inMemoryGlobalSubSectionTab);
            _context.Departments.Add(_inMemoryDepartment);
            _context.Users.Add(_inMemoryUser);
        }

        private void CheckSubSectionTab(SubSectionTab subSectionTab, SubSectionTab checkSubSectionTab)
        {
            Assert.AreEqual(checkSubSectionTab.Id, subSectionTab.Id);
            Assert.AreEqual(checkSubSectionTab._id, subSectionTab._id);
            Assert.AreEqual(checkSubSectionTab.DepartmentId, subSectionTab.DepartmentId);
            Assert.AreEqual(checkSubSectionTab.Name, subSectionTab.Name);
            Assert.AreEqual(checkSubSectionTab.Icon, subSectionTab.Icon);
        }

        [TestInitialize]
        public void SetUp()
        {
            GetSubSectionTabApiController();
            AddDemoToInMemoryContext();
            var identity = new GenericIdentity(AdminUser);
            Thread.CurrentPrincipal = new GenericPrincipal(identity, null);
        }

        [TestMethod]
        public void TestGetSubSectionTabs()
        {
            var subSectionTabs = _controller.GetSubSectionTabs();

            Assert.IsNotNull(subSectionTabs);
            Assert.AreEqual(2, subSectionTabs.Count());

            var subSectionTabsArray = subSectionTabs.ToArray();
            CheckSubSectionTab(subSectionTabsArray[0], _inMemorySubSectionTab);
            CheckSubSectionTab(subSectionTabsArray[1], _inMemoryGlobalSubSectionTab);
        }

        [TestMethod]
        public void TestGetGlobalSubSectionTabs()
        {
            var subSectionTabs = _controller.GetGlobalSubSectionTabs() as TestDbAsyncEnumerable<SubSectionTab>;

            Assert.IsInstanceOfType(subSectionTabs, typeof(TestDbAsyncEnumerable<SubSectionTab>));
            Assert.IsNotNull(subSectionTabs);

            Assert.AreEqual(1, subSectionTabs.Count());

            foreach (var subSectionTab in subSectionTabs)
            {
                CheckSubSectionTab(subSectionTab, _inMemoryGlobalSubSectionTab);
            }
        }

        [TestMethod]
        public void TestGetSubSectionTabIncorrectId()
        {
            var articleRevisions = _controller.GetSubSectionTab(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetSubSectionTabCorrectId()
        {
            var results = _controller.GetSubSectionTab(SubSectionTabId) as OkNegotiatedContentResult<SubSectionTab>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<SubSectionTab>));
            Assert.IsNotNull(results);

            CheckSubSectionTab(results.Content, _inMemorySubSectionTab);
        }

        [TestMethod]
        public void TestGetSubSectionTabsForDepartmentIncorrectId()
        {
            var articleRevisions = _controller.GetSubSectionTabsForDepartment(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }
        
        [TestMethod]
        public void TestPutSubSectionTabIncorrectId()
        {
            var results = _controller.PutSubSectionTab(999, GetDemoSubSectionTab()) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestPutSubSectionTabCorrectId()
        {
            var subSectionTab = GetDemoSubSectionTab();
            var newName = "New Name";
            subSectionTab.Name = newName;

            Assert.AreEqual(2, _context.SubSectionTabs.Count());

            var results = _controller.PutSubSectionTab(SubSectionTabId, subSectionTab) as StatusCodeResult;

            Assert.IsInstanceOfType(results, typeof(StatusCodeResult));
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.NoContent, results.StatusCode);
            Assert.AreEqual(2, _context.SubSectionTabs.Count());
        }

        [TestMethod]
        public void TestPostSubSectionTab()
        {
            Assert.AreEqual(2, _context.SubSectionTabs.Count());

            var result = _controller.PostSubSectionTab(GetDemoSubSectionTab()) as OkNegotiatedContentResult<SubSectionTab>;

            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<SubSectionTab>));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Content, typeof(SubSectionTab));

            Assert.AreEqual(3, _context.SubSectionTabs.Count());
        }

        [TestMethod]
        public void TestDeleteSubSectionTabIncorrectId()
        {
            var result = _controller.PutArchiveSubSectionTab(999) as NotFoundResult;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestDeleteSubSectionTabCorrectId()
        {
            var result = _controller.PutArchiveSubSectionTab(SubSectionTabId) as StatusCodeResult;

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(0, _context.ArticleRevisions.Count());
        }
    }
}
