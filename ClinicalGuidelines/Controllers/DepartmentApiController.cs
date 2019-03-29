//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Helpers;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.Controllers
{
    [Authorize]
    [RoutePrefix("api/department")]
    public class DepartmentApiController : ApiController
    {
        private readonly IClinicalGuidelinesAppContext _db = new ClinicalGuidelinesAppContext();
        private readonly IMappingHelper _mappingHelper = new MappingHelper();
        private readonly LogHelper _logHelper = new LogHelper();

        private const int ArticlesLoadAmount = 7;
        private const int ContactsLoadAmount = 10;
        private const int InifiniteLoadAmount = 2;

        public DepartmentApiController()
        {
        }

        public DepartmentApiController(IClinicalGuidelinesAppContext context, IMappingHelper mappingHelper)
        {
            _db = context;
            _mappingHelper = mappingHelper;
        }

        [AllowAnonymous]
        [Route("get")]
        public IHttpActionResult GetDepartments()
        {
            return Ok(_db.Departments.Where(x => x.Container == false && x.Live && !x.Archived).ToList());
        }

        [Route("get/editable")]
        public IHttpActionResult GetEditableDepartments()
        {
            var username = User.GetUsernameWithoutDomain();
            var user = _db.Users
                .Include(x => x.AdministationDepartments)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return Ok();
            }

            var departmentDtos = user.AdministationDepartments
                .Select(x => new DepartmentDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    ContainerId = x.ContainerId
                });
            return Ok(departmentDtos);
        }

        [AllowAnonymous]
        [Route("get/all")]
        public IQueryable<Department> GetAllDepartments()
        {
            return _db.Departments.Where(x => x.Live && !x.Archived);
        }

        [Route("get/containers")]
        public IQueryable<Department> GetDepartmentContainers()
        {
            return _db.Departments.Where(x => x.Container && x.Live && !x.Archived);
        }

        [Route("get/containers/all")]
        public IQueryable<Department> GetAllDepartmentContainers()
        {
            return _db.Departments.Where(x => x.Container);
        }

        [Route("get/editable/containers")]
        public IHttpActionResult GetEditableDepartmentContainers()
        {
            var username = User.GetUsernameWithoutDomain();
            var user = _db.Users
                .Include(x => x.AdministationDepartments)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return Ok();
            }            

            var containerList = new List<int>();

            foreach (var administrationDepartment in user.AdministationDepartments)
            {
                if (administrationDepartment.ContainerId == null) continue;

                var containerId = (int) administrationDepartment.ContainerId;
                if (!containerList.Contains(containerId))
                {
                    containerList.Add(containerId);
                }
            }

            var containers = _db.Departments.Where(x => containerList.Contains(x.Id));

            return Ok(containers);
        }

        [Route("get/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult GetPagedDepartments(int pageSize, int pageNumber)
        {
            var totalCount = _db.Departments.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var departments = _db.Departments
                .OrderByDescending(x => x.Container)
                .ThenBy(x=> x.ContainerId)
                .ThenBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Departments = departments
                }
            );
        }


        [Route("container/{containerId:int}/get")]
        public IQueryable<Department> GetContainerDepartments(int containerId)
        {
            return _db.Departments.Where(x => !x.Container && x.ContainerId == containerId && x.Live && !x.Archived);
        }

        [Route("get/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedDepartmentsByKeyword(int pageSize, int pageNumber, string searchTerm)
        {
            var totalCount = _db.Departments.Count(x =>
                x.Name.Contains(searchTerm) ||
                x.ShortName.Contains(searchTerm));
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var departments = _db.Departments
                .Where(x =>
                    x.Name.Contains(searchTerm) ||
                    x.ShortName.Contains(searchTerm))
                .OrderByDescending(x => x.Container)
                .ThenBy(x => x.ContainerId)
                .ThenBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
           
            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Departments = departments
                }
            );
        }

        [Route("get/{id:int}")]
        [ResponseType(typeof(Department))]
        public IHttpActionResult GetDepartment(int id)
        {
            var department = _db
                .Departments
                .Include(x => x.SubSectionTabs)
                .Select(x => new DepartmentDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    Archived = x.Archived,
                    Live = x.Live,
                    Container = x.Container,
                    ContainerId = x.ContainerId,
                    MainColour = x.MainColour,
                    SubColour = x.SubColour,
                    BackgroundColour = x.BackgroundColour,
                    SideColourVariationOne = x.SideColourVariationOne,
                    SideColourVariationTwo = x.SideColourVariationTwo,
                    SideColourVariationThree = x.SideColourVariationThree,
                    SideColourVariationFour = x.SideColourVariationFour,
                    SideColourVariationFive = x.SideColourVariationFive,
                    SubSectionTabs = x.SubSectionTabs
                        .Where(
                            y => !y.Archived 
                            && y.Live
                        )
                        .Select(
                            y => new SubSectionTabDto()
                            {
                                Id = y.Id,
                                _id = y._id,
                                Name = y.Name,
                                Icon = y.Icon
                            }
                        )
                        .ToList()
                })
                .SingleOrDefault(
                    x => x.Id == id 
                    && !x.Archived
                );

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        [AllowAnonymous]
        [Route("get/{id:int}/totals")]
        [ResponseType(typeof(Department))]
        public IHttpActionResult GetDepartmentTotals(int id)
        {
            var department = _db.Departments.Find(id);
            if (department == null || !department.Live || department.Archived)
            {
                return NotFound();
            }

            var departmentTotalsDto = new DepartmentTotalsDto();

            var contactsTotal = _db.Users.Count(x => x.DepartmentId == id && x.Contact);
            departmentTotalsDto.ContactsTotal = contactsTotal;

            var subSectionTabs = _db.SubSectionTabs.Where(x => 
                (
                    x.DepartmentId == id || 
                    x.DepartmentId == null
                ) && !x.Archived
                ).ToList();
            departmentTotalsDto.SubSectionTabsTotal = subSectionTabs.Count();

            var subSections = new List<DepartmentSubSectionTotalsDto>();

            departmentTotalsDto.OverallTotal = 0;

            foreach (var subSectionTab in subSectionTabs)
            {
                var articlesTotal = _db.Articles
                    .Include(x => x.CurrentArticleRevision)
                    .Count(x => 
                        x.CurrentArticleRevision.SubSectionTabId == subSectionTab.Id && 
                        x.CurrentArticleRevision.Departments.Any(y => y.Id == id) && 
                        x.CurrentArticleRevision.Live &&
                        !x.Archived);

                var departmentSubSectionTotalDto = new DepartmentSubSectionTotalsDto()
                {
                    Id = subSectionTab.Id,
                    ArticlesTotal = articlesTotal,
                    ArticlesBodyTotal = articlesTotal
                };

                subSections.Add(departmentSubSectionTotalDto);

                departmentTotalsDto.OverallTotal = departmentTotalsDto.OverallTotal +
                                                   departmentSubSectionTotalDto.ArticlesTotal +
                                                   departmentSubSectionTotalDto.ArticlesBodyTotal;
            }

            departmentTotalsDto.SubSections = subSections;
            departmentTotalsDto.OverallTotal = departmentTotalsDto.OverallTotal +
                                               departmentTotalsDto.ContactsTotal +
                                               departmentTotalsDto.SubSectionTabsTotal;

            return Ok(departmentTotalsDto);
        }

        [AllowAnonymous]
        [Route("get/list/search/{searchTerm}")]
        public IHttpActionResult GetDepartmentsFromSearchTerm(string searchTerm)
        {
            if (searchTerm == null || searchTerm.Length <= 2) return BadRequest();

            var departments = _db.Departments
                .OrderByDescending(x => x.Name)
                .Where(x =>
                    x.Live &&
                    !x.Archived &&
                    (
                        x.Name.Contains(searchTerm) ||
                        x.ShortName.Contains(searchTerm)
                    )
                );
            return Ok(departments);
        }

        [Route("get/{id:int}/articles")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentArticles(int id)
        {
            if (!DepartmentExists(id)) return NotFound();

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x => 
                    x.CurrentArticleRevision.Departments.Any(y => y.Id == id) && 
                    !x.Archived &&
                    !x.CurrentArticleRevision.SubSectionTab.Archived)
                .Take(ArticlesLoadAmount);
            
            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/articles/list")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentArticlesList(int id)
        {
            if (!DepartmentExists(id)) return NotFound();

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x => 
                    x.CurrentArticleRevision.Departments.Any(y => y.Id == id) && 
                    x.CurrentArticleRevision.Live && 
                    x.CurrentArticleRevision.SubSectionTab.Live && 
                    !x.Archived &&
                    !x.CurrentArticleRevision.SubSectionTab.Archived)
                .Take(ArticlesLoadAmount);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtosAsList(articles.ToList(), _db));
        }

        [Route("get/{id:int}/subsection/{subSectionTabId:int}/articles")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentArticlesBySubSectionTabId(int id, int subSectionTabId)
        {
            if (!DepartmentExists(id) || !SubSectionTabExists(subSectionTabId)) return NotFound();

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x => x.CurrentArticleRevision.Departments.Any(y => y.Id == id))
                .Where(x => x.CurrentArticleRevision.SubSectionTabId == subSectionTabId)
                .Where(x => !x.Archived)
                .Where(x => !x.CurrentArticleRevision.SubSectionTab.Archived)
                .Take(ArticlesLoadAmount);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/subsection/{subSectionTabId:int}/articles/all")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetAllDepartmentArticlesBySubSectionTabId(int id, int subSectionTabId)
        {
            if (!DepartmentExists(id) || !SubSectionTabExists(subSectionTabId)) return NotFound();

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x => x.CurrentArticleRevision.Departments.Any(y => y.Id == id))
                .Where(x => x.CurrentArticleRevision.SubSectionTabId == subSectionTabId)
                .Where(x => x.CurrentArticleRevision.Live)
                .Where(x => x.CurrentArticleRevision.SubSectionTab.Live)
                .Where(x => !x.CurrentArticleRevision.SubSectionTab.Archived)
                .Where(x => !x.Archived);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/subsection/{subSectionTabId:int}/articles/list")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentArticlesListBySubSectionTabId(int id, int subSectionTabId)
        {
            if (!DepartmentExists(id) || !SubSectionTabExists(subSectionTabId)) return NotFound();

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x => x.CurrentArticleRevision.Departments.Any(y => y.Id == id))
                .Where(x => x.CurrentArticleRevision.SubSectionTabId == subSectionTabId)
                .Where(x => x.CurrentArticleRevision.Live)
                .Where(x => x.CurrentArticleRevision.SubSectionTab.Live)
                .Where(x => !x.CurrentArticleRevision.SubSectionTab.Archived)
                .Where(x => !x.Archived)
                .Take(ArticlesLoadAmount);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtosAsList(articles.ToList(), _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/subsection/{subSectionTabId:int}/articles/list/all")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentAllArticlesListBySubSectionTabId(int id, int subSectionTabId)
        {
            if (!DepartmentExists(id) || !SubSectionTabExists(subSectionTabId)) return NotFound();

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .OrderBy(x => x.CurrentArticleRevision.Title)
                .Where(x => x.CurrentArticleRevision.Departments.Any(y => y.Id == id))
                .Where(x => x.CurrentArticleRevision.SubSectionTabId == subSectionTabId)
                .Where(x => x.CurrentArticleRevision.Live)
                .Where(x => x.CurrentArticleRevision.SubSectionTab.Live)
                .Where(x => !x.CurrentArticleRevision.SubSectionTab.Archived)
                .Where(x => !x.Archived);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtosAsList(articles.ToList(), _db));
        }

        [Route("get/{id:int}/subsection/{subSectionTabId:int}/articles/{articleId:int}")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentSubSectionArticlesFromArticleId(int id, int subSectionTabId, int articleId)
        {
            if (!DepartmentExists(id) || !SubSectionTabExists(subSectionTabId) || !ArticleExists(articleId)) return NotFound();

            //get the last date
            var lastArticleSent = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .SingleOrDefault(x => x.Id == articleId);

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x =>
                    (
                        x.CurrentArticleRevision.SubSectionTabId == subSectionTabId &&
                        x.CurrentArticleRevision.Departments.Any(y => y.Id == id) &&
                        x.CurrentArticleRevision.RevisionDateTime < lastArticleSent.CurrentArticleRevision.RevisionDateTime
                    ) && !x.Archived && !x.CurrentArticleRevision.SubSectionTab.Archived
                )
                .Take(InifiniteLoadAmount);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/subsection/{subSectionTabId:int}/articles/list/{articleId:int}")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentSubSectionArticlesListFromArticleId(int id, int subSectionTabId, int articleId)
        {
            if (!DepartmentExists(id) || !SubSectionTabExists(subSectionTabId) || !ArticleExists(articleId)) return NotFound();

            //get the last date
            var lastArticleSent = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .SingleOrDefault(x => x.Id == articleId);

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x =>
                    x.CurrentArticleRevision.Live && !x.Archived && !x.CurrentArticleRevision.SubSectionTab.Archived &&
                    (
                        x.CurrentArticleRevision.SubSectionTabId == subSectionTabId &&
                        x.CurrentArticleRevision.Departments.Any(y => y.Id == id) &&
                        x.CurrentArticleRevision.RevisionDateTime < lastArticleSent.CurrentArticleRevision.RevisionDateTime
                    )
                )
                .Take(InifiniteLoadAmount);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtosAsList(articles.ToList(), _db));
        }

        [Route("get/{id:int}/articles/{articleId:int}")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentArticlesFromArticleId(int id, int articleId)
        {
            if (!DepartmentExists(id) || !ArticleExists(articleId)) return NotFound();

            //get the last date
            var lastArticleSent = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .SingleOrDefault(x => x.Id == articleId);

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x => 
                    (
                        x.CurrentArticleRevision.Departments.Any(y => y.Id == id) && 
                        x.CurrentArticleRevision.RevisionDateTime < lastArticleSent.CurrentArticleRevision.RevisionDateTime
                    ) && !x.Archived && !x.CurrentArticleRevision.SubSectionTab.Archived
                )
                .Take(InifiniteLoadAmount);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/articles/list/{articleId:int}")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentArticlesListFromArticleId(int id, int articleId)
        {
            if (!DepartmentExists(id) || !ArticleExists(articleId)) return NotFound();

            //get the last date
            var lastArticleSent = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .SingleOrDefault(x => x.Id == articleId);

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x =>
                    x.CurrentArticleRevision.Live && !x.Archived && !x.CurrentArticleRevision.SubSectionTab.Archived &&
                    (
                        x.CurrentArticleRevision.Departments.Any(y => y.Id == id) &&
                        x.CurrentArticleRevision.RevisionDateTime < lastArticleSent.CurrentArticleRevision.RevisionDateTime
                    )
                )
                .Take(InifiniteLoadAmount);

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtosAsList(articles.ToList(), _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/articles/search/{searchTerm}")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentArticlesFromSearchTerm(int id, string searchTerm)
        {
            if (!DepartmentExists(id)) return NotFound();
            if (searchTerm == null || searchTerm.Length <= 2) return BadRequest();

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x => 
                    x.CurrentArticleRevision.Live &&
                    x.CurrentArticleRevision.Departments.Any(y => y.Id == id) && 
                    x.CurrentArticleRevision.SubSectionTab.Live &&
                    !x.CurrentArticleRevision.SubSectionTab.Archived &&
                    !x.Archived &&
                    (
                        x.CurrentArticleRevision.Title.Contains(searchTerm) ||
                        x.CurrentArticleRevision.SubSectionTab.Name.Contains(searchTerm) ||
                        x.CurrentArticleRevision.Body.Contains(searchTerm)
                    )
                );

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/articles/subsection/{subSectionTabId:int}/search/{searchTerm}")]
        [ResponseType(typeof(ArticleRevisionDto))]
        public IHttpActionResult GetDepartmentSubSectionArticlesFromSearchTerm(int id, int subSectionTabId, string searchTerm)
        {
            if (!DepartmentExists(id) || !SubSectionTabExists(subSectionTabId)) return NotFound();
            if (searchTerm == null || searchTerm.Length <= 2) return BadRequest();

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x =>
                    x.CurrentArticleRevision.Live &&
                    x.CurrentArticleRevision.Departments.Any(y => y.Id == id) &&
                    x.CurrentArticleRevision.SubSectionTabId == subSectionTabId &&
                    !x.CurrentArticleRevision.SubSectionTab.Archived &&
                    !x.Archived &&
                    (
                        x.CurrentArticleRevision.Title.Contains(searchTerm) ||
                        x.CurrentArticleRevision.SubSectionTab.Name.Contains(searchTerm) ||
                        x.CurrentArticleRevision.Body.Contains(searchTerm)
                    )
                );

            return Ok(_mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db));
        }

        [AllowAnonymous]
        [Route("get/{id:int}/users")]
        public IHttpActionResult GetUsersByDepartment(int id)
        {
            if (!DepartmentExists(id)) return NotFound();

            return Ok(_db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Where(
                    x =>
                        x.DepartmentId == id
                        && x.Contact && !x.Department.Archived
                    )
                .OrderBy(x => x.Forename)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    SamAccountName = x.UserName,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    JobTitle = x.JobTitle,
                    PhoneNumber = x.PhoneNumber,
                    EmailAddress = x.EmailAddress,
                    SiteAdmin = x.SiteAdmin,
                    LeadClinician = x.LeadClinician,
                    ContentApprover = x.ContentApprover,
                    ContentEditor = x.ContentEditor,
                    Contact = x.Contact,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    AdministationDepartments = x.AdministationDepartments.Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    })
                })
                .Take(ContactsLoadAmount)
                .ToList());
        }

        [AllowAnonymous]
        [Route("get/{id:int}/users/all")]
        public IHttpActionResult GetAllUsersByDepartment(int id)
        {
            if (!DepartmentExists(id)) return NotFound();

            return Ok(_db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Where(
                    x =>
                        x.DepartmentId == id
                        && x.Contact && !x.Department.Archived
                    )
                .OrderBy(x => x.Forename)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    SamAccountName = x.UserName,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    JobTitle = x.JobTitle,
                    PhoneNumber = x.PhoneNumber,
                    EmailAddress = x.EmailAddress,
                    SiteAdmin = x.SiteAdmin,
                    LeadClinician = x.LeadClinician,
                    ContentApprover = x.ContentApprover,
                    ContentEditor = x.ContentEditor,
                    Contact = x.Contact,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    AdministationDepartments = x.AdministationDepartments.Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    })
                })
                .ToList());
        }

        [AllowAnonymous]
        [Route("get/{id:int}/phone/all")]
        public IHttpActionResult GetAllPhoneInformationByDepartment(int id)
        {
            if (!DepartmentExists(id)) return NotFound();

            var contacts = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Where(
                    x =>
                        x.DepartmentId == id
                        && x.Contact && !x.Department.Archived
                )
                .OrderBy(x => x.Forename)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    SamAccountName = x.UserName,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    JobTitle = x.JobTitle,
                    PhoneNumber = x.PhoneNumber,
                    EmailAddress = x.EmailAddress,
                    SiteAdmin = x.SiteAdmin,
                    LeadClinician = x.LeadClinician,
                    ContentApprover = x.ContentApprover,
                    ContentEditor = x.ContentEditor,
                    Contact = x.Contact,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    AdministationDepartments = x.AdministationDepartments.Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    })
                })
                .ToList();

            var staticPhoneNumbers = _db.StaticPhoneNumbers.Include(x => x.Department)
                .Where(
                    x =>
                        x.DepartmentId == id
                        && !x.Department.Archived
                )
                .OrderBy(x => x.Title);

            var staticPhoneNumbersDto = new List<StaticPhoneNumberDto>();
            foreach (var staticPhoneNumber in staticPhoneNumbers)
            {
                staticPhoneNumbersDto.Add(new StaticPhoneNumberDto()
                {
                    Id = staticPhoneNumber.Id,
                    _id = staticPhoneNumber._id,
                    Type = staticPhoneNumber.Type,
                    TypeName = Enum.GetName(typeof(StaticPhoneNumberType), staticPhoneNumber.Type),
                    Title = staticPhoneNumber.Title,
                    PhoneNumber = staticPhoneNumber.PhoneNumber,
                    DepartmentId = staticPhoneNumber.DepartmentId,
                    DepartmentName = staticPhoneNumber.Department.Name,
                    DepartmentBackgroundColour = staticPhoneNumber.Department.BackgroundColour,
                    DepartmentMainColour = staticPhoneNumber.Department.MainColour
                });
            }

            return Ok(new 
            {
                Contacts = contacts,
                StaticPhoneNumbers = staticPhoneNumbersDto
            });
        }

        [AllowAnonymous]
        [Route("get/{containerId:int}/phone/all/containerId")]
        public IHttpActionResult GetAllPhoneInformationByContainer(int containerId)
        {
            if (!DepartmentExists(containerId)) return NotFound();

            var contacts = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Where(
                    x =>
                        x.Department.ContainerId == containerId
                        && x.Contact && !x.Department.Archived
                )
                .OrderBy(x => x.Forename)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    SamAccountName = x.UserName,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    JobTitle = x.JobTitle,
                    PhoneNumber = x.PhoneNumber,
                    EmailAddress = x.EmailAddress,
                    SiteAdmin = x.SiteAdmin,
                    LeadClinician = x.LeadClinician,
                    ContentApprover = x.ContentApprover,
                    ContentEditor = x.ContentEditor,
                    Contact = x.Contact,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    AdministationDepartments = x.AdministationDepartments.Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    })
                })
                .ToList();

            var staticPhoneNumbers = _db.StaticPhoneNumbers.Include(x => x.Department)
                .Where(
                    x =>
                        x.Department.ContainerId == containerId
                        && !x.Department.Archived
                )
                .OrderBy(x => x.Title);

            var staticPhoneNumbersDto = new List<StaticPhoneNumberDto>();
            foreach (var staticPhoneNumber in staticPhoneNumbers)
            {
                staticPhoneNumbersDto.Add(new StaticPhoneNumberDto()
                {
                    Id = staticPhoneNumber.Id,
                    _id = staticPhoneNumber._id,
                    Type = staticPhoneNumber.Type,
                    TypeName = Enum.GetName(typeof(StaticPhoneNumberType), staticPhoneNumber.Type),
                    Title = staticPhoneNumber.Title,
                    PhoneNumber = staticPhoneNumber.PhoneNumber,
                    DepartmentId = staticPhoneNumber.DepartmentId,
                    DepartmentName = staticPhoneNumber.Department.Name,
                    DepartmentBackgroundColour = staticPhoneNumber.Department.BackgroundColour,
                    DepartmentMainColour = staticPhoneNumber.Department.MainColour
                });
            }

            return Ok(new 
            {
                Contacts = contacts,
                StaticPhoneNumbers = staticPhoneNumbersDto
            });
        }

        [AllowAnonymous]
        [Route("get/{id:int}/users/{contactId:int}")]
        public IHttpActionResult GetUsersByDepartmentFromContactId(int id, int contactId)
        {
            if (!DepartmentExists(id) || !UserExists(contactId)) return NotFound();

            //get the last date
            var lastContactSent = _db.Users
                .SingleOrDefault(x => x.Id == contactId);

            return Ok(_db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Where(
                    x =>
                        x.DepartmentId == id
                        && !x.Department.Archived
                        && x.Contact
                        && String.Compare(x.Forename, lastContactSent.Forename, StringComparison.Ordinal) > 0
                    )
                .OrderBy(x => x.Forename)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    SamAccountName = x.UserName,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    JobTitle = x.JobTitle,
                    PhoneNumber = x.PhoneNumber,
                    EmailAddress = x.EmailAddress,
                    SiteAdmin = x.SiteAdmin,
                    LeadClinician = x.LeadClinician,
                    ContentApprover = x.ContentApprover,
                    ContentEditor = x.ContentEditor,
                    Contact = x.Contact,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    AdministationDepartments = x.AdministationDepartments.Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    })
                })
                .Take(InifiniteLoadAmount)
                .ToList());
        }

        [AllowAnonymous]
        [Route("get/{id:int}/users/search/{searchTerm}")]
        public IHttpActionResult GetUsersByDepartmentFromSearchTerm(int id, string searchTerm)
        {
            if (!DepartmentExists(id)) return NotFound();
            if (searchTerm == null || searchTerm.Length <= 2) return BadRequest();

            return Ok(_db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Where(
                    x =>
                        x.DepartmentId == id
                        && !x.Department.Archived
                        && x.Contact
                        && (
                            x.Forename.Contains(searchTerm) ||
                            x.Surname.Contains(searchTerm) ||
                            x.EmailAddress.Contains(searchTerm) ||
                            x.JobTitle.Contains(searchTerm)
                        )
                    )
                .OrderBy(x => x.Forename)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    SamAccountName = x.UserName,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    JobTitle = x.JobTitle,
                    PhoneNumber = x.PhoneNumber,
                    EmailAddress = x.EmailAddress,
                    SiteAdmin = x.SiteAdmin,
                    LeadClinician = x.LeadClinician,
                    ContentApprover = x.ContentApprover,
                    ContentEditor = x.ContentEditor,
                    Contact = x.Contact,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    AdministationDepartments = x.AdministationDepartments.Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    })
                })
                .ToList());
        }

        [AllowAnonymous]
        [Route("get/{id:int}/search/{searchTerm}")]
        public IHttpActionResult GetUsersAndArticlesByDepartmentFromSearchTerm(int id, string searchTerm)
        {
            if (!DepartmentExists(id)) return NotFound();
            if (searchTerm == null || searchTerm.Length <= 2) return BadRequest();

            var contacts = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Where(
                    x =>
                        x.DepartmentId == id
                        && !x.Department.Archived
                        && x.Contact
                        && (
                            x.Forename.Contains(searchTerm) ||
                            x.Surname.Contains(searchTerm) ||
                            x.EmailAddress.Contains(searchTerm) ||
                            x.JobTitle.Contains(searchTerm)
                            )
                )
                .OrderBy(x => x.Forename)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    SamAccountName = x.UserName,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    JobTitle = x.JobTitle,
                    PhoneNumber = x.PhoneNumber,
                    EmailAddress = x.EmailAddress,
                    SiteAdmin = x.SiteAdmin,
                    LeadClinician = x.LeadClinician,
                    ContentApprover = x.ContentApprover,
                    ContentEditor = x.ContentEditor,
                    Contact = x.Contact,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    AdministationDepartments = x.AdministationDepartments.Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    })
                })
                .ToList();

            var staticPhoneNumbers = _db.StaticPhoneNumbers.Include(x => x.Department)
                .Where(
                    x =>
                        x.DepartmentId == id
                        && !x.Department.Archived
                        && ( 
                            x.Title.Contains(searchTerm) ||
                            x.PhoneNumber.Contains(searchTerm)
                        )
                );

            var staticPhoneNumbersDto = new List<StaticPhoneNumberDto>();
            foreach (var staticPhoneNumber in staticPhoneNumbers)
            {
                staticPhoneNumbersDto.Add(new StaticPhoneNumberDto()
                {
                    Id = staticPhoneNumber.Id,
                    _id = staticPhoneNumber._id,
                    Type = staticPhoneNumber.Type,
                    TypeName = Enum.GetName(typeof(StaticPhoneNumberType), staticPhoneNumber.Type),
                    Title = staticPhoneNumber.Title,
                    PhoneNumber = staticPhoneNumber.PhoneNumber,
                    DepartmentId = staticPhoneNumber.DepartmentId,
                    DepartmentName = staticPhoneNumber.Department.Name
                });
            }

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x =>
                    x.CurrentArticleRevision.Live && 
                    x.CurrentArticleRevision.SubSectionTab.Live &&
                    x.CurrentArticleRevision.Departments.Any(y => y.Id == id) &&
                    !x.Archived &&
                    !x.CurrentArticleRevision.SubSectionTab.Archived && 
                    (
                        x.CurrentArticleRevision.Title.Contains(searchTerm) ||
                        x.CurrentArticleRevision.SubSectionTab.Name.Contains(searchTerm) ||
                        x.CurrentArticleRevision.Body.Contains(searchTerm)
                    )
                );

            var searchResultDto = new SearchResultDto()
            {
                CrossDepartmentSearch = false,
                ArticleResults = _mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db),
                ContactResults = contacts,
                StaticPhoneNumberResults = staticPhoneNumbersDto
            };

            return Ok(searchResultDto);
        }

        [AllowAnonymous]
        [Route("get/search/{searchTerm}")]
        public IHttpActionResult GetUsersAndArticlesFromSearchTerm(string searchTerm)
        {
            if (searchTerm == null || searchTerm.Length <= 2) return BadRequest();

            var contacts = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Where(
                    x =>
                        x.Contact
                        && !x.Department.Archived
                        && (
                            x.Forename.Contains(searchTerm) ||
                            x.Surname.Contains(searchTerm) ||
                            x.EmailAddress.Contains(searchTerm) ||
                            x.JobTitle.Contains(searchTerm)
                            )
                )
                .OrderBy(x => x.Forename)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    _id = x._id,
                    SamAccountName = x.UserName,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    JobTitle = x.JobTitle,
                    PhoneNumber = x.PhoneNumber,
                    EmailAddress = x.EmailAddress,
                    SiteAdmin = x.SiteAdmin,
                    LeadClinician = x.LeadClinician,
                    ContentApprover = x.ContentApprover,
                    ContentEditor = x.ContentEditor,
                    Contact = x.Contact,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    AdministationDepartments = x.AdministationDepartments.Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    })
                })
                .ToList();

            var staticPhoneNumbers = _db.StaticPhoneNumbers.Include(x => x.Department)
                .Where(
                    x =>
                        x.Title.Contains(searchTerm) ||
                        x.PhoneNumber.Contains(searchTerm)
                );

            var staticPhoneNumbersDto = new List<StaticPhoneNumberDto>();
            foreach (var staticPhoneNumber in staticPhoneNumbers)
            {
                staticPhoneNumbersDto.Add(new StaticPhoneNumberDto()
                {
                    Id = staticPhoneNumber.Id,
                    _id = staticPhoneNumber._id,
                    Type = staticPhoneNumber.Type,
                    TypeName = Enum.GetName(typeof(StaticPhoneNumberType), staticPhoneNumber.Type),
                    Title = staticPhoneNumber.Title,
                    PhoneNumber = staticPhoneNumber.PhoneNumber,
                    DepartmentId = staticPhoneNumber.DepartmentId,
                    DepartmentName = staticPhoneNumber.Department.Name
                });
            }

            var articles = _db.Articles
                .Include(x => x.CurrentArticleRevision)
                .Include(x => x.CurrentArticleRevision.Departments)
                .Include(x => x.CurrentArticleRevision.SubSectionTab)
                .Include(x => x.CurrentArticleRevision.Terms)
                .OrderByDescending(x => x.CurrentArticleRevision.RevisionDateTime)
                .Where(x =>
                    x.CurrentArticleRevision.Live &&
                    x.CurrentArticleRevision.SubSectionTab.Live &&
                    !x.Archived &&
                    !x.CurrentArticleRevision.SubSectionTab.Archived && 
                    (
                        x.CurrentArticleRevision.Title.Contains(searchTerm) ||
                        x.CurrentArticleRevision.SubSectionTab.Name.Contains(searchTerm) ||
                        x.CurrentArticleRevision.Body.Contains(searchTerm)
                    )
                );

            var searchResultDto = new SearchResultDto()
            {
                CrossDepartmentSearch = true,
                ArticleResults = _mappingHelper.MapArticlesToArticleRevisionDtos(articles.ToList(), true, _db),
                ContactResults = contacts,
                StaticPhoneNumberResults = staticPhoneNumbersDto
            };

            return Ok(searchResultDto);
        }

        [Route("put/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDepartment(int id, Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            if (id != department.Id)
            {
                return BadRequest();
            }

            if (department._id == Guid.Empty) department._id = Guid.NewGuid();

            _db.Logs.Add(
                _logHelper.CreateDepartmentApiControllerLog(
                    LogHelper.LogType.Update,
                    "DepartmentApiController.PutDepartment",
                    $"api/department/put/{id}",
                    department,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkDepartmentAsModified(department);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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
        [ResponseType(typeof(Department))]
        public IHttpActionResult PostDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            department._id = Guid.NewGuid();

            _db.Departments.Add(department);

            _db.Logs.Add(
                _logHelper.CreateDepartmentApiControllerLog(
                    LogHelper.LogType.Create,
                    "DepartmentApiController.PostDepartment",
                    "api/department/post/",
                    department,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok(department);
        }

        [Route("archive/{id:int}")]
        public IHttpActionResult PutArchiveDepartment(int id)
        {
            var department = _db.Departments.Find(id);
            if (department == null)
            {
                return NotFound();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            department.Archived = true;
            department.ArchiveDateTime = DateTime.Now;

            _db.Logs.Add(
                _logHelper.CreateDepartmentApiControllerLog(
                    LogHelper.LogType.Archive,
                    "DepartmentApiController.PutArchiveDepartment",
                    $"api/department/archive/{id}",
                    department,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkDepartmentAsModified(department);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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
        public IHttpActionResult PutUnArchiveDepartment(int id)
        {
            var department = _db.Departments.Find(id);
            if (department == null)
            {
                return NotFound();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            department.Archived = false;
            department.ArchiveDateTime = null;

            _db.Logs.Add(
                _logHelper.CreateDepartmentApiControllerLog(
                    LogHelper.LogType.Archive,
                    "DepartmentApiController.PutUnArchiveDepartment",
                    $"api/department/unarchive/{id}",
                    department,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkDepartmentAsModified(department);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        [Route("post/{departmentId:int}/subsectiontab/{subSectionTabId:int}")]
        [ResponseType(typeof(User))]
        public IHttpActionResult PostDepartmentSubSection(int departmentId, int subSectionTabId)
        {
            if (!IsUserSiteAdmin() && !IsUserLeadClinician() && !IsUserContentApprover() && !IsUserContentCreator())
            {
                return Unauthorized();
            }

            var subSectionTab = _db.SubSectionTabs                
                .FirstOrDefault(x => x.Id == subSectionTabId);
            var department = _db.Departments
                .Include(x => x.SubSectionTabs)
                .FirstOrDefault(x => x.Id == departmentId);

            if (subSectionTab == null || department == null)
            {
                return NotFound();
            }

            department.SubSectionTabs.Add(subSectionTab);

            _db.MarkDepartmentAsModified(department);
            _db.SaveChanges();

            _db.Logs.Add(
                _logHelper.CreateDepartmentApiControllerLog(
                    LogHelper.LogType.Create,
                    "DepartmentApiController.PostDepartmentSubSection",
                    $"api/user/post/{departmentId}/subsectiontab/{subSectionTabId}",
                    department,
                    User.GetUsernameWithoutDomain()
                )
            );

            return Ok();
        }

        [Route("delete/{departmentId:int}/subsectiontab/{subSectionTabId:int}")]
        public IHttpActionResult DeleteDepartmentSubSection(int departmentId, int subSectionTabId)
        {
            if (!IsUserSiteAdmin() && !IsUserLeadClinician() && !IsUserContentApprover() && !IsUserContentCreator())
            {
                return Unauthorized();
            }

            var subSectionTab = _db.SubSectionTabs
                .FirstOrDefault(x => x.Id == subSectionTabId);
            var department = _db.Departments
                .Include(x => x.SubSectionTabs)
                .FirstOrDefault(x => x.Id == departmentId);

            if (subSectionTab == null || department == null)
            {
                return NotFound();
            }

            department.SubSectionTabs.Remove(subSectionTab);

            _db.MarkDepartmentAsModified(department);
            _db.SaveChanges();

            _db.Logs.Add(
                _logHelper.CreateDepartmentApiControllerLog(
                    LogHelper.LogType.Archive,
                    "DepartmentApiController.DeleteDepartmentSubSection",
                    $"api/user/delete/{departmentId}/subsectiontab/{subSectionTabId}",
                    department,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok();
        }

        private bool IsUserSiteAdmin()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && x.SiteAdmin) > 0; 
        }

        private bool IsUserLeadClinician()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && x.LeadClinician) > 0;
        }

        private bool IsUserContentApprover()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && x.ContentApprover) > 0;
        }

        private bool IsUserContentCreator()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && x.ContentEditor) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DepartmentExists(int id)
        {
            return _db.Departments.Count(e => e.Id == id && e.Live) > 0;
        }

        private bool SubSectionTabExists(int id)
        {
            return _db.SubSectionTabs.Count(e => e.Id == id && e.Live) > 0;
        }

        private bool ArticleExists(int id)
        {
            return _db.Articles.Count(e => e.Id == id) > 0;
        }

        private bool UserExists(int id)
        {
            return _db.Users.Count(e => e.Id == id) > 0;
        }
    }
}