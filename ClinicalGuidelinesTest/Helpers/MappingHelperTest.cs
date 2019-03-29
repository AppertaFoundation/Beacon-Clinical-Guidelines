//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Helpers;
using ClinicalGuidelines.Models;
using ClinicalGuidelinesTest.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ClinicalGuidelinesTest.Helpers
{
    [TestClass]
    public class MappingHelperTest
    {
        private readonly MappingHelper _mappingHelper = new MappingHelper();
        private readonly TestClinicalGuidelinesAppContext _context = new TestClinicalGuidelinesAppContext();

        private Article _inMemoryArticle;
        private ArticleRevision _inMemoryArticleRevision;
        private Department _inMemoryDepartment;
        private SubSectionTab _inMemorySubSectionTab;
        private Term _inMemoryTerm;

        private const int ArticleId = 1;
        private const int ArticleRevisionId = 2;
        private const int DepartmentId = 3;
        private const int SubSectionTabId = 4;
        private const int TermId = 5;
        private const int RelatedArticleId = 6;
        private const int RelatedArticleRevisionId = 7;

        private readonly DateTime _testDateTime = Convert.ToDateTime("2016-01-01 00:00:00");

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

        private Article GetDemoRelatedArticle()
        {
            return new Article()
            {
                Id = RelatedArticleId,
                _id = new Guid("d58a8b27-b73b-46f5-aa5d-e062eba10a84"),
                ArticleRevisionId = RelatedArticleRevisionId,
                HitCount = 2,
                CurrentArticleRevision = GetDemoRelatedArticleRevision(),
                Archived = false
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

        private TermDto GetDemoTermDto()
        {
            return new TermDto()
            {
                Id = TermId,
                Name = "Test Term"
            };
        }

        private List<TermDto> GetDemoTermDtos()
        {
            return new List<TermDto>()
            {
                GetDemoTermDto()
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

        private ArticleRevision GetDemoRelatedArticleRevision()
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

        private ArticleRevisionDto GetDemoArticleRevisionDto()
        {
            return new ArticleRevisionDto()
            {
                Id = ArticleId,
                _id = new Guid("d58a8b27-b73b-46f5-aa5d-e062eba10a84"),
                SubSectionTabId = SubSectionTabId,
                Title = "Test Article",
                Structure = JsonConvert.SerializeObject("Test Structure"),
                Body = "Test Body",
                UserId = 1,
                Template = 1,
                RevisionDateTime = _testDateTime,
                Departments = GetDemoDepartmentDtoAsList(),
                Terms = GetDemoTermDtos(),
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
                ArchiveDateTime = null,
                Live = true
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
            return new List < DepartmentDto > { GetDemoDepartmentDto()};
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

        private void AddDemoToInMemoryContext()
        {
            _inMemoryArticle = GetDemoArticle();
            _inMemoryArticleRevision = GetDemoArticleRevision();
            _inMemoryDepartment = GetDemoDepartment();
            _inMemorySubSectionTab = GetDemoSubSectionTab();
            _inMemoryTerm = GetDemoTerm();
            _context.Articles.Add(_inMemoryArticle);
            _context.ArticleRevisions.Add(_inMemoryArticleRevision);
            _context.Departments.Add(_inMemoryDepartment);
            _context.SubSectionTabs.Add(_inMemorySubSectionTab);
            _context.Terms.Add(_inMemoryTerm);
        }

        [TestInitialize]
        public void SetUp()
        {
            AddDemoToInMemoryContext();
        }

        [TestMethod]
        public void TestMapArticlesToArticleRevisionDtosMappingRelatedFalse()
        {
            var article = GetDemoArticle();
            var articles = new List<Article>() { article };
            var articleRevisionDtos = _mappingHelper.MapArticlesToArticleRevisionDtos(articles, false, _context);
            
            Assert.IsInstanceOfType(articleRevisionDtos, typeof(List<ArticleRevisionDto>));
            Assert.IsNotNull(articleRevisionDtos);
            Assert.AreEqual(1, articleRevisionDtos.Count);

            foreach (var articleRevisionDto in articleRevisionDtos)
            {
                Assert.AreEqual(article.Id, articleRevisionDto.Id);
                Assert.AreEqual(article._id, articleRevisionDto._id);
                Assert.AreEqual(article.CurrentArticleRevision.Title, articleRevisionDto.Title);
                Assert.AreEqual(article.CurrentArticleRevision.SubSectionTab.Name, articleRevisionDto.SubTitle);
                Assert.AreEqual(article.CurrentArticleRevision.Body, articleRevisionDto.Body);
                Assert.AreEqual(article.CurrentArticleRevision.RevisionDateTime, articleRevisionDto.RevisionDateTime);
                Assert.AreEqual(article.CurrentArticleRevision.SubSectionTab.Id, articleRevisionDto.SubSectionTabId);
                Assert.AreEqual(JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files), articleRevisionDto.Files);

                if (articleRevisionDto.Departments != null)
                {
                    Assert.AreEqual(article.CurrentArticleRevision.Departments.Count, articleRevisionDto.Departments.Count);

                    for (var i = 0; i < articleRevisionDto.Departments.Count; i++)
                    {
                        Assert.AreEqual(article.CurrentArticleRevision.Departments[i].Id, articleRevisionDto.Departments[i].Id);
                        Assert.AreEqual(article.CurrentArticleRevision.Departments[i].Name, articleRevisionDto.Departments[i].Name);
                        Assert.AreEqual(article.CurrentArticleRevision.Departments[i].ShortName, articleRevisionDto.Departments[i].ShortName);
                    }
                }

                if (articleRevisionDto.Terms != null)
                {
                    Assert.AreEqual(article.CurrentArticleRevision.Terms.Count, articleRevisionDto.Terms.Count);

                    for (var i = 0; i < articleRevisionDto.Terms.Count; i++)
                    {
                        Assert.AreEqual(article.CurrentArticleRevision.Terms[i].Id, articleRevisionDto.Terms[i].Id);
                        Assert.AreEqual(article.CurrentArticleRevision.Terms[i].Name, articleRevisionDto.Terms[i].Name);
                    }
                }

                Assert.IsNull(articleRevisionDto.RelatedArticleDtos);
            }
        }

        [TestMethod]
        public void TestMapArticlesToArticleRevisionDtosMappingRelatedTrue()
        {
            //Add the related article to context
            var relatedArticle = GetDemoRelatedArticle();
            _context.Articles.Add(relatedArticle);

            var article = GetDemoArticle();
            var articles = new List<Article>() { article };
            var articleRevisionDtos = _mappingHelper.MapArticlesToArticleRevisionDtos(articles, true, _context);

            Assert.IsInstanceOfType(articleRevisionDtos, typeof(List<ArticleRevisionDto>));
            Assert.IsNotNull(articleRevisionDtos);
            Assert.AreEqual(1, articleRevisionDtos.Count);

            foreach (var articleRevisionDto in articleRevisionDtos)
            {
                Assert.AreEqual(article.Id, articleRevisionDto.Id);
                Assert.AreEqual(article._id, articleRevisionDto._id);
                Assert.AreEqual(article.CurrentArticleRevision.Title, articleRevisionDto.Title);
                Assert.AreEqual(article.CurrentArticleRevision.SubSectionTab.Name, articleRevisionDto.SubTitle);
                Assert.AreEqual(article.CurrentArticleRevision.Body, articleRevisionDto.Body);
                Assert.AreEqual(article.CurrentArticleRevision.RevisionDateTime, articleRevisionDto.RevisionDateTime);
                Assert.AreEqual(article.CurrentArticleRevision.SubSectionTab.Id, articleRevisionDto.SubSectionTabId);
                Assert.AreEqual(JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files), articleRevisionDto.Files);

                if (articleRevisionDto.Departments != null)
                {
                    Assert.AreEqual(article.CurrentArticleRevision.Departments.Count, articleRevisionDto.Departments.Count);

                    for (var i = 0; i < articleRevisionDto.Departments.Count; i++)
                    {
                        Assert.AreEqual(article.CurrentArticleRevision.Departments[i].Id, articleRevisionDto.Departments[i].Id);
                        Assert.AreEqual(article.CurrentArticleRevision.Departments[i].Name, articleRevisionDto.Departments[i].Name);
                        Assert.AreEqual(article.CurrentArticleRevision.Departments[i].ShortName, articleRevisionDto.Departments[i].ShortName);
                    }
                }

                if (articleRevisionDto.Terms != null)
                {
                    Assert.AreEqual(article.CurrentArticleRevision.Terms.Count, articleRevisionDto.Terms.Count);

                    for (var i = 0; i < articleRevisionDto.Terms.Count; i++)
                    {
                        Assert.AreEqual(article.CurrentArticleRevision.Terms[i].Id, articleRevisionDto.Terms[i].Id);
                        Assert.AreEqual(article.CurrentArticleRevision.Terms[i].Name, articleRevisionDto.Terms[i].Name);
                    }
                }

                Assert.IsNotNull(articleRevisionDto.RelatedArticleDtos);
                Assert.AreEqual(1, articleRevisionDto.RelatedArticleDtos.Count);

                foreach (var relatedArticlesDto in articleRevisionDto.RelatedArticleDtos)
                {
                    Assert.AreEqual(relatedArticle.Id, relatedArticlesDto.Id);
                    Assert.AreEqual(relatedArticle._id, relatedArticlesDto._id);
                    Assert.AreEqual(relatedArticle.CurrentArticleRevision.Title, relatedArticlesDto.Name);
                    Assert.AreEqual(relatedArticle.CurrentArticleRevision.SubSectionTabId, relatedArticlesDto.SubSectionTabId);

                    if (articleRevisionDto.Departments != null)
                    {
                        Assert.AreEqual(article.CurrentArticleRevision.Departments.Count, articleRevisionDto.Departments.Count);

                        for (var i = 0; i < articleRevisionDto.Departments.Count; i++)
                        {
                            Assert.AreEqual(article.CurrentArticleRevision.Departments[i].Id, articleRevisionDto.Departments[i].Id);
                            Assert.AreEqual(article.CurrentArticleRevision.Departments[i].Name, articleRevisionDto.Departments[i].Name);
                            Assert.AreEqual(article.CurrentArticleRevision.Departments[i].ShortName, articleRevisionDto.Departments[i].ShortName);
                        }
                    }

                    if (articleRevisionDto.Terms != null)
                    {
                        Assert.AreEqual(relatedArticle.CurrentArticleRevision.Terms[0].Id, relatedArticlesDto.Term.Id);
                        Assert.AreEqual(relatedArticle.CurrentArticleRevision.Terms[0].Name, relatedArticlesDto.Term.Name);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMapArticlesToArticleRevisionDtosAsList()
        {
            var article = GetDemoArticle();
            var articles = new List<Article>() { article };
            var articleRevisionDtos = _mappingHelper.MapArticlesToArticleRevisionDtosAsList(articles, _context);

            Assert.IsInstanceOfType(articleRevisionDtos, typeof(List<ArticleDto>));
            Assert.IsNotNull(articleRevisionDtos);
            Assert.AreEqual(1, articleRevisionDtos.Count);

            foreach (var articleRevisionDto in articleRevisionDtos)
            {
                Assert.AreEqual(article.Id, articleRevisionDto.Id);
                Assert.AreEqual(article._id, articleRevisionDto._id);
                Assert.AreEqual(article.CurrentArticleRevision.Title, articleRevisionDto.Title);
                Assert.AreEqual(article.CurrentArticleRevision.SubSectionTab.Name, articleRevisionDto.SubTitle);
            }
        }

        [TestMethod]
        public void TestMapRelatedArticlesToArticleRevisionDto()
        {
            //Add the related article to context
            var relatedArticle = GetDemoRelatedArticle();
            _context.Articles.Add(relatedArticle);

            var articleRevisionDto = GetDemoArticleRevisionDto();
            var articleRevisionDtos = new List<ArticleRevisionDto> {articleRevisionDto};

            var mappedArticleRevisionDtos = _mappingHelper.MapRelatedArticlesToArticleRevisionDto(articleRevisionDtos, _context);

            Assert.IsInstanceOfType(mappedArticleRevisionDtos, typeof(List<ArticleRevisionDto>));
            Assert.IsNotNull(mappedArticleRevisionDtos);
            Assert.AreEqual(1, mappedArticleRevisionDtos.Count);

            foreach (var mappedArticleRevisionDto in mappedArticleRevisionDtos)
            {
                Assert.AreEqual(articleRevisionDto.Id, mappedArticleRevisionDto.Id);
                Assert.AreEqual(articleRevisionDto._id, mappedArticleRevisionDto._id);
                Assert.AreEqual(articleRevisionDto.Title, mappedArticleRevisionDto.Title);
                Assert.AreEqual(articleRevisionDto.SubTitle, mappedArticleRevisionDto.SubTitle);
                Assert.AreEqual(articleRevisionDto.Body, mappedArticleRevisionDto.Body);
                Assert.AreEqual(articleRevisionDto.RevisionDateTime, mappedArticleRevisionDto.RevisionDateTime);
                Assert.AreEqual(articleRevisionDto.SubSectionTabId, mappedArticleRevisionDto.SubSectionTabId);
                Assert.AreEqual(articleRevisionDto.Files, mappedArticleRevisionDto.Files);

                if (mappedArticleRevisionDto.Departments != null && articleRevisionDto.Departments != null)
                {
                    Assert.AreEqual(articleRevisionDto.Departments.Count, mappedArticleRevisionDto.Departments.Count);

                    for (var i = 0; i < articleRevisionDto.Departments.Count; i++)
                    {
                        Assert.AreEqual(articleRevisionDto.Departments[i].Id, mappedArticleRevisionDto.Departments[i].Id);
                        Assert.AreEqual(articleRevisionDto.Departments[i].Name, mappedArticleRevisionDto.Departments[i].Name);
                        Assert.AreEqual(articleRevisionDto.Departments[i].ShortName, mappedArticleRevisionDto.Departments[i].ShortName);
                    }
                }

                if (mappedArticleRevisionDto.Terms != null && articleRevisionDto.Terms != null)
                {
                    Assert.AreEqual(articleRevisionDto.Terms.Count, mappedArticleRevisionDto.Terms.Count);

                    for (var i = 0; i < articleRevisionDto.Terms.Count; i++)
                    {
                        Assert.AreEqual(articleRevisionDto.Terms[i].Id, mappedArticleRevisionDto.Terms[i].Id);
                        Assert.AreEqual(articleRevisionDto.Terms[i].Name, mappedArticleRevisionDto.Terms[i].Name);
                    }
                }

                Assert.IsNotNull(mappedArticleRevisionDto.RelatedArticleDtos);
                Assert.AreEqual(1, mappedArticleRevisionDto.RelatedArticleDtos.Count);

                foreach (var relatedArticlesDto in mappedArticleRevisionDto.RelatedArticleDtos)
                {
                    Assert.AreEqual(relatedArticle.Id, relatedArticlesDto.Id);
                    Assert.AreEqual(relatedArticle._id, relatedArticlesDto._id);
                    Assert.AreEqual(relatedArticle.CurrentArticleRevision.Title, relatedArticlesDto.Name);
                    Assert.AreEqual(relatedArticle.CurrentArticleRevision.SubSectionTabId, relatedArticlesDto.SubSectionTabId);

                    if (mappedArticleRevisionDto.Departments != null && articleRevisionDto.Departments != null)
                    {
                        Assert.AreEqual(articleRevisionDto.Departments.Count, mappedArticleRevisionDto.Departments.Count);

                        for (var i = 0; i < articleRevisionDto.Departments.Count; i++)
                        {
                            Assert.AreEqual(articleRevisionDto.Departments[i].Id, mappedArticleRevisionDto.Departments[i].Id);
                            Assert.AreEqual(articleRevisionDto.Departments[i].Name, mappedArticleRevisionDto.Departments[i].Name);
                            Assert.AreEqual(articleRevisionDto.Departments[i].ShortName, mappedArticleRevisionDto.Departments[i].ShortName);
                        }
                    }

                    if (articleRevisionDto.Terms != null)
                    {
                        Assert.AreEqual(relatedArticle.CurrentArticleRevision.Terms[0].Id, relatedArticlesDto.Term.Id);
                        Assert.AreEqual(relatedArticle.CurrentArticleRevision.Terms[0].Name, relatedArticlesDto.Term.Name);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMapRelatedArticleToArticleRevisionDto()
        {
            //Add the related article to context
            var relatedArticle = GetDemoRelatedArticle();
            _context.Articles.Add(relatedArticle);

            var articleRevisionDto = GetDemoArticleRevisionDto();

            var mappedArticleRevisionDto = _mappingHelper.MapRelatedArticleToArticleRevisionDto(articleRevisionDto, _context);

            Assert.IsInstanceOfType(mappedArticleRevisionDto, typeof(ArticleRevisionDto));
            Assert.IsNotNull(mappedArticleRevisionDto);

            Assert.AreEqual(articleRevisionDto.Id, mappedArticleRevisionDto.Id);
            Assert.AreEqual(articleRevisionDto._id, mappedArticleRevisionDto._id);
            Assert.AreEqual(articleRevisionDto.Title, mappedArticleRevisionDto.Title);
            Assert.AreEqual(articleRevisionDto.SubTitle, mappedArticleRevisionDto.SubTitle);
            Assert.AreEqual(articleRevisionDto.Body, mappedArticleRevisionDto.Body);
            Assert.AreEqual(articleRevisionDto.RevisionDateTime, mappedArticleRevisionDto.RevisionDateTime);
            Assert.AreEqual(articleRevisionDto.SubSectionTabId, mappedArticleRevisionDto.SubSectionTabId);
            Assert.AreEqual(articleRevisionDto.Files, mappedArticleRevisionDto.Files);

            if (mappedArticleRevisionDto.Departments != null && articleRevisionDto.Departments != null)
            {
                Assert.AreEqual(articleRevisionDto.Departments.Count, mappedArticleRevisionDto.Departments.Count);

                for (var i = 0; i < articleRevisionDto.Departments.Count; i++)
                {
                    Assert.AreEqual(articleRevisionDto.Departments[i].Id, mappedArticleRevisionDto.Departments[i].Id);
                    Assert.AreEqual(articleRevisionDto.Departments[i].Name, mappedArticleRevisionDto.Departments[i].Name);
                    Assert.AreEqual(articleRevisionDto.Departments[i].ShortName, mappedArticleRevisionDto.Departments[i].ShortName);
                }
            }

            if (mappedArticleRevisionDto.Terms != null && articleRevisionDto.Terms != null)
            {
                Assert.AreEqual(articleRevisionDto.Terms.Count, mappedArticleRevisionDto.Terms.Count);

                for (var i = 0; i < articleRevisionDto.Terms.Count; i++)
                {
                    Assert.AreEqual(articleRevisionDto.Terms[i].Id, mappedArticleRevisionDto.Terms[i].Id);
                    Assert.AreEqual(articleRevisionDto.Terms[i].Name, mappedArticleRevisionDto.Terms[i].Name);
                }
            }

            Assert.IsNotNull(mappedArticleRevisionDto.RelatedArticleDtos);
            Assert.AreEqual(1, mappedArticleRevisionDto.RelatedArticleDtos.Count);

            foreach (var relatedArticlesDto in mappedArticleRevisionDto.RelatedArticleDtos)
            {
                Assert.AreEqual(relatedArticle.Id, relatedArticlesDto.Id);
                Assert.AreEqual(relatedArticle._id, relatedArticlesDto._id);
                Assert.AreEqual(relatedArticle.CurrentArticleRevision.Title, relatedArticlesDto.Name);
                Assert.AreEqual(relatedArticle.CurrentArticleRevision.SubSectionTabId, relatedArticlesDto.SubSectionTabId);

                if (mappedArticleRevisionDto.Departments != null && articleRevisionDto.Departments != null)
                {
                    Assert.AreEqual(articleRevisionDto.Departments.Count, mappedArticleRevisionDto.Departments.Count);

                    for (var i = 0; i < articleRevisionDto.Departments.Count; i++)
                    {
                        Assert.AreEqual(articleRevisionDto.Departments[i].Id, mappedArticleRevisionDto.Departments[i].Id);
                        Assert.AreEqual(articleRevisionDto.Departments[i].Name, mappedArticleRevisionDto.Departments[i].Name);
                        Assert.AreEqual(articleRevisionDto.Departments[i].ShortName, mappedArticleRevisionDto.Departments[i].ShortName);
                    }
                }

                if (articleRevisionDto.Terms != null)
                {
                    Assert.AreEqual(relatedArticle.CurrentArticleRevision.Terms[0].Id, relatedArticlesDto.Term.Id);
                    Assert.AreEqual(relatedArticle.CurrentArticleRevision.Terms[0].Name, relatedArticlesDto.Term.Name);
                }
            }
        }
    }
}
