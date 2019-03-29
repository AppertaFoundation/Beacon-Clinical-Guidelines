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
using System.Web.Http.Results;
using ClinicalGuidelines.Controllers;
using ClinicalGuidelines.Models;
using ClinicalGuidelinesTest.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ClinicalGuidelinesTest.Controllers
{
    [TestClass]
    public class ArticleRevisionApiControllerTest
    {
        private ArticleRevisionApiController _controller;
        private readonly TestClinicalGuidelinesAppContext _context = new TestClinicalGuidelinesAppContext();

        private ArticleRevision _inMemoryArticleRevision;

        private const int ArticleRevisionId = 1;

        private readonly DateTime _testDateTime = Convert.ToDateTime("2016-01-01 00:00:00");

        private void GetArticleRevisionApiController()
        {
            _controller = new ArticleRevisionApiController(_context);
        }

        private ArticleRevision GetDemoArticleRevision()
        {
            return new ArticleRevision()
            {
                Id = ArticleRevisionId,
                ArticleId = 1,
                SubSectionTabId = 1,
                Title = "Test Article",
                Structure = JsonConvert.SerializeObject("Test Structure"),
                Body = "Test Body",
                Live = false,
                Public = false,
                ReviewDateTime = null,
                StartDateTime = null,
                ExpiryDateTime = null,
                UserId = 1,
                Template = 1,
                RevisionDateTime = _testDateTime,
                Departments = GetDemoDepartmentAsList(),
                Files = JsonConvert.SerializeObject("Test Files")
            };
        }

        private Department GetDemoDepartment()
        {
            return new Department()
            {
                Id = 1,
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

        private void AddDemoToInMemoryContext()
        {
            _inMemoryArticleRevision = GetDemoArticleRevision();
            _context.ArticleRevisions.Add(_inMemoryArticleRevision);
        }

        private void CheckArticleRevision(ArticleRevision articleRevision)
        {
            var demoArticleRevision = GetDemoArticleRevision();

            Assert.AreEqual(demoArticleRevision.Id, articleRevision.Id);
            Assert.AreEqual(demoArticleRevision.ArticleId, articleRevision.ArticleId);
            Assert.AreEqual(demoArticleRevision.SubSectionTabId, articleRevision.SubSectionTabId);
            Assert.AreEqual(demoArticleRevision.Title, articleRevision.Title);
            Assert.AreEqual(demoArticleRevision.Structure, articleRevision.Structure);
            Assert.AreEqual(demoArticleRevision.Body, articleRevision.Body);
            Assert.AreEqual(demoArticleRevision.Live, articleRevision.Live);
            Assert.AreEqual(demoArticleRevision.Public, articleRevision.Public);
            Assert.AreEqual(demoArticleRevision.ReviewDateTime, articleRevision.ReviewDateTime);
            Assert.AreEqual(demoArticleRevision.StartDateTime, articleRevision.StartDateTime);
            Assert.AreEqual(demoArticleRevision.ExpiryDateTime, articleRevision.ExpiryDateTime);
            Assert.AreEqual(demoArticleRevision.UserId, articleRevision.UserId);
            Assert.AreEqual(demoArticleRevision.Template, articleRevision.Template);
            Assert.AreEqual(demoArticleRevision.RevisionDateTime, articleRevision.RevisionDateTime);
            Assert.AreEqual(demoArticleRevision.Files, articleRevision.Files);

            if (articleRevision.Departments != null)
            {
                Assert.AreEqual(demoArticleRevision.Departments.Count, articleRevision.Departments.Count);

                for (var i = 0; i < articleRevision.Departments.Count; i++)
                {
                    Assert.AreEqual(demoArticleRevision.Departments[i].Id, articleRevision.Departments[i].Id);
                    Assert.AreEqual(demoArticleRevision.Departments[i].Name, articleRevision.Departments[i].Name);
                    Assert.AreEqual(demoArticleRevision.Departments[i].ShortName, articleRevision.Departments[i].ShortName);
                }
            }
        }

        [TestInitialize]
        public void SetUp()
        {
            GetArticleRevisionApiController();
            AddDemoToInMemoryContext();
        }

        [TestMethod]
        public void TestGetArticles()
        {
            var articleRevisions = _controller.GetArticleRevisions() as TestArticleRevisionDbSet;

            Assert.IsInstanceOfType(articleRevisions, typeof(TestArticleRevisionDbSet));
            Assert.IsNotNull(articleRevisions);

            Assert.AreEqual(1, articleRevisions.Count());

            foreach (var articleRevision in articleRevisions)
            {
                CheckArticleRevision(articleRevision);
            }
        }

        [TestMethod]
        public void TestGetArticleIncorrectId()
        {
            var articleRevisions = _controller.GetArticleRevision(999) as NotFoundResult;

            Assert.IsInstanceOfType(articleRevisions, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestGetArticleCorrectId()
        {
            var results = _controller.GetArticleRevision(ArticleRevisionId) as OkNegotiatedContentResult<ArticleRevision>;

            Assert.IsInstanceOfType(results, typeof(OkNegotiatedContentResult<ArticleRevision>));
            Assert.IsNotNull(results);

            CheckArticleRevision(results.Content);
        }

        [TestMethod]
        public void TestPutArticleRevisionIncorrectId()
        {
            var results = _controller.PutArticleRevision(999, GetDemoArticleRevision()) as BadRequestResult;

            Assert.IsInstanceOfType(results, typeof(BadRequestResult));
        }

        [TestMethod]
        public void TestPutArticleRevisionCorrectId()
        {
            var articleRevision = GetDemoArticleRevision();
            var newBody = "New Body";
            articleRevision.Body = newBody;

            Assert.AreEqual(1, _context.ArticleRevisions.Count());

            var results = _controller.PutArticleRevision(ArticleRevisionId, articleRevision) as StatusCodeResult;

            Assert.IsInstanceOfType(results, typeof(StatusCodeResult));
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.NoContent, results.StatusCode);
            Assert.AreEqual(1, _context.ArticleRevisions.Count());
        }

        [TestMethod]
        public void TestPostArticleRevision()
        {
            Assert.AreEqual(1, _context.ArticleRevisions.Count());

            var result = _controller.PostArticleRevision(GetDemoArticleRevision()) as OkNegotiatedContentResult<ArticleRevision>;

            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ArticleRevision>));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Content, typeof(ArticleRevision));

            Assert.AreEqual(2, _context.ArticleRevisions.Count());
        }

        [TestMethod]
        public void TestDeleteArticleRevisionIncorrectId()
        {
            var result = _controller.DeleteArticleRevision(999) as NotFoundResult;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void TestDeleteArticleRevisionCorrectId()
        {
            var result = _controller.DeleteArticleRevision(ArticleRevisionId) as OkNegotiatedContentResult<ArticleRevision>;

            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ArticleRevision>));
            Assert.AreEqual(0, _context.ArticleRevisions.Count());
        }
    }
}
