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
using System.Web.WebPages;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Helpers;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.Controllers
{
    [Authorize]
    [RoutePrefix("api/user")]
    public class UserApiController : ApiController
    {
        private readonly IClinicalGuidelinesAppContext _db = new ClinicalGuidelinesAppContext();
        private readonly LogHelper _logHelper = new LogHelper();

        public UserApiController()
        {
        }

        public UserApiController(IClinicalGuidelinesAppContext context)
        {
            _db = context;
        }

        [Route("get/current")]
        public IHttpActionResult GetCurrentUser()
        {
            //Get the user information from AD
            var userDto = User.GetUserBySamAccountName(User.GetUsernameWithoutDomain());

            //Does the user exist in the clinical guidance app? 
            var clinicalUser = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments) 
                .SingleOrDefault(x => x.UserName == userDto.SamAccountName);

            if (clinicalUser == null)
            {
                //create new
                var user = new User
                {
                    _id = Guid.NewGuid(),
                    UserName = userDto.SamAccountName,
                    Forename = userDto.Forename,
                    Surname = userDto.Surname,
                    JobTitle = userDto.JobTitle,
                    PhoneNumber = userDto.PhoneNumber,
                    EmailAddress = userDto.EmailAddress,
                    SiteAdmin = userDto.SiteAdmin,
                    LeadClinician = userDto.LeadClinician,
                    ContentApprover = userDto.ContentApprover,
                    ContentEditor = userDto.ContentEditor,
                    Contact = userDto.Contact
                };

                _db.Logs.Add(
                    _logHelper.CreateUserApiControllerLog(
                        LogHelper.LogType.Create, 
                        "UserApiController.GetCurrentUser", 
                        "api/user/get/current", 
                        user,
                        User.GetUsernameWithoutDomain()
                    )
                );
                
                _db.Users.Add(user);
                _db.SaveChanges();

                userDto.Id = user.Id;
            }
            else
            {
                //overwrite dto with app information
                userDto.Id = clinicalUser.Id;
                userDto._id = clinicalUser._id;
                userDto.DisplayName = clinicalUser.Surname + " " + clinicalUser.Forename + ", " + clinicalUser.JobTitle;
                userDto.Forename = clinicalUser.Forename;
                userDto.Surname = clinicalUser.Surname;
                userDto.JobTitle = clinicalUser.JobTitle;
                userDto.PhoneNumber = clinicalUser.PhoneNumber;
                userDto.EmailAddress = clinicalUser.EmailAddress;
                userDto.SiteAdmin = clinicalUser.SiteAdmin;
                userDto.LeadClinician = clinicalUser.LeadClinician;
                userDto.ContentApprover = clinicalUser.ContentApprover;
                userDto.ContentEditor = clinicalUser.ContentEditor;
                userDto.Contact = clinicalUser.Contact;
                userDto.DepartmentId = clinicalUser.DepartmentId;
                userDto.DepartmentName = (clinicalUser.Department != null) ? clinicalUser.Department.Name : "";

                var administrationDepartments = clinicalUser
                    .AdministationDepartments
                    .Select(clinicalUserAdministrationDepartment => new DepartmentDto()
                    {
                        Id = clinicalUserAdministrationDepartment.Id
                    }
                    )
                    .ToList();

                userDto.AdministationDepartments = administrationDepartments;
            }

            userDto.CurrentServer = GeneralHelper.GetCurrentHostServer();

            return Ok(userDto);
        }

        [Route("get/list")]
        public IHttpActionResult GetUsersByName(string searchName)
        {
            var userDtos = UserHelper.GetUsersByName(searchName);
            return Ok(userDtos);
        }

        [Route("get")]
        public IHttpActionResult GetUsers()
        {
            var users = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments);

            var usersDto = users
                .Select(x => new UserDto()
                    {
                        Id = x.Id,
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
                    }
                );

            return Ok(usersDto);
        }

        [Route("get/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult GetPagedUsers(int pageSize, int pageNumber)
        {
            var totalCount = _db.Users.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var users = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Select(x => new UserDto()
                    {
                        Id = x.Id,
                        _id = x._id,
                        DisplayName = x.Surname + " " + x.Forename + ", " + x.JobTitle,
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
                }
                )
                .OrderBy(x => x.Forename)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Users = users
                }
            );
        }

        [Route("get/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedUsersByKeyword(int pageSize, int pageNumber, string searchTerm)
        {
            var totalCount = _db.Users
                .Include(x => x.Department)
                .Count(x => 
                    x.Forename.Contains(searchTerm) ||
                    x.Surname.Contains(searchTerm) ||
                    x.JobTitle.Contains(searchTerm));
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var users = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Select(x => new UserDto()
                    {
                        Id = x.Id,
                        _id = x._id,
                        DisplayName = x.Surname + " " + x.Forename + ", " + x.JobTitle,
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
                }
                )
                .Where(x =>
                    x.Forename.Contains(searchTerm) ||
                    x.Surname.Contains(searchTerm) ||
                    x.JobTitle.Contains(searchTerm))
                .OrderBy(x => x.Forename)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Users = users
                }
            );
        }

        [Route("get/{id:int}")]
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            var user = _db.Users
                .Include(x => x.Department)
                .Include(x => x.AdministationDepartments)
                .Select(x => 
                    new UserDto(){
                        Id = x.Id,
                        _id = x._id,
                        DisplayName = x.Surname + " " + x.Forename + ", " + x.JobTitle,
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
                    }
                )
                .FirstOrDefault(x => x.Id == id);

            return Ok(user);
        }

        [Route("put/{id:int}/department/{departmentId:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUserDepartment(int id, int departmentId)
        {
            //Check new department is valid
            var department = _db.Departments
                .SingleOrDefault(x => x.Id == departmentId && !x.Archived);

            if (department == null) return BadRequest("Department not found");

            //Get the user information from AD
            var userDto = User.GetUserBySamAccountName(User.GetUsernameWithoutDomain());

            //Does the user exist in the clinical guidance app? 
            var user = _db.Users
                .SingleOrDefault(x => x.UserName == userDto.SamAccountName);

            if (user == null) return BadRequest("User not found");
            if (user.Id != id) return BadRequest("Department can only be changed for current user");

            if (user._id == Guid.Empty) user._id = Guid.NewGuid();
            user.DepartmentId = departmentId;

            _db.Logs.Add(
                _logHelper.CreateUserApiControllerLog(
                    LogHelper.LogType.Update,
                    "UserApiController.PutUserDepartment",
                    $"api/user/put/{id}/department/{departmentId}",
                    user,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkUserAsModified(user);

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

            return Ok();
        }

        [Route("put/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            if (user._id == Guid.Empty) user._id = Guid.NewGuid();
            
            _db.Logs.Add(
                _logHelper.CreateUserApiControllerLog(
                    LogHelper.LogType.Update,
                    "UserApiController.PutUser",
                    $"api/user/put/{id}/",
                    user,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkUserAsModified(user);

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
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (IsUserNameTaken(user.UserName))
            {
                return BadRequest("Username already in database.");
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            user._id = Guid.NewGuid();

            _db.Users.Add(user);

            _db.Logs.Add(
                _logHelper.CreateUserApiControllerLog(
                    LogHelper.LogType.Create,
                    "UserApiController.PostUser",
                    "api/user/post/",
                    user,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok(user);
        }

        [Route("post/administration-department/{userId:int}/{departmentId:int}")]
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUserAdministrationDepartment(int userId, int departmentId)
        {
            if (!IsUserSiteAdmin() && !IsUserLeadClinician() && !IsUserContentApprover() && !IsUserContentCreator())
            {
                return Unauthorized();
            }

            var user = _db.Users
                .Include(x => x.AdministationDepartments)
                .FirstOrDefault(x => x.Id == userId);
            var department = _db.Departments.FirstOrDefault(x => x.Id == departmentId);

            if (user == null || department == null)
            {
                return NotFound();
            }

            user.AdministationDepartments.Add(department);

            _db.MarkUserAsModified(user);
            _db.SaveChanges();

            _db.Logs.Add(
                _logHelper.CreateUserApiControllerLog(
                    LogHelper.LogType.Create,
                    "UserApiController.PostUserAdministrationDepartment",
                    $"api/user/post/administration-department/{userId}/{departmentId}",
                    user,
                    User.GetUsernameWithoutDomain()
                )
            );

            return Ok();
        }

        [Route("delete/administration-department/{userId:int}/{departmentId:int}")]
        public IHttpActionResult DeleteUserAdministrationDepartment(int userId, int departmentId)
        {
            if (!IsUserSiteAdmin() && !IsUserLeadClinician() && !IsUserContentApprover() && !IsUserContentCreator())
            {
                return Unauthorized();
            }

            var user = _db.Users
                .Include(x => x.AdministationDepartments)
                .FirstOrDefault(x => x.Id == userId);
            var department = _db.Departments.FirstOrDefault(x => x.Id == departmentId);

            if (user == null || department == null)
            {
                return NotFound();
            }

            user.AdministationDepartments.Remove(department);

            _db.MarkUserAsModified(user);
            _db.SaveChanges();

            _db.Logs.Add(
                _logHelper.CreateUserApiControllerLog(
                    LogHelper.LogType.Delete,
                    "UserApiController.DeleteUserAdministrationDepartment",
                    $"api/user/delete/administration-department/{userId}/{departmentId}",
                    user,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok();
        }

        [Route("delete/{id:int}")]
        public IHttpActionResult DeleteUser(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            _db.Users.Remove(user);

            _db.Logs.Add(
                _logHelper.CreateUserApiControllerLog(
                    LogHelper.LogType.Delete,
                    "UserApiController.DeleteUser",
                    $"api/user/delete/{id}",
                    user,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok(user);
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

        private bool UserExists(int id)
        {
            return _db.Users.Count(e => e.Id == id) > 0;
        }

        private bool IsUserNameTaken(string userName)
        {
            var user = _db.Users.Where(x => x.UserName == userName).ToList();

            return user.Count > 0;
        }

        private bool IsUserUsed(User user)
        {
            return false; //TODO: Implement this check
        }
    }
}