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
    [RoutePrefix("api/staticphonenumber")]
    public class StaticPhoneNumberApiController : ApiController
    {
        private readonly IClinicalGuidelinesAppContext _db = new ClinicalGuidelinesAppContext();
        private readonly LogHelper _logHelper = new LogHelper();

        public StaticPhoneNumberApiController()
        {
        }

        public StaticPhoneNumberApiController(IClinicalGuidelinesAppContext context)
        {
            _db = context;
        }

        [Route("get")]
        public IHttpActionResult GetStaticPhoneNumbers()
        {
            var staticPhoneNumbers = _db.StaticPhoneNumbers.Include(x => x.Department);

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

            return Ok(staticPhoneNumbersDto);
        }

        [Route("get/types")]
        public IHttpActionResult GetStaticPhoneNumberTypes()
        {
            return Ok(Enum.GetNames(typeof(StaticPhoneNumberType)));
        }

        [Route("get/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult GetPagedStaticPhoneNumbers(int pageSize, int pageNumber)
        {
            var totalCount = _db.StaticPhoneNumbers.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var staticPhoneNumbers = _db.StaticPhoneNumbers
                .Include(x => x.Department)
                .OrderBy(x => x.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

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

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    StaticPhoneNumbers = staticPhoneNumbersDto
                }
            );
        }

        [Route("get/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedStaticPhoneNumbersByKeyword(int pageSize, int pageNumber, string searchTerm)
        {
            var totalCount = _db.StaticPhoneNumbers
                .Include(x => x.Department)
                .Count(x => 
                    x.Title.Contains(searchTerm) ||
                    x.Department.ShortName.Contains(searchTerm) ||
                    x.Department.Name.Contains(searchTerm));
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var staticPhoneNumbers = _db.StaticPhoneNumbers
                .Include(x => x.Department)                
                .Where(x =>
                    x.Title.Contains(searchTerm) ||
                    x.Department.ShortName.Contains(searchTerm) ||
                    x.Department.Name.Contains(searchTerm))
                .OrderBy(x => x.Title)
                .Skip((pageNumber - 1)*pageSize)
                .Take(pageSize);

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

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    StaticPhoneNumbers = staticPhoneNumbersDto
                }
            );
        }

        [Route("get/{id:int}")]
        [ResponseType(typeof(StaticPhoneNumber))]
        public IHttpActionResult GetStaticPhoneNumber(int id)
        {
            var staticPhoneNumber = _db.StaticPhoneNumbers.Find(id);
            if (staticPhoneNumber == null)
            {
                return NotFound();
            }

            return Ok(staticPhoneNumber);
        }

        [Route("put/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStaticPhoneNumber(int id, StaticPhoneNumber staticPhoneNumber)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != staticPhoneNumber.Id)
            {
                return BadRequest();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            if (staticPhoneNumber._id == Guid.Empty) staticPhoneNumber._id = Guid.NewGuid();

            _db.Logs.Add(
                _logHelper.CreateStaticPhoneNumberApiControllerLog(
                    LogHelper.LogType.Update,
                    "StaticPhoneNumberApiController.PutStaticPhoneNumber",
                    $"api/staticphonenumber/put/{id}",
                    staticPhoneNumber,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkStaticPhoneNumberAsModified(staticPhoneNumber);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        [ResponseType(typeof(StaticPhoneNumber))]
        public IHttpActionResult PostStaticPhoneNumber(StaticPhoneNumber staticPhoneNumber)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            staticPhoneNumber._id = Guid.NewGuid();

            _db.StaticPhoneNumbers.Add(staticPhoneNumber);

            _db.Logs.Add(
                _logHelper.CreateStaticPhoneNumberApiControllerLog(
                    LogHelper.LogType.Create,
                    "StaticPhoneNumberApiController.PostStaticPhoneNumber",
                    "api/staticphonenumber/post/",
                    staticPhoneNumber,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok(staticPhoneNumber);
        }

        [Route("delete/{id:int}")]
        public IHttpActionResult DeleteStaticPhoneNumber(int id)
        {
            var staticPhoneNumber = _db.StaticPhoneNumbers.Find(id);
            if (staticPhoneNumber == null)
            {
                return NotFound();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            _db.StaticPhoneNumbers.Remove(staticPhoneNumber);

            _db.Logs.Add(
                _logHelper.CreateStaticPhoneNumberApiControllerLog(
                    LogHelper.LogType.Delete,
                    "StaticPhoneNumberApiController.DeleteStaticPhoneNumber",
                    $"api/staticphonenumber/delete/{id}",
                    staticPhoneNumber,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok(staticPhoneNumber);
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

        private bool UserExists(int id)
        {
            return _db.Users.Count(e => e.Id == id) > 0;
        }
    }
}
