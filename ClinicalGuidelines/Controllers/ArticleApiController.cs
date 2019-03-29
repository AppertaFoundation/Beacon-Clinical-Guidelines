//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Helpers;
using ClinicalGuidelines.Models;
using Newtonsoft.Json;

namespace ClinicalGuidelines.Controllers
{
    [Authorize]
    [RoutePrefix("api/article")]
    public class ArticleApiController : ApiController
    {
        private readonly IClinicalGuidelinesAppContext _db = new ClinicalGuidelinesAppContext();
        private readonly IMappingHelper _mappingHelper = new MappingHelper();
        private readonly LogHelper _logHelper = new LogHelper();

        public ArticleApiController()
        {
        }

        public ArticleApiController(IClinicalGuidelinesAppContext context, IMappingHelper mappingHelper)
        {
            _db = context;
            _mappingHelper = mappingHelper;
        }

        private Term FindTermById(int id)
        {
            return _db.Terms.Find(id);
        }

        private Department FindDepartmentById(int id)
        {
            return _db.Departments.Find(id);
        }

        private Term FindOrCreateTermByName(string name)
        {
            var term = _db.Terms.SingleOrDefault(x => x.Name == name);
            return term ?? new Term() { Name = name };
        }

        [Route("get")]
        public IHttpActionResult GetArticles()
        {
            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Where(x => !x.Archived)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime);

            var articlesDto = new List<ArticleRevisionDto>();
            foreach (var article in articles)
            {
                var articleRevision = new ArticleRevisionDto()
                {

                    Id = article.Id,
                    _id = article._id,
                    RevisionId = article.CurrentArticleRevision.Id,
                    Body = article.CurrentArticleRevision.Body,
                    Structure = JsonConvert.DeserializeObject(article.CurrentArticleRevision.Structure),
                    SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                    Title = article.CurrentArticleRevision.Title,
                    RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                    Viewed = article.HitCount,
                    UserId = article.CurrentArticleRevision.UserId,
                    SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                    Template = article.CurrentArticleRevision.Template,
                    Files =
                        (article.CurrentArticleRevision.Files != null)
                            ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files)
                            : null,
                    Live = article.CurrentArticleRevision.Live,
                    Approval = article.CurrentArticleRevision.Approval,
                    ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                    Rejected = article.CurrentArticleRevision.Rejected,
                    Departments = new List<DepartmentDto>(),
                    ExpiryDateTime = article.CurrentArticleRevision.ExpiryDateTime
                };

                foreach (var department in article.CurrentArticleRevision.Departments)
                {
                    articleRevision.Departments.Add(new DepartmentDto()
                    {
                        Id = department.Id,
                        _id = department._id,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        ContainerId = department.ContainerId
                    });
                }

                articlesDto.Add(articleRevision);
            }

