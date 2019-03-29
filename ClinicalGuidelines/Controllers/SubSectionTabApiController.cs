//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ClinicalGuidelines.Helpers;
using ClinicalGuidelines.Models;
using ClinicalGuidelines.DTOs;

namespace ClinicalGuidelines.Controllers
{
    [Authorize]
    [RoutePrefix("api/subsectiontab")]
    public class SubSectionTabApiController : ApiController
    {
        private readonly IClinicalGuidelinesAppContext _db = new ClinicalGuidelinesAppContext();
        private readonly LogHelper _logHelper = new LogHelper();

        public SubSectionTabApiController()
        {
        }

        public SubSectionTabApiController(IClinicalGuidelinesAppContext context)
        {
            _db = context;
        }

        [Route("get")]
        public IQueryable<SubSectionTab> GetSubSectionTabs()
        {
            return _db.SubSectionTabs.Where(x => !x.Archived);
        }

        [Route("get/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult GetPagedtSubSectionTabs(int pageSize, int pageNumber)
        {
            var totalCount = _db.SubSectionTabs.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var subSectionTabs = _db.SubSectionTabs
                .OrderBy(x => x.DepartmentId)
                .ThenBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    SubSectionTabs = subSectionTabs
                }
            );
        }

        [Route("get/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedtSubSectionTabsByKeyword(int pageSize, int pageNumber, string searchTerm)
        {
            var totalCount = _db.SubSectionTabs.Count(x => x.Name.Contains(searchTerm));
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var subSectionTabs = _db.SubSectionTabs
                .Where(x => x.Name.Contains(searchTerm))
                .OrderBy(x => x.DepartmentId)
                .ThenBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    SubSectionTabs = subSectionTabs
                }
            );
        }

        [Route("get/live/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult GetPagedLiveSubSectionTabs(int pageSize, int pageNumber)
        {
            var subSectionTabs = _db.SubSectionTabs
                .Where(x => !x.Archived && x.Live)
                .OrderBy(x => x.DepartmentId)
                .ThenBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var totalCount = _db.SubSectionTabs.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);            

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    SubSectionTabs = subSectionTabs
                }
            );
        }

        [Route("get/live/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedLiveSubSectionTabsByKeyword(int pageSize, int pageNumber, string searchTerm)
        {
            var subSectionTabs = _db.SubSectionTabs
                .Where(x => !x.Archived && x.Live)
                .Where(x => x.Name.Contains(searchTerm))
                .OrderBy(x => x.DepartmentId)
                .ThenBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var totalCount = _db.SubSectionTabs.Count(x => x.Name.Contains(searchTerm));
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    SubSectionTabs = subSectionTabs
                }
            );
        }

        [Route("get/global")]
        public IQueryable<SubSectionTab> GetGlobalSubSectionTabs()
        {
            return _db.SubSectionTabs.Where(x => x.DepartmentId == null && !x.Archived);
        }

        [Route("get/nonglobal/{id:int}")]
        public IQueryable<SubSectionTab> GetDepartmentNonGlobalSubSectionTabs(int id)
        {
            return _db.SubSectionTabs.Where(x => x.DepartmentId == id && !x.Archived);
        }

        [Route("get/{id:int}")]
        [ResponseType(typeof(SubSectionTab))]
        public IHttpActionResult GetSubSectionTab(int id)
        {
            var subSectionTab = _db.SubSectionTabs.Find(id);
            if (subSectionTab == null || subSectionTab.Archived)
            {
                return NotFound();
            }

            return Ok(subSectionTab);
        }

        [AllowAnonymous]
        [Route("get/department/{departmentId:int}")]
        [ResponseType(typeof(SubSectionTab))]
        public IHttpActionResult GetSubSectionTabsForDepartment(int departmentId)
        {
            if (!DepartmentExists(departmentId)) return NotFound();

            var department = _db.Departments
                .Include(x => x.SubSectionTabs)                
                .Where(x => x.Id == departmentId)
                .Select(x => new DepartmentDto()
                {
                    SubSectionTabs = x.SubSectionTabs
                        .Select(y => new SubSectionTabDto()
                        {
                            Id = y.Id,
                            _id = y._id,
                            Name = y.Name,
                            Icon = y.Icon,
                            Live = y.Live,
                            Archived = y.Archived,
                            Department = null
                        }
                        )
                        .ToList()
                });

            return Ok(department.First().SubSectionTabs);
        }

        [Route("put/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSubSectionTab(int id, SubSectionTab subSectionTab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            if (id != subSectionTab.Id)
            {
                return BadRequest();
            }

            if (subSectionTab._id == Guid.Empty) subSectionTab._id = Guid.NewGuid();

            _db.Logs.Add(
                _logHelper.CreateSubSectionTabApiControllerLog(
                    LogHelper.LogType.Update,
                    "SubSectionTabApiController.PutSubSectionTab",
                    $"api/subsectiontab/put/{id}",
                    subSectionTab,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkSubSectionTabAsModified(subSectionTab);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubSectionTabExists(id))
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
        [ResponseType(typeof(SubSectionTab))]
        public IHttpActionResult PostSubSectionTab(SubSectionTab subSectionTab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            subSectionTab._id = Guid.NewGuid();

            _db.SubSectionTabs.Add(subSectionTab);

            _db.Logs.Add(
                _logHelper.CreateSubSectionTabApiControllerLog(
                    LogHelper.LogType.Create,
                    "SubSectionTabApiController.PostSubSectionTab",
                    "api/subsectiontab/post",
                    subSectionTab,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok(subSectionTab);
        }

        [Route("archive/{id:int}")]
        [ResponseType(typeof(SubSectionTab))]
        public IHttpActionResult PutArchiveSubSectionTab(int id)
        {
            var subSectionTab = _db.SubSectionTabs.Find(id);
            if (subSectionTab == null)
            {
                return NotFound();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            subSectionTab.Archived = true;
            subSectionTab.ArchiveDateTime = DateTime.Now;

            _db.Logs.Add(
                _logHelper.CreateSubSectionTabApiControllerLog(
                    LogHelper.LogType.Archive,
                    "SubSectionTabApiController.PutArchiveSubSectionTab",
                    $"api/subsectiontab/archive/{id}",
                    subSectionTab,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkSubSectionTabAsModified(subSectionTab);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubSectionTabExists(id))
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
        [ResponseType(typeof(SubSectionTab))]
        public IHttpActionResult PutUnArchiveSubSectionTab(int id)
        {
            var subSectionTab = _db.SubSectionTabs.Find(id);
            if (subSectionTab == null)
            {
                return NotFound();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            subSectionTab.Archived = false;
            subSectionTab.ArchiveDateTime = null;

            _db.Logs.Add(
                _logHelper.CreateSubSectionTabApiControllerLog(
                    LogHelper.LogType.Archive,
                    "SubSectionTabApiController.PutUnArchiveSubSectionTab",
                    $"api/subsectiontab/unarchive/{id}",
                    subSectionTab,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkSubSectionTabAsModified(subSectionTab);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubSectionTabExists(id))
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

        private bool IsUserSiteAdmin()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && x.SiteAdmin) > 0; ;
        }

        private bool IsUserLeadClinician()
        {
            var userName = User.GetUsernameWithoutDomain();
            return _db.Users.Count(x => x.UserName == userName && x.LeadClinician) > 0;
        }

        private bool SubSectionTabExists(int id)
        {
            return _db.SubSectionTabs.Count(e => e.Id == id) > 0;
        }

        private bool DepartmentExists(int id)
        {
            return _db.Departments.Count(e => e.Id == id && e.Live) > 0;
        }
    }
}