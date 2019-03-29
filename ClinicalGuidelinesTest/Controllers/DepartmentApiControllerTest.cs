//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web.Http.Results;
using ClinicalGuidelines.Controllers;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Models;
using ClinicalGuidelinesTest.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ClinicalGuidelinesTest.Controllers
{
    [TestClass]
    public class DepartmentApiControllerTest
    {
        private DepartmentApiController _controller;
        private readonly TestClinicalGuidelinesAppContext _context = new TestClinicalGuidelinesAppContext();
        private readonly TestMappingHelper _mappingHelper = new TestMappingHelper();

        private Department _inMemoryDepartment;
        private User _inMemoryUser;
        private SubSectionTab _inMemorySubSectionTab;
        private Article _inMemoryArticle;
        private ArticleRevision _inMemoryArticleRevision;
        private Term _inMemoryTerm;

        private const int DepartmentId = 1;
        private const int UserId = 2;
        private const int SubSectionTabId = 3;
        private const int ArticleId = 4;
        private const int ArticleRevisionId = 5;
        private const int TermId = 6;

        private const string AdminUser = "AdminTest";

        private readonly DateTime _testDateTime = Convert.ToDateTime("2016-01-01 00:00:00");

        private void GetDepartmentApiController()
        {
            _controller = new DepartmentApiController(_context, _mappingHelper);
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

        private List<Department> GetDemoDepartmentAsList()
        {
            return new List<Department> { GetDemoDepartment() }; ;
        }

        private User GetDemoUser()
        {
            return new User()
            {
                Id = UserId,
                _id = new Guid("6a568a7c-f711-4f6b-a861-5200bd8f1a34"),
                UserName = AdminUser,
                Forename = "Test",
                Surname = "User",
                JobTitle = "Tester",
                EmailAddress = "test@testing.nhs.uk",
                PhoneNumber = "1234",
                SiteAdmin = true,
                LeadClinician = true,
                ContentEditor = true,
                Contact = true,
                DepartmentId = DepartmentId,
                Department = GetDemoDepartment(),
                Archived = false,
                ArchiveDateTime = null
            };
        }

        private SubSectionTab GetDemoSubSectionTab()
        {
            return new SubSectionTab()
            {
                Id = SubSectionTabId,
                _id = new Guid("46009b5b-d5b5-47e2-94eb-285d022b0cf7"),
                DepartmentId = DepartmentId,
                Department = GetDemoDepartment(),
                Name = "Test Subsection",
                Icon = "Test-Icon.png",
                Live = true
            };
        }

        private Article GetDemoArticle()
        {
            return new Article()
            {
                Id = ArticleId,
                _id = new Guid("d58a8b27-b73b-46f5-aa5d-e062eba10a84"),
                ArticleRevisionId = ArticleRevisionId,
                HitCount = 2,
                CurrentArticleRevision = GetDemoArticleRevision()
            };
        }

        private ArticleRevision GetDemoArticleRevision()
        {
            return new ArticleRevision()
            {
                Id = ArticleRevisionId,
                ArticleId = ArticleId,
                SubSectionTabId = SubSectionTabId,
                Title = "Test Article",
                Structure = JsonConvert.SerializeObject("Test Structure"),
                Body = "Test Body",
                Live = true,
                Public = false,
                ReviewDateTime = null,
                StartDateTime = null,
                ExpiryDateTime = null,
                UserId = 1,
                Template = 1,
                RevisionDateTime = _testDateTime,
                Departments = GetDemoDepartmentAsList(),
                SubSectionTab = GetDemoSubSectionTab(),
                Terms = GetDemoTerms(),
                Files = JsonConvert.SerializeObject("Test Files")
            };
        }

        private Term GetDemoTerm()
        {
            return new Term()
            {
                Id = TermId,
                Name = "Test Term"
            };
        }

        private List<Term> GetDemoTerms()
        {
            return new List<Term>()
            {
                GetDemoTerm()
            };
        }

        private void AddDemoToInMemoryContext()
        {
            _inMemoryDepartment = GetDemoDepartment();
            _inMemoryUser = GetDemoUser();
            _inMemorySubSectionTab = GetDemoSubSectionTab();
            _inMemoryArticle = GetDemoArticle();
            _inMemoryArticleRevision = GetDemoArticleRevision();
            _inMemoryTerm = GetDemoTerm();
            _context.Departments.Add(_inMemoryDepartment);
            _context.Users.Add(_inMemoryUser);
            _context.SubSectionTabs.Add(_inMemorySubSectionTab);
            _context.Articles.Add(_inMemoryArticle);
            _context.ArticleRevisions.Add(_inMemoryArticleRevision);
            _context.Terms.Add(_inMemoryTerm);
        }

        private void CheckDepartment(Department department)
        {
            var demoDepartment = GetDemoDepartment();

            Assert.AreEqual(demoDepartment.Id, department.Id);
            Assert.AreEqual(demoDepartment._id, department._id);
            Assert.AreEqual(demoDepartment.Name, department.Name);
            Assert.AreEqual(demoDepartment.ShortName, department.ShortName);
            Assert.AreEqual(demoDepartment.MainColour, department.MainColour);
            Assert.AreEqual(demoDepartment.SubColour, department.SubColour);
            Assert.AreEqual(demoDepartment.BackgroundColour, department.BackgroundColour);
            Assert.AreEqual(demoDepartment.SideColourVariationOne, department.SideColourVariationOne);
            Assert.AreEqual(demoDepartment.SideColourVariationTwo, department.SideColourVariationTwo);
            Assert.AreEqual(demoDepartment.SideColourVariationThree, department.SideColourVariationThree);
            Assert.AreEqual(demoDepartment.SideColourVariationFour, department.SideColourVariationFour);
            Assert.AreEqual(demoDepartment.SideColourVariationFive, department.SideColourVariationFive);
            Assert.AreEqual(demoDepartment.Archived, department.Archived);
            Assert.AreEqual(demoDepartment.ArchiveDateTime, department.ArchiveDateTime);
        }

        [TestInitialize]
        public void SetUp()
        {
            GetDepartmentApiController();
            AddDemoToInMemoryContext();
            var identity = new GenericIdentity(AdminUser);
            Thread.CurrentPrincipal = new GenericPrincipal(identity, null);
        }

        [TestMethod]
        public void TestGetDepartments()
        {
            var results = _controller.GetDepartments() as OkNegotiatedContentResult<List<Department>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<Department>>));
            Assert.IsNotNull(results);

            var departments = results.Content;
            Assert.AreEqual(1, departments.Count());
            foreach (var department in departments)
            {
                CheckDepartment(department);
            }

        }

        [TestMethod]
        public void TestGetDepartmentIncorrectId()
        {
            var articleRevisions = _controller.GetDepartment(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentCorrectId()
        {
            var results = _controller.GetDepartment(DepartmentId) as OkNegotiatedContentResult<Department>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<Department>));
            Assert.IsNotNull(results);

            CheckDepartment(results.Content);
        }

        [TestMethod]
        public void TestGetDepartmentTotalsIncorrectId()
        {
            var articleRevisions = _controller.GetDepartmentTotals(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentTotalsCorrectId()
        {
            var results = _controller.GetDepartmentTotals(DepartmentId) as OkNegotiatedContentResult<DepartmentTotalsDto>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<DepartmentTotalsDto>));
            Assert.IsNotNull(results);

            var totals = results.Content;

            Assert.AreEqual(1, totals.ContactsTotal);
            Assert.AreEqual(1, totals.SubSectionTabsTotal);

            foreach (var subSection in totals.SubSections)
            {
                Assert.AreEqual(1, subSection.ArticlesBodyTotal);
                Assert.AreEqual(1, subSection.ArticlesTotal);
            }

            Assert.AreEqual(4, totals.OverallTotal);
        }

        [TestMethod]
        public void TestGetDepartmentWithSearchTermInDataset()
        {
            var searchTerm = "Test";
            var results = _controller.GetDepartmentsFromSearchTerm(searchTerm) as OkNegotiatedContentResult<IQueryable<Department>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<IQueryable<Department>>));
            Assert.IsNotNull(results);

            var departments = results.Content;

            Assert.AreEqual(1, departments.Count());
        }

        [TestMethod]
        public void TestGetDepartmentWithSearchTermOutsideDataset()
        {
            var searchTerm = "Not there";
            var results = _controller.GetDepartmentsFromSearchTerm(searchTerm) as OkNegotiatedContentResult<IQueryable<Department>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<IQueryable<Department>>));
            Assert.IsNotNull(results);

            var departments = results.Content;

            Assert.AreEqual(0, departments.Count());
        }

        [TestMethod]
        public void TestGetDepartmentWithSearchTermToShort()
        {
            var searchTerm = "se";
            var results = _controller.GetDepartmentsFromSearchTerm(searchTerm) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetDepartmentWithSearchTermNull()
        {
            var results = _controller.GetDepartmentsFromSearchTerm(null) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesIncorrectId()
        {
            var articleRevisions = _controller.GetDepartmentArticles(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesCorrectId()
        {
            var results = _controller.GetDepartmentArticles(DepartmentId) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListIncorrectId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesList(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListCorrectId()
        {
            var results = _controller.GetDepartmentArticlesList(DepartmentId) as OkNegotiatedContentResult<List<ArticleDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesBySubSectionIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesBySubSectionTabId(999, SubSectionTabId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesBySubSectionIdIncorrectSubSectionTabId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesBySubSectionTabId(DepartmentId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesBySubSectionIdCorrectIds()
        {
            var results = _controller.GetDepartmentArticlesBySubSectionTabId(DepartmentId, SubSectionTabId) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetAllDepartmentArticlesBySubSectionIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetAllDepartmentArticlesBySubSectionTabId(999, SubSectionTabId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetAllDepartmentArticlesBySubSectionIdIncorrectSubSectionTabId()
        {
            var articleRevisions = _controller.GetAllDepartmentArticlesBySubSectionTabId(DepartmentId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetAllDepartmentArticlesBySubSectionIdCorrectIds()
        {
            var results = _controller.GetAllDepartmentArticlesBySubSectionTabId(DepartmentId, SubSectionTabId) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListBySubSectionIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesListBySubSectionTabId(999, SubSectionTabId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListBySubSectionIdIncorrectSubSectionTabId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesListBySubSectionTabId(DepartmentId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListBySubSectionIdCorrectIds()
        {
            var results = _controller.GetDepartmentArticlesListBySubSectionTabId(DepartmentId, SubSectionTabId) as OkNegotiatedContentResult<List<ArticleDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetDepartmentAllArticlesListBySubSectionTabIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetDepartmentAllArticlesListBySubSectionTabId(999, SubSectionTabId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentAllArticlesListBySubSectionTabIdIncorrectSubSectionTabId()
        {
            var articleRevisions = _controller.GetDepartmentAllArticlesListBySubSectionTabId(DepartmentId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentAllArticlesListBySubSectionTabIdCorrectIds()
        {
            var results = _controller.GetDepartmentAllArticlesListBySubSectionTabId(DepartmentId, SubSectionTabId) as OkNegotiatedContentResult<List<ArticleDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromArticleIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetDepartmentSubSectionArticlesFromArticleId(999, SubSectionTabId, ArticleId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromArticleIdIncorrectSubSectionTabId()
        {
            var articleRevisions = _controller.GetDepartmentSubSectionArticlesFromArticleId(DepartmentId, 999, ArticleId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromArticleIdIncorrectArticleId()
        {
            var articleRevisions = _controller.GetDepartmentSubSectionArticlesFromArticleId(DepartmentId, SubSectionTabId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromArticleId()
        {
            //First add a new article to the context with a later revision date so it will be returned by the function 
            var laterArticle = GetDemoArticle();
            var newArticleId = 999;
            var newTitle = "Another Article";
            var newRevisionDateTime = _testDateTime.AddDays(-2);

            laterArticle.Id = newArticleId;
            laterArticle.CurrentArticleRevision.Title = newTitle;
            laterArticle.CurrentArticleRevision.RevisionDateTime = newRevisionDateTime;
            _context.Articles.Add(laterArticle);

            var results = _controller.GetDepartmentSubSectionArticlesFromArticleId(DepartmentId, SubSectionTabId, ArticleId) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(2, _context.Articles.Count());
            Assert.AreEqual(newTitle, _context.Articles.Find(newArticleId).CurrentArticleRevision.Title);
            Assert.AreEqual(newRevisionDateTime, _context.Articles.Find(newArticleId).CurrentArticleRevision.RevisionDateTime);
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromArticleIdNoLaterArticles()
        {
            var results = _controller.GetDepartmentSubSectionArticlesFromArticleId(DepartmentId, SubSectionTabId, ArticleId) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(0, results.Content.Count);
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesListFromArticleIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetDepartmentSubSectionArticlesListFromArticleId(999, SubSectionTabId, ArticleId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesListFromArticleIdIncorrectSubSectionTabId()
        {
            var articleRevisions = _controller.GetDepartmentSubSectionArticlesListFromArticleId(DepartmentId, 999, ArticleId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesListFromArticleIdIncorrectArticleId()
        {
            var articleRevisions = _controller.GetDepartmentSubSectionArticlesListFromArticleId(DepartmentId, SubSectionTabId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesListFromArticleIdArticleId()
        {
            //First add a new article to the context with a later revision date so it will be returned by the function 
            var laterArticle = GetDemoArticle();
            var newArticleId = 999;
            var newTitle = "Another Article";
            var newRevisionDateTime = _testDateTime.AddDays(-2);

            laterArticle.Id = newArticleId;
            laterArticle.CurrentArticleRevision.Title = newTitle;
            laterArticle.CurrentArticleRevision.RevisionDateTime = newRevisionDateTime;
            _context.Articles.Add(laterArticle);

            var results = _controller.GetDepartmentSubSectionArticlesListFromArticleId(DepartmentId, SubSectionTabId, ArticleId) as OkNegotiatedContentResult<List<ArticleDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(2, _context.Articles.Count());
            Assert.AreEqual(newTitle, _context.Articles.Find(newArticleId).CurrentArticleRevision.Title);
            Assert.AreEqual(newRevisionDateTime, _context.Articles.Find(newArticleId).CurrentArticleRevision.RevisionDateTime);
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesListFromArticleIdNoLaterArticles()
        {
            var results = _controller.GetDepartmentSubSectionArticlesListFromArticleId(DepartmentId, SubSectionTabId, ArticleId) as OkNegotiatedContentResult<List<ArticleDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(0, results.Content.Count);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesFromArticleIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesFromArticleId(999, ArticleId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesFromArticleIdIncorrectArticleId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesFromArticleId(DepartmentId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesFromArticleId()
        {
            //First add a new article to the context with a later revision date so it will be returned by the function 
            var laterArticle = GetDemoArticle();
            var newArticleId = 999;
            var newTitle = "Another Article";
            var newRevisionDateTime = _testDateTime.AddDays(-2);

            laterArticle.Id = newArticleId;
            laterArticle.CurrentArticleRevision.Title = newTitle;
            laterArticle.CurrentArticleRevision.RevisionDateTime = newRevisionDateTime;
            _context.Articles.Add(laterArticle);

            var results = _controller.GetDepartmentArticlesFromArticleId(DepartmentId, ArticleId) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(2, _context.Articles.Count());
            Assert.AreEqual(newTitle, _context.Articles.Find(newArticleId).CurrentArticleRevision.Title);
            Assert.AreEqual(newRevisionDateTime, _context.Articles.Find(newArticleId).CurrentArticleRevision.RevisionDateTime);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesFromArticleIdNoLaterArticles()
        {
            var results = _controller.GetDepartmentArticlesFromArticleId(DepartmentId, ArticleId) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(0, results.Content.Count);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListFromArticleIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesListFromArticleId(999, ArticleId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListFromArticleIdIncorrectArticleId()
        {
            var articleRevisions = _controller.GetDepartmentArticlesListFromArticleId(DepartmentId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListFromArticleId()
        {
            //First add a new article to the context with a later revision date so it will be returned by the function 
            var laterArticle = GetDemoArticle();
            var newArticleId = 999;
            var newTitle = "Another Article";
            var newRevisionDateTime = _testDateTime.AddDays(-2);

            laterArticle.Id = newArticleId;
            laterArticle.CurrentArticleRevision.Title = newTitle;
            laterArticle.CurrentArticleRevision.RevisionDateTime = newRevisionDateTime;
            _context.Articles.Add(laterArticle);

            var results = _controller.GetDepartmentArticlesListFromArticleId(DepartmentId, ArticleId) as OkNegotiatedContentResult<List<ArticleDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(2, _context.Articles.Count());
            Assert.AreEqual(newTitle, _context.Articles.Find(newArticleId).CurrentArticleRevision.Title);
            Assert.AreEqual(newRevisionDateTime, _context.Articles.Find(newArticleId).CurrentArticleRevision.RevisionDateTime);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesListFromArticleIdNoLaterArticles()
        {
            var results = _controller.GetDepartmentArticlesListFromArticleId(DepartmentId, ArticleId) as OkNegotiatedContentResult<List<ArticleDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(0, results.Content.Count);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesFromSearchTermIncorrectDepartmentId()
        {
            var searchTerm = "Test";
            var articleRevisions = _controller.GetDepartmentArticlesFromSearchTerm(999, searchTerm) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesFromSearchTerm()
        {
            var searchTerm = "Test";
            var results = _controller.GetDepartmentArticlesFromSearchTerm(DepartmentId, searchTerm) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetDepartmentArticlesFromSearchTermToShort()
        {
            var searchTerm = "se";
            var results = _controller.GetDepartmentArticlesFromSearchTerm(DepartmentId, searchTerm) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetDepartmentArticlesFromSearchTermNull()
        {
            var results = _controller.GetDepartmentArticlesFromSearchTerm(DepartmentId, null) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromSearchTermIncorrectDepartmentId()
        {
            var searchTerm = "Test";
            var articleRevisions = _controller.GetDepartmentSubSectionArticlesFromSearchTerm(999, SubSectionTabId, searchTerm) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromSearchTermIncorrectSubSectionTabId()
        {
            var searchTerm = "Test";
            var articleRevisions = _controller.GetDepartmentSubSectionArticlesFromSearchTerm(DepartmentId, 999, searchTerm) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromSearchTerm()
        {
            var searchTerm = "Test";
            var results = _controller.GetDepartmentSubSectionArticlesFromSearchTerm(DepartmentId, SubSectionTabId, searchTerm) as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromSearchTermToShort()
        {
            var searchTerm = "se";
            var results = _controller.GetDepartmentSubSectionArticlesFromSearchTerm(DepartmentId, SubSectionTabId, searchTerm) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetDepartmentSubSectionArticlesFromSearchTermTermNull()
        {
            var results = _controller.GetDepartmentSubSectionArticlesFromSearchTerm(DepartmentId, SubSectionTabId, null) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetUsersByDepartmentIncorrectId()
        {
            var articleRevisions = _controller.GetUsersByDepartment(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetUsersByDepartmentCorrectId()
        {
            var results = _controller.GetUsersByDepartment(DepartmentId) as OkNegotiatedContentResult<List<UserDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<UserDto>>));
            Assert.IsNotNull(results);

            var users = results.Content;

            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(_context.Users.Find(UserId).JobTitle, users[0].JobTitle);
        }

        [TestMethod]
        public void TestGetAllUsersByDepartmentIncorrectId()
        {
            var articleRevisions = _controller.GetAllUsersByDepartment(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetAllUsersByDepartmentCorrectId()
        {
            var results = _controller.GetAllUsersByDepartment(DepartmentId) as OkNegotiatedContentResult<List<UserDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<UserDto>>));
            Assert.IsNotNull(results);

            var users = results.Content;

            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(_context.Users.Find(UserId).JobTitle, users[0].JobTitle);
        }

        [TestMethod]
        public void TestGetUsersByDepartmentFromContactIdIncorrectDepartmentId()
        {
            var articleRevisions = _controller.GetUsersByDepartmentFromContactId(999, UserId) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetUsersByDepartmentFromContactIdIncorrectUserId()
        {
            var articleRevisions = _controller.GetUsersByDepartmentFromContactId(DepartmentId, 999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetUsersByDepartmentFromContactId()
        {
            //First add a new user as this result set is ordered by forename
            var laterUser = GetDemoUser();
            var newUserId = 999;
            var newForename = "Zebadia";

            laterUser.Id = newUserId;
            laterUser.Forename = newForename;
            _context.Users.Add(laterUser);

            var results = _controller.GetUsersByDepartmentFromContactId(DepartmentId, UserId) as OkNegotiatedContentResult<List<UserDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<UserDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(2, _context.Users.Count());
            Assert.AreEqual(newForename, _context.Users.Find(newUserId).Forename);
        }

        [TestMethod]
        public void TestGetUsersByDepartmentFromContactIdNoLaterUsers()
        {
            var results = _controller.GetUsersByDepartmentFromContactId(DepartmentId, UserId) as OkNegotiatedContentResult<List<UserDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<UserDto>>));
            Assert.IsNotNull(results);

            Assert.AreEqual(0, results.Content.Count);
        }

        [TestMethod]
        public void TestGetUsersByDepartmentFromSearchTermIncorrectDepartmentId()
        {
            var searchTerm = "Test";
            var articleRevisions = _controller.GetUsersByDepartmentFromSearchTerm(999, searchTerm) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetUsersByDepartmentFromSearchTerm()
        {
            var searchTerm = "Test";
            var results = _controller.GetUsersByDepartmentFromSearchTerm(DepartmentId, searchTerm) as OkNegotiatedContentResult<List<UserDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<UserDto>>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetUsersByDepartmentFromSearchTermToShort()
        {
            var searchTerm = "se";
            var results = _controller.GetUsersByDepartmentFromSearchTerm(DepartmentId, searchTerm) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetUsersByDepartmentFromSearchTermNull()
        {
            var results = _controller.GetUsersByDepartmentFromSearchTerm(DepartmentId, null) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetUsersAndArticlesByDepartmentFromSearchTermDepartmentId()
        {
            var searchTerm = "Test";
            var articleRevisions = _controller.GetUsersAndArticlesByDepartmentFromSearchTerm(999, searchTerm) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetUsersAndArticlesByDepartmentFromSearchTerm()
        {
            var searchTerm = "Test";
            var results = _controller.GetUsersAndArticlesByDepartmentFromSearchTerm(DepartmentId, searchTerm) as OkNegotiatedContentResult<SearchResultDto>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<SearchResultDto>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetUsersAndArticlesByDepartmentFromSearchTermToShort()
        {
            var searchTerm = "se";
            var results = _controller.GetUsersAndArticlesByDepartmentFromSearchTerm(DepartmentId, searchTerm) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetUsersAndArticlesByDepartmentFromSearchTermNull()
        {
            var results = _controller.GetUsersAndArticlesByDepartmentFromSearchTerm(DepartmentId, null) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetUsersAndArticlesFromSearchTerm()
        {
            var searchTerm = "Test";
            var results = _controller.GetUsersAndArticlesFromSearchTerm(searchTerm) as OkNegotiatedContentResult<SearchResultDto>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<SearchResultDto>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetUsersAndArticlesFromSearchTermToShort()
        {
            var searchTerm = "se";
            var results = _controller.GetUsersAndArticlesFromSearchTerm(searchTerm) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestGetUsersAndArticlesFromSearchTermNull()
        {
            var results = _controller.GetUsersAndArticlesFromSearchTerm(null) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestPutDepartmentIncorrectId()
        {
            var results = _controller.PutDepartment(999, GetDemoDepartment()) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestPutDepartmentCorrectId()
        {
            var department = GetDemoDepartment();
            var newName = "New Name";
            department.Name = newName;

            Assert.AreEqual(1, _context.Departments.Count());

            var results = _controller.PutDepartment(DepartmentId, department) as StatusCodeResult;

            Assert.IsInstanceOfType(results, typeof(StatusCodeResult));
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.NoContent, results.StatusCode);
            Assert.AreEqual(1, _context.Departments.Count());
        }

        [TestMethod]
        public void TestPostDepartment()
        {
            Assert.AreEqual(1, _context.Departments.Count());

            var result = _controller.PostDepartment(GetDemoDepartment()) as OkNegotiatedContentResult<Department>;

            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Department>));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Content, typeof(Department));

            Assert.AreEqual(2, _context.Departments.Count());
        }

        [TestMethod]
        public void TestPutArchiveDepartmentIncorrectId()
        {
            var result = _controller.PutArchiveDepartment(999) as NotFoundResult;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestPutArchiveDepartmentCorrectId()
        {
            var result = _controller.PutArchiveDepartment(DepartmentId) as StatusCodeResult;

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);

            var contextDepartment = _context.Departments.Find(DepartmentId);
            Assert.AreEqual(true, contextDepartment.Archived);
        }
    }
}