            return Ok(articlesDto);
        }

        private IQueryable<Article> GetFilterArticlesQuery(int containerId, int departmentId, int subSectionTabId)
        {
            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Terms);
            
            if (containerId != 0 && departmentId == 0)
            {
                articles = articles
                    .Where(x => x.CurrentArticleRevision.Departments.Any(y => y.ContainerId == containerId));
            }
            else if (containerId == 0 && departmentId != 0)
            {
                articles = articles
                    .Where(x => x.CurrentArticleRevision.Departments.Any(y => y.Id == departmentId));
            }
            else if (containerId != 0 && departmentId != 0)
            {
                articles = articles
                    .Where(x => x.CurrentArticleRevision.Departments.Any(y => y.ContainerId == containerId && y.Id == departmentId));
            }

            if (subSectionTabId != 0)
            {
                articles = articles.Where(x => x.CurrentArticleRevision.SubSectionTab.Id == subSectionTabId);
            }
            
            articles = articles
                .Where(x => x.CurrentArticleRevision.Departments.Any(y => !y.Archived))
                .Where(x => !x.CurrentArticleRevision.SubSectionTab.Archived);
                
            return articles;
        }

        [Route("get/{containerId:int}/{departmentId:int}/{subSectionTabId:int}/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult GetPagedArticles(int containerId, int departmentId, int subSectionTabId, int pageSize, int pageNumber)
        {
            var username = User.GetUsernameWithoutDomain();
            var user = _db.Users
                .Include(x => x.AdministationDepartments)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return Ok();
            }

            var departmentList = new List<int>();

            foreach (var administrationDepartment in user.AdministationDepartments)
            {
                if (!departmentList.Contains(administrationDepartment.Id))
                {
                    departmentList.Add(administrationDepartment.Id);
                }
            }

            var articles = GetFilterArticlesQuery(containerId, departmentId, subSectionTabId);

            articles = articles
                    .Where(x => x.CurrentArticleRevision.Departments.Any(y => departmentList.Contains(y.Id)));

            var totalCount = articles.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            articles = articles
                    .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);            

            var articlesDto = new List<ArticleRevisionDto>();
            foreach (var article in articles)
            {
                var articleRevision = new ArticleRevisionDto()
                {
                    Id = article.Id,
                    _id = article._id,
                    RevisionId = article.CurrentArticleRevision.Id,
                    Body = article.CurrentArticleRevision.Body,
                    Structure = JsonConvert.DeserializeObject(article.CurrentArticleRevision.Structure),
                    SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                    Title = article.CurrentArticleRevision.Title,
                    RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                    Viewed = article.HitCount,
                    UserId = article.CurrentArticleRevision.UserId,
                    SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                    Files =
                        (article.CurrentArticleRevision.Files != null)
                            ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files)
                            : null,
                    Live = article.CurrentArticleRevision.Live,
                    Terms = new List<TermDto>(),
                    Archived = article.Archived,
                    Approval = article.CurrentArticleRevision.Approval,
                    ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                    Rejected = article.CurrentArticleRevision.Rejected,
                    Departments = new List<DepartmentDto>(),
                    ExpiryDateTime = article.CurrentArticleRevision.ExpiryDateTime
                };

                foreach (var department in article.CurrentArticleRevision.Departments)
                {
                    articleRevision.Departments.Add(new DepartmentDto()
                    {
                        Id = department.Id,
                        _id = department._id,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        ContainerId = department.ContainerId
                    });
                }

                foreach (var term in article.CurrentArticleRevision.Terms)
                {
                    articleRevision.Terms.Add(new TermDto()
                    {
                        Id = term.Id,
                        Name = term.Name
                    });
                }

                articlesDto.Add(articleRevision);
            }

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Articles = articlesDto
                }
            );
        }

        [Route("get/live/{containerId:int}/{departmentId:int}/{subSectionTabId:int}/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult GetPagedLiveArticles(int containerId, int departmentId, int subSectionTabId, int pageSize, int pageNumber)
        {
            var articles = GetFilterArticlesQuery(containerId, departmentId, subSectionTabId);

            articles = articles
                    .Where(x => !x.Archived && x.CurrentArticleRevision.Live && x.CurrentArticleRevision.Departments.All(y => y.Live && !y.Archived));

            var totalCount = articles.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            if (subSectionTabId != 0 || departmentId != 0)
            {
                articles = articles
                       .OrderBy(x => x.CurrentArticleRevision.Title)
                       .Skip((pageNumber - 1) * pageSize)
                       .Take(pageSize);
            } else
            {
                articles = articles
                    .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }            

            var articlesDto = new List<ArticleRevisionDto>();
            foreach (var article in articles)
            {
                var articleRevision = new ArticleRevisionDto()
                {
                    Id = article.Id,
                    _id = article._id,
                    RevisionId = article.CurrentArticleRevision.Id,
                    Body = article.CurrentArticleRevision.Body,
                    Structure = JsonConvert.DeserializeObject(article.CurrentArticleRevision.Structure),
                    SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                    Title = article.CurrentArticleRevision.Title,
                    RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                    Viewed = article.HitCount,
                    UserId = article.CurrentArticleRevision.UserId,
                    SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                    Files =
                        (article.CurrentArticleRevision.Files != null)
                            ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files)
                            : null,
                    Live = article.CurrentArticleRevision.Live,
                    Terms = new List<TermDto>(),
                    Archived = article.Archived,
                    Approval = article.CurrentArticleRevision.Approval,
                    ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                    Rejected = article.CurrentArticleRevision.Rejected,
                    Departments = new List<DepartmentDto>(),
                    ExpiryDateTime = article.CurrentArticleRevision.ExpiryDateTime
                };

                foreach (var department in article.CurrentArticleRevision.Departments)
                {
                    articleRevision.Departments.Add(new DepartmentDto()
                    {
                        Id = department.Id,
                        _id = department._id,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        ContainerId = department.ContainerId
                    });
                }

                foreach (var term in article.CurrentArticleRevision.Terms)
                {
                    articleRevision.Terms.Add(new TermDto()
                    {
                        Id = term.Id,
                        Name = term.Name
                    });
                }

                articlesDto.Add(articleRevision);
            }

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Articles = articlesDto
                }
            );
        }

        [Route("get/{containerId:int}/{departmentId:int}/{subSectionTabId:int}/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedArticlesByKeyword(int containerId, int departmentId, int subSectionTabId, int pageSize, int pageNumber, string searchTerm)
        {
            var username = User.GetUsernameWithoutDomain();
            var user = _db.Users
                .Include(x => x.AdministationDepartments)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return Ok();
            }

            var departmentList = new List<int>();

            foreach (var administrationDepartment in user.AdministationDepartments)
            {
                if (!departmentList.Contains(administrationDepartment.Id))
                {
                    departmentList.Add(administrationDepartment.Id);
                }
            }

            var articles = GetFilterArticlesQuery(containerId, departmentId, subSectionTabId);

            articles = articles
                    .Where(x => x.CurrentArticleRevision.Departments.Any(y => departmentList.Contains(y.Id)))
                    .Where(x => x.CurrentArticleRevision.Title.Contains(searchTerm) || x.CurrentArticleRevision.SubSectionTab.Name.Contains(searchTerm));

            var totalCount = articles.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            articles = articles
                    .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);            

            var articlesDto = new List<ArticleRevisionDto>();
            foreach (var article in articles)
            {
                var articleRevision = new ArticleRevisionDto()
                {
                    Id = article.Id,
                    _id = article._id,
                    RevisionId = article.CurrentArticleRevision.Id,
                    Body = article.CurrentArticleRevision.Body,
                    Structure = JsonConvert.DeserializeObject(article.CurrentArticleRevision.Structure),
                    SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                    Title = article.CurrentArticleRevision.Title,
                    RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                    Viewed = article.HitCount,
                    UserId = article.CurrentArticleRevision.UserId,
                    SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                    Files =
                        (article.CurrentArticleRevision.Files != null)
                            ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files)
                            : null,
                    Live = article.CurrentArticleRevision.Live,
                    Terms = new List<TermDto>(),
                    Archived = article.Archived,
                    Approval = article.CurrentArticleRevision.Approval,
                    ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                    Rejected = article.CurrentArticleRevision.Rejected,
                    Departments = new List<DepartmentDto>(),
                    ExpiryDateTime = article.CurrentArticleRevision.ExpiryDateTime
                };

                foreach (var department in article.CurrentArticleRevision.Departments)
                {
                    articleRevision.Departments.Add(new DepartmentDto()
                    {
                        Id = department.Id,
                        _id = department._id,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        ContainerId = department.ContainerId
                    });
                }

                foreach (var term in article.CurrentArticleRevision.Terms)
                {
                    articleRevision.Terms.Add(new TermDto()
                    {
                        Id = term.Id,
                        Name = term.Name
                    });
                }

                articlesDto.Add(articleRevision);
            }

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Articles = articlesDto
                }
            );
        }

        [Route("get/live/{containerId:int}/{departmentId:int}/{subSectionTabId:int}/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedLiveArticlesByKeyword(int containerId, int departmentId, int subSectionTabId, int pageSize, int pageNumber, string searchTerm)
        {
            var articles = GetFilterArticlesQuery(containerId, departmentId, subSectionTabId);

            articles = articles
                    .Where(x => x.CurrentArticleRevision.Title.Contains(searchTerm) || x.CurrentArticleRevision.SubSectionTab.Name.Contains(searchTerm))
                    .Where(x => !x.Archived && x.CurrentArticleRevision.Live && x.CurrentArticleRevision.Departments.All(y => y.Live && !y.Archived));

            var totalCount = articles.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            if (subSectionTabId != 0 || departmentId != 0)
            {
                articles = articles
                       .OrderBy(x => x.CurrentArticleRevision.Title)
                       .Skip((pageNumber - 1) * pageSize)
                       .Take(pageSize);
            }
            else
            {
                articles = articles
                    .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }

            var articlesDto = new List<ArticleRevisionDto>();
            foreach (var article in articles)
            {
                var articleRevision = new ArticleRevisionDto()
                {
                    Id = article.Id,
                    _id = article._id,
                    RevisionId = article.CurrentArticleRevision.Id,
                    Body = article.CurrentArticleRevision.Body,
                    Structure = JsonConvert.DeserializeObject(article.CurrentArticleRevision.Structure),
                    SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                    Title = article.CurrentArticleRevision.Title,
                    RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                    Viewed = article.HitCount,
                    UserId = article.CurrentArticleRevision.UserId,
                    SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                    Files =
                        (article.CurrentArticleRevision.Files != null)
                            ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files)
                            : null,
                    Live = article.CurrentArticleRevision.Live,
                    Terms = new List<TermDto>(),
                    Archived = article.Archived,
                    Approval = article.CurrentArticleRevision.Approval,
                    ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                    Rejected = article.CurrentArticleRevision.Rejected,
                    Departments = new List<DepartmentDto>(),
                    ExpiryDateTime = article.CurrentArticleRevision.ExpiryDateTime
                };

                foreach (var department in article.CurrentArticleRevision.Departments)
                {
                    articleRevision.Departments.Add(new DepartmentDto()
                    {
                        Id = department.Id,
                        _id = department._id,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        ContainerId = department.ContainerId
                    });
                }

                foreach (var term in article.CurrentArticleRevision.Terms)
                {
                    articleRevision.Terms.Add(new TermDto()
                    {
                        Id = term.Id,
                        Name = term.Name
                    });
                }

                articlesDto.Add(articleRevision);
            }

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Articles = articlesDto
                }
            );
        }

        [Route("approvals/get/{pageSize:int}/{pageNumber:int}/")]
        public IHttpActionResult GetPagedArticleApprovals(int pageSize, int pageNumber)
        {
            var username = User.GetUsernameWithoutDomain();
            var user = _db.Users
                .Include(x => x.AdministationDepartments)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return Ok();
            }

            var departmentList = new List<int>();

            foreach (var administrationDepartment in user.AdministationDepartments)
            {
                if (!departmentList.Contains(administrationDepartment.Id))
                {
                    departmentList.Add(administrationDepartment.Id);
                }
            }

            var approvalsAwaiting = _db.ArticleRevisions
                .Include(x => x.Departments)
                .Join(
                    _db.Articles,
                    ar => ar.ArticleId,
                    a => a.Id,
                    (ar, a) => new { NewArticleRevision = ar, Article = a, ar.Departments }
                )
                .Join(
                    _db.SubSectionTabs,
                    arSubId => arSubId.NewArticleRevision.SubSectionTabId,
                    sub => sub.Id,
                    (arSubId, sub) => new { arSubId.NewArticleRevision, arSubId.Article, arSubId.Departments, SubSectionTab = sub }
                )
                .Where(x => x.NewArticleRevision.Approval)
                .Where(x => x.Departments.Any(y => departmentList.Contains(y.Id)))
                .OrderBy(x => x.NewArticleRevision.RevisionDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var totalCount = _db.ArticleRevisions.Count(x => x.Approval);
            var totalPages = Math.Ceiling((double)totalCount / pageSize);            

            var articlesDto = new List<ArticleRevisionDto>();
            foreach (var article in approvalsAwaiting)
            {
                var articleRevision = new ArticleRevisionDto()
                {
                    Id = article.Article.Id,
                    _id = article.Article._id,
                    RevisionId = article.NewArticleRevision.Id,
                    Body = article.NewArticleRevision.Body,
                    Structure = JsonConvert.DeserializeObject(article.NewArticleRevision.Structure),
                    SubTitle = article.SubSectionTab.Name,
                    Title = article.NewArticleRevision.Title,
                    RevisionDateTime = article.NewArticleRevision.RevisionDateTime,
                    Viewed = article.Article.HitCount,
                    UserId = article.NewArticleRevision.UserId,
                    SubSectionTabId = article.SubSectionTab.Id,
                    Files =
                        (article.NewArticleRevision.Files != null)
                            ? JsonConvert.DeserializeObject(article.NewArticleRevision.Files)
                            : null,
                    Live = article.NewArticleRevision.Live,
                    Archived = article.Article.Archived,
                    Approval = article.NewArticleRevision.Approval,
                    ApprovalComments = article.NewArticleRevision.ApprovalComments,
                    Rejected = article.NewArticleRevision.Rejected,
                    Departments = new List<DepartmentDto>(),
                    ShowGallery = article.NewArticleRevision.ShowGallery,
                    ExpiryDateTime = article.NewArticleRevision.ExpiryDateTime
                };

                foreach (var department in article.Departments)
                {
                    articleRevision.Departments.Add(new DepartmentDto()
                    {
                        Id = department.Id,
                        _id = department._id,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        ContainerId = department.ContainerId
                    });
                }

                articlesDto.Add(articleRevision);
            }

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Articles = articlesDto
                }
            );
        }

        [Route("approvals/get/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedArticleApprovalsByKeyword(int pageSize, int pageNumber, string searchTerm)
        {
            var username = User.GetUsernameWithoutDomain();
            var user = _db.Users
                .Include(x => x.AdministationDepartments)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return Ok();
            }

            var departmentList = new List<int>();

            foreach (var administrationDepartment in user.AdministationDepartments)
            {
                if (!departmentList.Contains(administrationDepartment.Id))
                {
                    departmentList.Add(administrationDepartment.Id);
                }
            }

            var approvalsAwaiting = _db.ArticleRevisions
                .Include(x => x.Departments)
                .Join(
                    _db.Articles,
                    ar => ar.ArticleId,
                    a => a.Id,
                    (ar, a) => new { NewArticleRevision = ar, Article = a, ar.Departments }
                )
                .Join(
                    _db.SubSectionTabs,
                    arSubId => arSubId.NewArticleRevision.SubSectionTabId,
                    sub => sub.Id,
                    (arSubId, sub) => new { arSubId.NewArticleRevision, arSubId.Article, arSubId.Departments, SubSectionTab = sub }
                )
                .Where(x => x.NewArticleRevision.Approval)
                .Where(x => x.Departments.Any(y => departmentList.Contains(y.Id)))
                .Where(x =>
                    x.NewArticleRevision.Title.Contains(searchTerm) ||
                    x.SubSectionTab.Name.Contains(searchTerm) ||
                    x.Departments.Any(y =>
                        y.ShortName.Contains(searchTerm) ||
                        y.Name.Contains(searchTerm)
                    )
                )
                .OrderBy(x => x.NewArticleRevision.RevisionDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var totalCount = _db.ArticleRevisions
                .Count(x => 
                    x.Approval && 
                    (
                        x.Title.Contains(searchTerm) ||
                        x.SubSectionTab.Name.Contains(searchTerm) ||
                        x.Departments.Any(y => 
                            y.ShortName.Contains(searchTerm) ||
                            y.Name.Contains(searchTerm)
                        )
                    )
                );
            var totalPages = Math.Ceiling((double)totalCount / pageSize);            

            var articlesDto = new List<ArticleRevisionDto>();
            foreach (var article in approvalsAwaiting)
            {
                var articleRevision = new ArticleRevisionDto()
                {
                    Id = article.Article.Id,
                    _id = article.Article._id,
                    RevisionId = article.NewArticleRevision.Id,
                    Body = article.NewArticleRevision.Body,
                    Structure = JsonConvert.DeserializeObject(article.NewArticleRevision.Structure),
                    SubTitle = article.SubSectionTab.Name,
                    Title = article.NewArticleRevision.Title,
                    RevisionDateTime = article.NewArticleRevision.RevisionDateTime,
                    Viewed = article.Article.HitCount,
                    UserId = article.NewArticleRevision.UserId,
                    SubSectionTabId = article.SubSectionTab.Id,
                    Files =
                        (article.NewArticleRevision.Files != null)
                            ? JsonConvert.DeserializeObject(article.NewArticleRevision.Files)
                            : null,
                    Live = article.NewArticleRevision.Live,
                    Archived = article.Article.Archived,
                    Approval = article.NewArticleRevision.Approval,
                    ApprovalComments = article.NewArticleRevision.ApprovalComments,
                    Rejected = article.NewArticleRevision.Rejected,
                    Departments = new List<DepartmentDto>(),
                    ShowGallery = article.NewArticleRevision.ShowGallery,
                    ExpiryDateTime = article.NewArticleRevision.ExpiryDateTime
                };

                foreach (var department in article.Departments)
                {
                    articleRevision.Departments.Add(new DepartmentDto()
                    {
                        Id = department.Id,
                        _id = department._id,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        ContainerId = department.ContainerId
                    });
                }

                articlesDto.Add(articleRevision);
            }

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Articles = articlesDto
                }
            );
        }

        [Route("get/{id:int}/history")]
        public IHttpActionResult GetArticleHistory(int id)
        {
            var articleRevisions = _db.ArticleRevisions
                .Join(
                    _db.Users,
                    ar => ar.UserId,
                    u => u.Id,
                    (ar, u) => new {ArticleRevision = ar, User = u}
                )
                .Where(x => x.ArticleRevision.ArticleId == id)
                .OrderBy(x => x.ArticleRevision.RevisionDateTime); 

            var articleRevisionHistoryDto = articleRevisions
                .Select(articleRevision => new ArticleRevisionHistoryDto()
                    {
                        Id = articleRevision.ArticleRevision.Id,
                        Live = articleRevision.ArticleRevision.Live,
                        RevisionDateTime = articleRevision.ArticleRevision.RevisionDateTime,
                        User = articleRevision.User.Forename + " " + articleRevision.User.Surname + ", " + articleRevision.User.UserName
                })
                .ToList();

            return Ok(articleRevisionHistoryDto);
        }

        [Route("get/{id:int}")]
        [ResponseType(typeof(Article))]
        public IHttpActionResult GetArticle(int id)
        {
            var article = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Terms)
                .SingleOrDefault(x => x.Id == id && !x.Archived);

            if (article == null)
            {
                return NotFound();
            }

            var articlesDto = new ArticleRevisionDto()
            {
                Id = article.Id,
                _id = article._id,
                RevisionId = article.CurrentArticleRevision.Id,
                Body = article.CurrentArticleRevision.Body,
                Structure = JsonConvert.DeserializeObject(article.CurrentArticleRevision.Structure),
                SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                Title = article.CurrentArticleRevision.Title,
                RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                Viewed = article.HitCount,
                UserId = article.CurrentArticleRevision.UserId,
                SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                Template = article.CurrentArticleRevision.Template,
                Terms = new List<TermDto>(),
                Files = (article.CurrentArticleRevision.Files != null) ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files) : null,
                Live = article.CurrentArticleRevision.Live,
                Approval = article.CurrentArticleRevision.Approval,
                ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                Rejected = article.CurrentArticleRevision.Rejected,
                Departments = new List<DepartmentDto>(),
                ShowGallery = article.CurrentArticleRevision.ShowGallery,
                ExpiryDateTime = article.CurrentArticleRevision.ExpiryDateTime
            };

            foreach (var department in article.CurrentArticleRevision.Departments)
            {
                articlesDto.Departments.Add(new DepartmentDto()
                {
                    Id = department.Id,
                    _id = department._id,
                    Name = department.Name,
                    ShortName = department.ShortName,
                    ContainerId = department.ContainerId
                });
            }

            foreach (var term in article.CurrentArticleRevision.Terms)
            {
                articlesDto.Terms.Add(new TermDto()
                {
                    Id = term.Id,
                    Name = term.Name
                });
            }

            return Ok(_mappingHelper.MapRelatedArticleToArticleRevisionDto(articlesDto, _db));
        }

        [Route("get/{id:int}/revision/{revisionId:int}")]
        [ResponseType(typeof(Article))]
        public IHttpActionResult GetArticleRevision(int id, int revisionId)
        {
            var article = _db.Articles
                .SingleOrDefault(x => x.Id == id && !x.Archived);

            var revision = _db.ArticleRevisions
                .Include(x => x.Departments)
                .Include(x => x.SubSectionTab)
                .Include(x => x.Terms)
                .SingleOrDefault(x => x.Id == revisionId);

            if (article == null || revision == null)
            {
                return NotFound();
            }

            article.CurrentArticleRevision = revision;

            var articlesDto = new ArticleRevisionDto()
            {
                Id = article.Id,
                _id = article._id,
                RevisionId = article.CurrentArticleRevision.Id,
                Body = article.CurrentArticleRevision.Body,
                Structure = JsonConvert.DeserializeObject(article.CurrentArticleRevision.Structure),
                SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                Title = article.CurrentArticleRevision.Title,
                RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                Viewed = article.HitCount,
                UserId = article.CurrentArticleRevision.UserId,
                SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                Template = article.CurrentArticleRevision.Template,
                Terms = new List<TermDto>(),
                Files = (article.CurrentArticleRevision.Files != null) ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files) : null,
                Live = article.CurrentArticleRevision.Live,
                Approval = article.CurrentArticleRevision.Approval,
                ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                Rejected = article.CurrentArticleRevision.Rejected,
                Departments = new List<DepartmentDto>(),
                ShowGallery = article.CurrentArticleRevision.ShowGallery,
                ExpiryDateTime = article.CurrentArticleRevision.ExpiryDateTime
            };

            foreach (var department in article.CurrentArticleRevision.Departments)
            {
                articlesDto.Departments.Add(new DepartmentDto()
                {
                    Id = department.Id,
                    _id = department._id,
                    Name = department.Name,
                    ShortName = department.ShortName,
                    ContainerId = department.ContainerId
                });
            }

            foreach (var term in article.CurrentArticleRevision.Terms)
            {
                articlesDto.Terms.Add(new TermDto()
                {
                    Id = term.Id,
                    Name = term.Name
                });
            }

            return Ok(_mappingHelper.MapRelatedArticleToArticleRevisionDto(articlesDto, _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/app")]
        [ResponseType(typeof(Article))]
        public IHttpActionResult GetArticleAppSide(int id)
        {
            var article = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Terms)
                .SingleOrDefault(x => 
                    x.Id == id && 
                    x.CurrentArticleRevision.Live && 
                    !x.Archived &&
                    !x.CurrentArticleRevision.SubSectionTab.Archived);

            if (article == null)
            {
                return NotFound();
            }

            var articlesDto = new ArticleRevisionDto()
            {
                Id = article.Id,
                _id = article._id,
                RevisionId = article.CurrentArticleRevision.Id,
                Body = article.CurrentArticleRevision.Body,
                SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                Title = article.CurrentArticleRevision.Title,
                RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                SubSectionTabId = article.CurrentArticleRevision.SubSectionTabId,
                Template = article.CurrentArticleRevision.Template,
                Terms = new List<TermDto>(),
                Files = (article.CurrentArticleRevision.Files != null) ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files) : null,
                Live = article.CurrentArticleRevision.Live,
                Approval = article.CurrentArticleRevision.Approval,
                ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                Rejected = article.CurrentArticleRevision.Rejected,
                Departments = new List<DepartmentDto>(),
                ShowGallery = article.CurrentArticleRevision.ShowGallery,
                ExpiryDateTime = article.CurrentArticleRevision.ExpiryDateTime
            };

            foreach (var department in article.CurrentArticleRevision.Departments)
            {
                articlesDto.Departments.Add(new DepartmentDto()
                {
                    Id = department.Id,
                    _id = department._id,
                    Name = department.Name,
                    ShortName = department.ShortName,
                    ContainerId = department.ContainerId
                });
            }

            foreach (var term in article.CurrentArticleRevision.Terms)
            {
                articlesDto.Terms.Add(new TermDto()
                {
                    Id = term.Id,
                    Name = term.Name
                });
            }

            return Ok(_mappingHelper.MapRelatedArticleToArticleRevisionDto(articlesDto, _db));
        }

        [AllowAnonymous]
        [Route("put/{id:int}/hit")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAddToHitCountForArticle(int id)
        {
            var article = _db.Articles.Find(id);

            if (article == null)
            {
                return NotFound();
            }

            article.HitCount++;

            _db.MarkArticleAsModified(article);
            _db.SaveChanges();

            return Ok();
        }

        [Route("put/{id:int}")]
        [ResponseType(typeof(void))]
        /*
         * Edit the current revision 
         */
        public IHttpActionResult PutArticle(int id, ArticleRevisionDto article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

            if (!IsAllowedToEdit())
            {
                return Unauthorized();
            }

            //If this is a specific revision load that to apply update otherwise load the article including the current live revision
            if (article.RevisionId != 0)
            {
                //Find the article revision
                var thisArticleRevision = _db.ArticleRevisions
                    .Include(x => x.Terms)
                    .Include(x => x.Departments)
                    .SingleOrDefault(x => x.Id == article.RevisionId);
                if (thisArticleRevision == null)
                {
                    return BadRequest();
                }

                var live = (IsContentEditor()) ? false : article.Live;

                thisArticleRevision.Title = article.Title;
                thisArticleRevision.SubSectionTabId = article.SubSectionTabId;
                thisArticleRevision.Body = article.Body;
                thisArticleRevision.Structure = JsonConvert.SerializeObject(article.Structure);
                thisArticleRevision.Live = live;
                thisArticleRevision.Public = true;
                thisArticleRevision.RevisionDateTime = DateTime.Now;
                thisArticleRevision.UserId = article.UserId;
                thisArticleRevision.ArticleId = article.Id;
                thisArticleRevision.Template = article.Template;
                thisArticleRevision.Terms = new List<Term>();
                thisArticleRevision.Files = JsonConvert.SerializeObject(article.Files);
                thisArticleRevision.Approval = article.Approval;
                thisArticleRevision.ApprovalComments = (article.Approval) ? null : article.ApprovalComments;
                thisArticleRevision.Rejected = (article.Approval) ? false : article.Rejected;
                thisArticleRevision.Departments = new List<Department>();
                thisArticleRevision.ShowGallery = article.ShowGallery;
                thisArticleRevision.ExpiryDateTime = article.ExpiryDateTime;

                // Ensure the last edits are transferred to new revision so they can be highlighted on client
                var revisionId = thisArticleRevision.Id.ToString();
                var structure = thisArticleRevision.Structure;
                // Change the revisionEditedId on edited to blocks to new revision id
                structure = structure.Replace("-1", revisionId);
                thisArticleRevision.Structure = structure;

                foreach (var department in article.Departments.Select(departmentDto => FindDepartmentById(departmentDto.Id)))
                {
                    thisArticleRevision.Departments.Add(department);
                }

                foreach (var term in article.Terms.Select(
                        termDto => (termDto.Id != 0)
                            ? FindTermById(termDto.Id)
                            : FindOrCreateTermByName(termDto.Name)
                    )
                )
                {
                    thisArticleRevision.Terms.Add(term);
                }

                if (live)
                {
                    //If the new revision is being made live unpublish the current live one
                    var liveRevisions = _db.ArticleRevisions.Where(x => x.ArticleId == id && x.Live && x.Id != thisArticleRevision.Id);

                    foreach (var liveRevision in liveRevisions)
                    {
                        liveRevision.Live = false;
                    }
                }

                //If the new revision is being made live unpublish the current live one
                var currentLiveRevision = _db.Articles.Include(x => x.CurrentArticleRevision).SingleOrDefault(x => x.Id == id);
                if (live)
                {
                    if (currentLiveRevision != null && currentLiveRevision.ArticleRevisionId != article.RevisionId) currentLiveRevision.ArticleRevisionId = thisArticleRevision.Id;
                }

                _db.Logs.Add(
                    _logHelper.CreateArticleApiControllerLog(
                        LogHelper.LogType.Update,
                        "ArticleApiController.PutArticle->NewRevision",
                        $"api/article/put/{id}",
                        article,
                        User.GetUsernameWithoutDomain()
                    )
                );
            }
            else
            {
                //Find the article
                var currentArticle = _db.Articles
                    .Include(x => x.CurrentArticleRevision)
                    .Include(x => x.CurrentArticleRevision.Departments)
                    .Include(x => x.CurrentArticleRevision.Terms)
                    .SingleOrDefault(x => x.Id == id);
                if (currentArticle == null)
                {
                    return BadRequest();
                }

                currentArticle.CurrentArticleRevision.Title = article.Title;
                currentArticle.CurrentArticleRevision.SubSectionTabId = article.SubSectionTabId;
                currentArticle.CurrentArticleRevision.Body = article.Body;
                currentArticle.CurrentArticleRevision.Structure = JsonConvert.SerializeObject(article.Structure);
                currentArticle.CurrentArticleRevision.Live = (IsContentEditor()) ? false : article.Live;
                currentArticle.CurrentArticleRevision.Public = true;
                currentArticle.CurrentArticleRevision.RevisionDateTime = DateTime.Now;
                currentArticle.CurrentArticleRevision.UserId = article.UserId;
                currentArticle.CurrentArticleRevision.ArticleId = article.Id;
                currentArticle.CurrentArticleRevision.Template = article.Template;
                currentArticle.CurrentArticleRevision.Terms = new List<Term>();
                currentArticle.CurrentArticleRevision.Files = JsonConvert.SerializeObject(article.Files);
                currentArticle.CurrentArticleRevision.Approval = article.Approval;
                currentArticle.CurrentArticleRevision.ApprovalComments = (article.Approval) ? null : article.ApprovalComments;
                currentArticle.CurrentArticleRevision.Rejected = (article.Approval) ? false : article.Rejected;
                currentArticle.CurrentArticleRevision.Departments = new List<Department>();
                currentArticle.CurrentArticleRevision.ShowGallery = article.ShowGallery;
                currentArticle.CurrentArticleRevision.ExpiryDateTime = article.ExpiryDateTime;

                // Ensure the last edits are transferred to new revision so they can be highlighted on client
                var revisionId = currentArticle.CurrentArticleRevision.Id;
                var structure = currentArticle.CurrentArticleRevision.Structure;
                // Change the revisionEditedId on edited to blocks to new revision id
                structure = structure.Replace("-1", revisionId.ToString());
                currentArticle.CurrentArticleRevision.Structure = structure;

                foreach (var department in article.Departments.Select(departmentDto => FindDepartmentById(departmentDto.Id)))
                {
                    currentArticle.CurrentArticleRevision.Departments.Add(department);
                }

                foreach (var term in article.Terms.Select(
                        termDto => (termDto.Id != 0)
                            ? FindTermById(termDto.Id)
                            : FindOrCreateTermByName(termDto.Name)
                    )
                )
                {
                    currentArticle.CurrentArticleRevision.Terms.Add(term);
                }

                if (currentArticle._id == Guid.Empty) currentArticle._id = Guid.NewGuid();

                _db.Logs.Add(
                    _logHelper.CreateArticleApiControllerLog(
                        LogHelper.LogType.Update,
                        "ArticleApiController.PutArticle->EditExistingRevision",
                        $"api/article/put/{id}",
                        article,
                        User.GetUsernameWithoutDomain()
                    )
                );
            }

            try
            {
                _db.SaveChanges();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("put/{id:int}/revision")]
        [ResponseType(typeof(void))]
        /*
         * Create a new revision
         */
        public IHttpActionResult PutArticleRevision(int id, ArticleRevisionDto article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

            if (!IsAllowedToEdit())
            {
                return Unauthorized();
            }

            // Find the article
            var currentArticle = _db.Articles.SingleOrDefault(x => x.Id == id);
            if (currentArticle == null)
            {
                return BadRequest();
            }

            var live = (IsContentEditor()) ? false : article.Live;

            // If the new revision is being made live unpublish the current live one
            if (live)
            {
                var currentRevision = _db.ArticleRevisions.Find(currentArticle.ArticleRevisionId);
                currentRevision.Live = false;
            }

            // Create a new revision using new details
            var newRevision = new ArticleRevision()
            {
                Title = article.Title,
                SubSectionTabId = article.SubSectionTabId,
                Body = article.Body,
                Structure = JsonConvert.SerializeObject(article.Structure),
                Live = live,
                Public = true,
                RevisionDateTime = DateTime.Now,
                UserId = article.UserId,
                ArticleId = article.Id,
                Template = article.Template,
                Terms = new List<Term>(),
                Files = JsonConvert.SerializeObject(article.Files),
                Approval = article.Approval,
                ApprovalComments = (article.Approval) ? null : article.ApprovalComments,
                Rejected = (article.Approval) ? false : article.Rejected,
                Departments = new List<Department>(),
                ShowGallery = article.ShowGallery,
                ExpiryDateTime = article.ExpiryDateTime
            };

            foreach (var department in article.Departments)
            {
                newRevision.Departments.Add(FindDepartmentById(department.Id));
            }

            foreach (var term in article.Terms.Select(
                    termDto => (termDto.Id != 0)
                        ? FindTermById(termDto.Id)
                        : FindOrCreateTermByName(termDto.Name)
                )
            )
            {
                newRevision.Terms.Add(term);
            }

            // Add the new revision
            _db.ArticleRevisions.Add(newRevision);

            // If the new revision is being made live set it as the revision that the main article points to
            if (live)
            {
                currentArticle.ArticleRevisionId = newRevision.Id;
            }

            if (currentArticle._id == Guid.Empty) currentArticle._id = Guid.NewGuid();

            _db.Logs.Add(
                _logHelper.CreateArticleApiControllerLog(
                    LogHelper.LogType.Update,
                    "ArticleApiController.PutArticleRevision",
                    $"api/article/put/{id}/revision",
                    article,
                    User.GetUsernameWithoutDomain()
                )
            );

            try
            {
                _db.SaveChanges();
                // Ensure the last edits are transferred to new revision so they can be highlighted on client
                var newRevisionId = newRevision.Id.ToString();
                var structure = JsonConvert.SerializeObject(article.Structure);
                // Change the revisionEditedId on edited to blocks to new revision id
                structure = structure.Replace("-1", newRevisionId);
                newRevision.Structure = structure;

                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("approve/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutApproveRevision(int id, ArticleRevisionDto article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

            if (!IsPrivaledgedUser())
            {
                return Unauthorized();
            }

            //Find the article
            var currentArticle = _db.Articles.SingleOrDefault(x => x.Id == id);
            if (currentArticle == null)
            {
                return BadRequest();
            }

            //Approving the content so we unpublish the current revision
            var currentRevision = _db.ArticleRevisions.Find(currentArticle.ArticleRevisionId);
            currentRevision.Live = false;

            //Find the article revision
            var thisArticleRevision = _db.ArticleRevisions
                .Include(x => x.Terms)
                .SingleOrDefault(x => x.Id == article.RevisionId);

            if (thisArticleRevision == null)
            {
                return BadRequest();
            }

            thisArticleRevision.Live = true;
            thisArticleRevision.Approval = false;
            thisArticleRevision.ApprovalDateTime = DateTime.Now;
            thisArticleRevision.ApprovalComments = article.ApprovalComments;
            thisArticleRevision.ApprovalBy = User.GetUsernameWithoutDomain();
            thisArticleRevision.Rejected = false;

            currentArticle.ArticleRevisionId = thisArticleRevision.Id;

            _db.Logs.Add(
                _logHelper.CreateArticleApiControllerLog(
                    LogHelper.LogType.Update,
                    "ArticleApiController.PutApproveRevision",
                    $"api/article/approve/{id}",
                    article,
                    User.GetUsernameWithoutDomain()
                )
            );

            var authorName = _db.Users.FirstOrDefault(a => a.Id == thisArticleRevision.UserId).UserName;

            EmailHelper.SendApprovalEmail(thisArticleRevision);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("reject/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRejectRevision(int id, ArticleRevisionDto article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

            if (!IsPrivaledgedUser())
            {
                return Unauthorized();
            }

            //Find the article
            var currentArticle = _db.Articles.SingleOrDefault(x => x.Id == id);
            if (currentArticle == null)
            {
                return BadRequest();
            }

            //Find the article revision
            var thisArticleRevision = _db.ArticleRevisions
                .Include(x => x.Terms)
                .SingleOrDefault(x => x.Id == article.RevisionId);
            if (thisArticleRevision == null)
            {
                return BadRequest();
            }

            thisArticleRevision.Approval = false;
            thisArticleRevision.ApprovalComments = article.ApprovalComments;
            thisArticleRevision.ApprovalBy = User.GetUsernameWithoutDomain();
            thisArticleRevision.Rejected = true;

            var approverUsername = User.GetUsernameWithoutDomain();
            var author = _db.Users.FirstOrDefault(a => a.Id == thisArticleRevision.UserId);
            var approver = _db.Users.FirstOrDefault(ap => ap.UserName == approverUsername);
            // Create email message using stringbuilder
            var sb = new StringBuilder();
            sb.AppendLine($"Dear {author.Forename} {author.Surname}, ");
            sb.AppendLine();
            sb.AppendLine($"I have rejected article - {thisArticleRevision.Title}, please see my comments below.");
            sb.AppendLine();
            sb.AppendLine($"{thisArticleRevision.ApprovalComments}");
            sb.AppendLine();
            sb.AppendLine($"Kind Regards, {approver.Forename} {approver.Surname}");

            var approvalMessage = sb.ToString();
            var approvalSubject = $"{thisArticleRevision.Title} has been approved.";

            EmailHelper.SendMail(author.EmailAddress, approver.EmailAddress, approvalSubject, approvalMessage);

            _db.Logs.Add(
                _logHelper.CreateArticleApiControllerLog(
                    LogHelper.LogType.Update,
                    "ArticleApiController.PutRejectRevision",
                    $"api/article/reject/{id}",
                    article,
                    User.GetUsernameWithoutDomain()
                )
            );

            var authorName = _db.Users.FirstOrDefault(a => a.Id == thisArticleRevision.UserId).UserName;

            //if (!Equals(authorName.ToUpper(), thisArticleRevision.ApprovalBy.ToUpper()))
            EmailHelper.SendRejectedEmail(thisArticleRevision);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        [Route("post")]
        public IHttpActionResult PostNewArticle(ArticleRevisionDto article)
        {
            if (!IsAllowedToEdit())
            {
                return Unauthorized();
            }

            var newRevision = new ArticleRevision()
            {
                Title = article.Title,
                SubSectionTabId = article.SubSectionTabId,
                Body = article.Body,
                Structure = JsonConvert.SerializeObject(article.Structure),
                Live = (IsContentEditor()) ? false : article.Live,
                Public = true,
                RevisionDateTime = DateTime.Now,
                UserId = article.UserId,
                Template = article.Template,
                Terms = new List<Term>(),
                Files = JsonConvert.SerializeObject(article.Files),
                Approval = article.Approval,
                ApprovalComments = article.ApprovalComments,
                Rejected = article.Rejected,
                Departments = new List<Department>(),
                ShowGallery = article.ShowGallery,
                ExpiryDateTime = article.ExpiryDateTime
            };

            foreach (var department in article.Departments)
            {
                newRevision.Departments.Add(FindDepartmentById(department.Id));
            }

            foreach (var termDto in article.Terms)
            {
                newRevision.Terms.Add(FindOrCreateTermByName(termDto.Name));
            }

            //Add the new revision
            _db.ArticleRevisions.Add(newRevision);
            _db.SaveChanges();

            var newArticle = new Article()
            {
                ArticleRevisionId = newRevision.Id,
                HitCount = 0,
                _id = Guid.NewGuid()
            };

            _db.Articles.Add(newArticle);
            _db.SaveChanges();

            newRevision.ArticleId = newArticle.Id;

            article.Id = newArticle.Id;
            article._id = newArticle._id;
            article.RevisionId = newArticle.ArticleRevisionId;

            _db.Logs.Add(
                _logHelper.CreateArticleApiControllerLog(
                    LogHelper.LogType.Create,
                    "ArticleApiController.PostNewArticle",
                    "api/article/post/",
                    article,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok();
        }

        [Route("archive/{id:int}")]
        [ResponseType(typeof(Article))]
        public IHttpActionResult PutArchiveArticle(int id)
        {
            if (!ArticleExists(id)) return NotFound();

            if (!IsPrivaledgedUser())
            {
                return Unauthorized();
            }

            var article = _db.Articles.Find(id);

            article.Archived = true;
            article.ArchiveDateTime = DateTime.Now;

            _db.Logs.Add(
                _logHelper.CreateArticleApiControllerArchiveLog(
                    LogHelper.LogType.Archive,
                    "ArticleApiController.PutArchiveArticle",
                    $"api/article/archive/{id}",
                    article,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkArticleAsModified(article);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("unarchive/{id:int}")]
        [ResponseType(typeof(Article))]
        public IHttpActionResult PutUnArchiveArticle(int id)
        {
            if (!ArticleExists(id)) return NotFound();

            if (!IsPrivaledgedUser())
            {
                return Unauthorized();
            }

            var article = _db.Articles.Find(id);

            article.Archived = false;
            article.ArchiveDateTime = null;

            _db.Logs.Add(
                _logHelper.CreateArticleApiControllerArchiveLog(
                    LogHelper.LogType.Archive,
                    "ArticleApiController.PutUnArchiveArticle",
                    $"api/article/unarchive/{id}",
                    article,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkArticleAsModified(article);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool IsPrivaledgedUser()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && (x.SiteAdmin || x.LeadClinician || x.ContentApprover)) > 0; 
        }

        private bool IsAllowedToEdit()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && (x.ContentEditor || x.SiteAdmin || x.LeadClinician || x.ContentApprover)) > 0; 
        }

        private bool IsContentEditor()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && (x.ContentEditor && !x.SiteAdmin && !x.LeadClinician && !x.ContentApprover)) > 0; 
        }

        private bool ArticleExists(int id)
        {
            return _db.Articles.Count(e => e.Id == id) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}