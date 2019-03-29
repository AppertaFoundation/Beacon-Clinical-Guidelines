//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
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
    public class ArticleApiControllerTest 
    {
        private ArticleApiController _controller;
        private readonly TestClinicalGuidelinesAppContext _context = new TestClinicalGuidelinesAppContext();
        private readonly TestMappingHelper _mappingHelper = new TestMappingHelper();

        private Article _inMemoryArticle;
        private ArticleRevision _inMemoryArticleRevision;
        private Department _inMemoryDepartment;
        private SubSectionTab _inMemorySubSectionTab;
        private Term _inMemoryTerm;
        private User _inMemoryUser;

        private const int ArticleId = 1;
        private const int ArticleRevisionId = 2;
        private const int DepartmentId = 3;
        private const int SubSectionTabId = 4;
        private const int TermId = 5;
        private const int UserId = 6;

        private const string AdminUser = "AdminTest";

        private readonly DateTime _testDateTime = Convert.ToDateTime("2016-01-01 00:00:00");

        private void GetArticleApiController()
        {
            _controller = new ArticleApiController(_context, _mappingHelper);
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

        private ArticleRevisionDto GetDemoArticleRevisionDto()
        {
            var article = GetDemoArticle();
            var termDtos = article.CurrentArticleRevision.Terms.Select(
                term => new TermDto()
                {
                    Id = term.Id, Name = term.Name
                }
            ).ToList();

            return new ArticleRevisionDto()
            {
                Id = article.Id,
                _id = article._id,
                RevisionId = article.CurrentArticleRevision.Id,
                Title = article.CurrentArticleRevision.Title,
                SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                Body = article.CurrentArticleRevision.Body,
                Structure = article.CurrentArticleRevision.Structure,
                RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                Viewed = article.HitCount,
                UserId = article.CurrentArticleRevision.UserId,
                Departments = GetDemoDepartmentDtoAsList(),
                Template = article.CurrentArticleRevision.Template,
                SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                Terms = termDtos,
                Files = article.CurrentArticleRevision.Files
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
                ArchiveDateTime = null
            };
        }

        private List<Department> GetDemoDepartmentAsList()
        {
            return new List<Department> { GetDemoDepartment() }; ;
        }

        private DepartmentDto GetDemoDepartmentDto()
        {
            return new DepartmentDto()
            {
                Id = DepartmentId,
                _id = new Guid("51d895f8-29d0-49c1-878e-8972095be5c3"),
                Name = "Test Department",
                ShortName = "TEST",
            };
        }

        private List<DepartmentDto> GetDemoDepartmentDtoAsList()
        {
            return new List<DepartmentDto> { GetDemoDepartmentDto() };
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
                Icon = "Test-Icon.png"
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
            _inMemoryArticle = GetDemoArticle();
            _inMemoryArticleRevision = GetDemoArticleRevision();
            _inMemoryDepartment = GetDemoDepartment();
            _inMemorySubSectionTab = GetDemoSubSectionTab();
            _inMemoryTerm = GetDemoTerm();
            _inMemoryUser = GetDemoUser();
            _context.Articles.Add(_inMemoryArticle);
            _context.ArticleRevisions.Add(_inMemoryArticleRevision);
            _context.Departments.Add(_inMemoryDepartment);
            _context.SubSectionTabs.Add(_inMemorySubSectionTab);
            _context.Terms.Add(_inMemoryTerm);
            _context.Users.Add(_inMemoryUser);
        }

        private void CheckArticleRevisionDtoAgainstArticle(ArticleRevisionDto articleRevisionDto)
        {
            Assert.AreEqual(_inMemoryArticle.Id, articleRevisionDto.Id);
            Assert.AreEqual(_inMemoryArticle._id, articleRevisionDto._id);
            Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.Body, articleRevisionDto.Body);
            Assert.AreEqual(JsonConvert.DeserializeObject(_inMemoryArticle.CurrentArticleRevision.Structure), articleRevisionDto.Structure);
            Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.SubSectionTab.Name, articleRevisionDto.SubTitle);
            Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.Title, articleRevisionDto.Title);
            Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.RevisionDateTime, articleRevisionDto.RevisionDateTime);
            Assert.AreEqual(_inMemoryArticle.HitCount, articleRevisionDto.Viewed);
            Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.UserId, articleRevisionDto.UserId);
            Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.SubSectionTab.Id, articleRevisionDto.SubSectionTabId);
            Assert.AreEqual(JsonConvert.DeserializeObject(_inMemoryArticle.CurrentArticleRevision.Files), articleRevisionDto.Files);
            Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.Template, articleRevisionDto.Template);

            if (articleRevisionDto.Departments != null)
            {
                Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.Departments.Count, articleRevisionDto.Departments.Count);

                for (var i = 0; i < articleRevisionDto.Departments.Count; i++)
                {
                    Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.Departments[i].Id, articleRevisionDto.Departments[i].Id);
                    Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.Departments[i].Name, articleRevisionDto.Departments[i].Name);
                    Assert.AreEqual(_inMemoryArticle.CurrentArticleRevision.Departments[i].ShortName, articleRevisionDto.Departments[i].ShortName);
                }
            }

            if (articleRevisionDto.Terms != null)
            {
                foreach (var termDto in articleRevisionDto.Terms)
                {
                    CheckTermDtoAgainstArticleTerms(termDto, _inMemoryArticle.CurrentArticleRevision.Terms[0]);
                }
            }

            if (articleRevisionDto.RelatedArticleDtos != null)
            {
                //Tested separately in the helper class test
                Assert.AreEqual(0, articleRevisionDto.RelatedArticleDtos.Count);
            }
        }

        private void CheckTermDtoAgainstArticleTerms(TermDto termDto, Term term)
        {
            Assert.AreEqual(term.Id, termDto.Id);
            Assert.AreEqual(term.Name, termDto.Name);
        }

        [TestInitialize]
        public void SetUp()
        {
            GetArticleApiController();
            AddDemoToInMemoryContext();
            var identity = new GenericIdentity(AdminUser);
            Thread.CurrentPrincipal = new GenericPrincipal(identity, null);
        }

        [TestMethod]
        public void TestGetArticles()
        {
            var results = _controller.GetArticles() as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);

            var articleRevisionDtos = results.Content;

            Assert.AreEqual(1, articleRevisionDtos.Count);

            foreach (var articleRevisionDto in articleRevisionDtos)
            {
                CheckArticleRevisionDtoAgainstArticle(articleRevisionDto);
            }
        }

        [TestMethod]
        public void TestGetArticleCorrectId()
        {
            var results = _controller.GetArticle(ArticleId) as OkNegotiatedContentResult<ArticleRevisionDto>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<ArticleRevisionDto>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetArticleIncorrectId()
        {
            var results = _controller.GetArticle(999) as NotFoundResult;

            Assert.IsInstanceOfType(results, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetArticleAppSideCorrectId()
        {
            var results = _controller.GetArticleAppSide(ArticleId) as OkNegotiatedContentResult<ArticleRevisionDto>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<ArticleRevisionDto>));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestGetArticleAppSideIncorrectId()
        {
            var results = _controller.GetArticleAppSide(999) as NotFoundResult;

            Assert.IsInstanceOfType(results, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestPutArticleHitCorrectId()
        {
            var result = _controller.PutAddToHitCountForArticle(ArticleId);

            Assert.IsInstanceOfType(result, typeof(OkResult));

            var results = _controller.GetArticles() as OkNegotiatedContentResult<List<ArticleRevisionDto>>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<List<ArticleRevisionDto>>));
            Assert.IsNotNull(results);

            var articleRevisionDtos = results.Content;

            Assert.AreEqual(1, articleRevisionDtos.Count);
            Assert.AreEqual(GetDemoArticle().HitCount+1, articleRevisionDtos[0].Viewed);
        }

        [TestMethod]
        public void TestPutArticleHitIncorrectId()
        {
            var results = _controller.PutAddToHitCountForArticle(999) as NotFoundResult;

            Assert.IsInstanceOfType(results, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestPutArticleCorrectIdNoNewRevision()
        {
            //Test of editing a revision not creating a new one
            var changedText = "New Body";
            var newArticleRevisionDto = GetDemoArticleRevisionDto();
            newArticleRevisionDto.Body = changedText;
            Assert.AreEqual(1, _context.ArticleRevisions.Count());
            var result = _controller.PutArticle(ArticleId, newArticleRevisionDto) as StatusCodeResult;

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
            Assert.AreEqual(1, _context.ArticleRevisions.Count());
            Assert.AreEqual(changedText, _context.ArticleRevisions.Find(ArticleRevisionId).Body);
        }

        [TestMethod]
        public void TestPutArticleIncorrectId()
        {
            var results = _controller.PutArticle(999, GetDemoArticleRevisionDto()) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestPutArticleMissingId()
        {
            var articleRevisionDto = GetDemoArticleRevisionDto();
            articleRevisionDto.Id = 999;
            var results = _controller.PutArticle(999, articleRevisionDto) as StatusCodeResult;

            Assert.IsInstanceOfType(results, typeof(StatusCodeResult));
        }

        [TestMethod]
        public void TestPutArticleWithNewTermCorrectId()
        {
            var newTerm = new TermDto()
            {
                Id = 0,
                Name = "New Term"
            };
            var newArticleRevisionDto = GetDemoArticleRevisionDto();
            newArticleRevisionDto.Terms.Add(newTerm);
            var result = _controller.PutArticle(ArticleId, newArticleRevisionDto) as StatusCodeResult;

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
            Assert.AreEqual(2, _context.ArticleRevisions.Find(newArticleRevisionDto.RevisionId).Terms.Count());
            Assert.AreEqual("New Term", _context.ArticleRevisions.Find(newArticleRevisionDto.RevisionId).Terms[1].Name);
        }

        [TestMethod]
        public void TestPutArticleWithExistingUnlinkedTermCorrectId()
        {
            //Add the term to the context to test that it is linked to rather than a new term added
            var newTerm = new Term()
            {
                Id = 999,
                Name = "New Term"
            };
            _context.Terms.Add(newTerm);

            var newTermDto = new TermDto()
            {
                Id = 999,
                Name = "New Term"
            };
            var newArticleRevisionDto = GetDemoArticleRevisionDto();
            newArticleRevisionDto.Terms.Add(newTermDto);
            var result = _controller.PutArticle(ArticleId, newArticleRevisionDto) as StatusCodeResult;

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
            Assert.AreEqual(2, _context.ArticleRevisions.Find(newArticleRevisionDto.RevisionId).Terms.Count());
            Assert.AreEqual("New Term", _context.ArticleRevisions.Find(newArticleRevisionDto.RevisionId).Terms[1].Name);
        }

        [TestMethod]
        public void TestPutArticleWithEmptyGuidCorrectId()
        {
            var newArticleRevisionDto = GetDemoArticleRevisionDto();
            newArticleRevisionDto._id = Guid.Empty;
            var result = _controller.PutArticle(ArticleId, newArticleRevisionDto) as StatusCodeResult;

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
            Assert.AreEqual(1, _context.ArticleRevisions.Count());
            Assert.AreNotEqual(Guid.Empty, _context.Articles.Find(ArticleId)._id);
        }

        [TestMethod]
        public void TestPostArticle()
        {
            var newArticleRevisionDto = GetDemoArticleRevisionDto();
            var newBody = "New Body";
            newArticleRevisionDto.Body = newBody;

            Assert.AreEqual(1, _context.Articles.Count());
            Assert.AreEqual(1, _context.ArticleRevisions.Count());

            var result = _controller.PostNewArticle(newArticleRevisionDto) as OkResult;

            Assert.IsInstanceOfType(result, typeof(OkResult));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, _context.Articles.Count());
            Assert.AreEqual(2, _context.ArticleRevisions.Count());

            Assert.AreEqual(newBody, _context.ArticleRevisions.Find(0).Body);
        }

        [TestMethod]
        public void TestArticleIncorrectId()
        {
            var result = _controller.PutArchiveArticle(999) as NotFoundResult;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestArticleCorrectId()
        {
            var result = _controller.PutArchiveArticle(ArticleId) as StatusCodeResult;

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));

            var contextArticle = _context.Articles.Find(ArticleId);
            Assert.AreEqual(true, contextArticle.Archived);
        }

        [TestMethod]
        public void TestUnArticleIncorrectId()
        {
            var result = _controller.PutUnArchiveArticle(999) as NotFoundResult;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestUnArticleCorrectId()
        {
            var result = _controller.PutUnArchiveArticle(ArticleId) as StatusCodeResult;

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));

            var contextArticle = _context.Articles.Find(ArticleId);
            Assert.AreEqual(false, contextArticle.Archived);
        }
    }
}
