//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Models;
using Newtonsoft.Json;

namespace ClinicalGuidelines.Helpers
{
    public class LogHelper
    {
        private Log _log;

        public enum LogType
        {
            Create,
            Update,
            Delete,
            Archive
        }

        private void SetLog(string samAccountName, string methodCalled, string requestUri)
        {
            _log = new Log
            {
                SamAccountName = samAccountName,
                MethodCalled = methodCalled,
                RequestUri = requestUri,
            };
        }

        private void SetResponse(string response)
        {
            _log.Response = response;
        }

        private Log GetLog()
        {
            _log.DateTimeCalled = DateTime.Now;
            return _log;
        }

        public Log CreateUserApiControllerLog(LogType notificationType, string method, string requestUri, User user, string samAccountName)
        {
            object action;
            var details = new UserDto
            {
                Id = user.Id,
                _id = user._id,
                DisplayName = user.Surname + " " + user.Forename + ", " + user.JobTitle,
                Forename = user.Forename,
                Surname = user.Surname,
                JobTitle = user.JobTitle,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                SiteAdmin = user.SiteAdmin,
                LeadClinician = user.LeadClinician,
                ContentApprover = user.ContentApprover,
                ContentEditor = user.ContentEditor,
                Contact = user.Contact,
                DepartmentId = user.DepartmentId,
                CurrentServer = GeneralHelper.GetCurrentHostServer()
            };

            switch (notificationType)
            {
                case LogType.Create:
                    action = new
                    {
                        Action = $"Account created {user.UserName}.",
                        Details = details
                    };
                    break;
                case LogType.Update:
                    action = new
                    {
                        Action = $"Account updated {user.UserName}.",
                        Details = details
                    };
                    break;
                case LogType.Delete:
                    action = new
                    {
                        Action = $"Account deleted {user.UserName}.",
                        Details = details
                    };
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? CreateLog(samAccountName, method, requestUri, action) : null;
        }

        public Log CreateDepartmentApiControllerLog(LogType notificationType, string method, string requestUri, Department department, string samAccountName)
        {
            object action;
            var details = new DepartmentDto
            {
                Id = department.Id,
                _id = department._id,
                Name = department.Name,
                ShortName = department.ShortName,
                Archived = department.Archived,
                Live = department.Live,
                ContainerId = department.ContainerId
            };

            switch (notificationType)
            {
                case LogType.Create:
                    action = new
                    {
                        Action = $"Department created {department.Name}.",
                        Details = details
                    };
                    break;
                case LogType.Update:
                    action = new
                    {
                        Action = $"Department updated {department.Name}.",
                        Details = details
                    };
                    break;
                case LogType.Archive:
                    action = new
                    {
                        Action = $"Department archived {department.Name}.",
                        Details = details
                    };
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? CreateLog(samAccountName, method, requestUri, action) : null;
        }

        public Log CreateStaticPhoneNumberApiControllerLog(LogType notificationType, string method, string requestUri, StaticPhoneNumber staticPhoneNumber, string samAccountName)
        {
            object action;
            var details = new StaticPhoneNumberDto()
            {
                Id = staticPhoneNumber.Id,
                _id = staticPhoneNumber._id,
                Type = staticPhoneNumber.Type,
                TypeName = Enum.GetName(typeof(StaticPhoneNumberType), staticPhoneNumber.Type),
                Title = staticPhoneNumber.Title,
                PhoneNumber = staticPhoneNumber.PhoneNumber,
                DepartmentId = staticPhoneNumber.DepartmentId,
                DepartmentName = (staticPhoneNumber.Department != null && 
                                    staticPhoneNumber.Department.Name != null) 
                                ? staticPhoneNumber.Department.Name : ""
            };

            switch (notificationType)
            {
                case LogType.Create:
                    action = new
                    {
                        Action = $"Static phone number created {staticPhoneNumber.Title}.",
                        Details = details
                    };
                    break;
                case LogType.Update:
                    action = new
                    {
                        Action = $"Static phone number updated {staticPhoneNumber.Title}.",
                        Details = details
                    };
                    break;
                case LogType.Delete:
                    action = new
                    {
                        Action = $"Static phone number deleted {staticPhoneNumber.Title}.",
                        Details = details
                    };
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? CreateLog(samAccountName, method, requestUri, action) : null;
        }

        public Log CreateSubSectionTabApiControllerLog(LogType notificationType, string method, string requestUri, SubSectionTab subSectionTab, string samAccountName)
        {
            object action;
            var details = new SubSectionTabDto()
            {
                Id = subSectionTab.Id,
                _id = subSectionTab._id,
                Name = subSectionTab.Name,
                Archived = subSectionTab.Archived,
                Icon = subSectionTab.Icon,
                Live = subSectionTab.Live,
                DepartmentId = subSectionTab.DepartmentId
            };

            switch (notificationType)
            {
                case LogType.Create:
                    action = new
                    {
                        Action = $"Subsectiontab created {subSectionTab.Name}.",
                        Details = details
                    };
                    break;
                case LogType.Update:
                    action = new
                    {
                        Action = $"Subsectiontab updated {subSectionTab.Name}.",
                        Details = details
                    };
                    break;
                case LogType.Archive:
                    action = new
                    {
                        Action = $"Subsectiontab archive {subSectionTab.Name}.",
                        Details = details
                    };
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? CreateLog(samAccountName, method, requestUri, action) : null;
        }

        public Log CreateArticleStructureTemplateApiControllerLog(LogType notificationType, string method, string requestUri, ArticleStructureTemplate articleStructureTemplate, string samAccountName)
        {
            object action;
            var details = new ArticleStructureTemplateDto()
            {
                Id = articleStructureTemplate.Id,
                Title = articleStructureTemplate.Title,
                Structure = articleStructureTemplate.Structure
            };

            switch (notificationType)
            {
                case LogType.Create:
                    action = new
                    {
                        Action = $"Article template created {articleStructureTemplate.Title}.",
                        Details = details
                    };
                    break;
                case LogType.Update:
                    action = new
                    {
                        Action = $"Article template updated {articleStructureTemplate.Title}.",
                        Details = details
                    };
                    break;
                case LogType.Delete:
                    action = new
                    {
                        Action = $"Article template delete {articleStructureTemplate.Title}.",
                        Details = details
                    };
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? CreateLog(samAccountName, method, requestUri, action) : null;
        }

        public Log CreateArticleApiControllerLog(LogType notificationType, string method, string requestUri, ArticleRevisionDto article, string samAccountName)
        {
            object action;
            var details = new ArticleDto()
            {
                Id = article.Id,
                _id = article._id,
                RevisionId = article.RevisionId,
                Title = article.Title,
                SubTitle = article.SubTitle
            };

            switch (notificationType)
            {
                case LogType.Create:
                    action = new
                    {
                        Action = $"Article created {article.Id}.",
                        Details = details
                    };
                    break;
                case LogType.Update:
                    action = new
                    {
                        Action = $"Article updated {article.Id}.",
                        Details = details
                    };
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? CreateLog(samAccountName, method, requestUri, action) : null;
        }

        public Log CreateArticleApiControllerArchiveLog(LogType notificationType, string method, string requestUri, Article article, string samAccountName)
        {
            object action;
            var details = new ArticleDto()
            {
                Id = article.Id,
                _id = article._id,
                RevisionId = article.ArticleRevisionId,
                Title = (article.CurrentArticleRevision != null) ? article.CurrentArticleRevision.Title : "",
                SubTitle = (article.CurrentArticleRevision?.SubSectionTab != null) ? article.CurrentArticleRevision.SubSectionTab.Name : ""
            };

            switch (notificationType)
            {
                case LogType.Archive:
                    action = new
                    {
                        Action = $"Article archived {article.Id}.",
                        Details = details
                    };
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? CreateLog(samAccountName, method, requestUri, action) : null;
        }

        private Log CreateLog(string samAccountName, string methodCalled, string requestUri, object action)
        {
            SetLog(samAccountName, methodCalled, requestUri);
            SetResponse(JsonConvert.SerializeObject(action));
            return GetLog();
        }
    }
}