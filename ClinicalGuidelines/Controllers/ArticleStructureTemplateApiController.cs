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
using Newtonsoft.Json;

namespace ClinicalGuidelines.Controllers
{
    [Authorize]
    [RoutePrefix("api/article/template")]
    public class ArticleStructureTemplateApiController : ApiController
    {
        private readonly IClinicalGuidelinesAppContext _db = new ClinicalGuidelinesAppContext();
        private readonly LogHelper _logHelper = new LogHelper();

        public ArticleStructureTemplateApiController()
        {
        }

        public ArticleStructureTemplateApiController(IClinicalGuidelinesAppContext context)
        {
            _db = context;
        }

        [Route("get")]
        public IHttpActionResult GetArticleStructureTemplates()
        {
            var articleStructureTemplates = _db.ArticleStructureTemplates;

            var articlesStructureTemplatesDto = new List<ArticleStructureTemplateDto>();
            foreach (var articleStructureTemplate in articleStructureTemplates)
            {
                articlesStructureTemplatesDto.Add(new ArticleStructureTemplateDto()
                {
                    Id = articleStructureTemplate.Id,
                    Structure = JsonConvert.DeserializeObject(articleStructureTemplate.Structure),
                    Title = articleStructureTemplate.Title
                });
            }

            return Ok(articlesStructureTemplatesDto);
        }

        [Route("get/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult GetPagedArticleTemplates(int pageSize, int pageNumber)
        {
            var totalCount = _db.ArticleStructureTemplates.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var articleStructureTemplates = _db.ArticleStructureTemplates
                .OrderByDescending(x => x.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var articlesStructureTemplatesDto = new List<ArticleStructureTemplateDto>();
            foreach (var articleStructureTemplate in articleStructureTemplates)
            {
                articlesStructureTemplatesDto.Add(new ArticleStructureTemplateDto()
                {
                    Id = articleStructureTemplate.Id,
                    Structure = JsonConvert.DeserializeObject(articleStructureTemplate.Structure),
                    Title = articleStructureTemplate.Title
                });
            }

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    ArticleTemplates = articlesStructureTemplatesDto
                }
            );
        }

        [Route("get/{pageSize:int}/{pageNumber:int}/{searchTerm}")]
        public IHttpActionResult GetPagedArticlesByKeyword(int pageSize, int pageNumber, string searchTerm)
        {
            var totalCount = _db.ArticleStructureTemplates.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var articleStructureTemplates = _db.ArticleStructureTemplates
                .Where(x => x.Title.Contains(searchTerm))
                .OrderByDescending(x => x.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var articlesStructureTemplatesDto = new List<ArticleStructureTemplateDto>();
            foreach (var articleStructureTemplate in articleStructureTemplates)
            {
                articlesStructureTemplatesDto.Add(new ArticleStructureTemplateDto()
                {
                    Id = articleStructureTemplate.Id,
                    Structure = JsonConvert.DeserializeObject(articleStructureTemplate.Structure),
                    Title = articleStructureTemplate.Title
                });
            }

            return Ok(
                new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    ArticleTemplates = articlesStructureTemplatesDto
                }
            );
        }

        [Route("get/{id:int}")]
        [ResponseType(typeof(ArticleStructureTemplate))]
        public IHttpActionResult GetArticleStructureTemplate(int id)
        {
            var articleStructureTemplate = _db.ArticleStructureTemplates.Find(id);
            if (articleStructureTemplate == null)
            {
                return NotFound();
            }

            var articleStructureTemplateDto = new ArticleStructureTemplateDto()
            {
                Id = articleStructureTemplate.Id,
                Structure = JsonConvert.DeserializeObject(articleStructureTemplate.Structure),
                Title = articleStructureTemplate.Title
            };

            return Ok(articleStructureTemplateDto);
        }

        [Route("put/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutArticleStructureTemplate(int id, ArticleStructureTemplateDto articleStructureTemplateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            if (id != articleStructureTemplateDto.Id)
            {
                return BadRequest();
            }

            var articleStructureTemplate = new ArticleStructureTemplate()
            {
                Id = articleStructureTemplateDto.Id,
                Structure = JsonConvert.SerializeObject(articleStructureTemplateDto.Structure),
                Title = articleStructureTemplateDto.Title
            };

            _db.Logs.Add(
                _logHelper.CreateArticleStructureTemplateApiControllerLog(
                    LogHelper.LogType.Update,
                    "ArticleStructureTemplateApiController.PutArticleStructureTemplate",
                    $"api/article/template/put/{id}",
                    articleStructureTemplate,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.MarkArticleStructureTemplateAsModified(articleStructureTemplate);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleStructureTemplateExists(id))
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
        [ResponseType(typeof(ArticleStructureTemplate))]
        public IHttpActionResult PostArticleStructureTemplate(ArticleStructureTemplateDto articleStructureTemplateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            var articleStructureTemplate = new ArticleStructureTemplate()
            {
                Id = articleStructureTemplateDto.Id,
                Structure = JsonConvert.SerializeObject(articleStructureTemplateDto.Structure),
                Title = articleStructureTemplateDto.Title
            };

            _db.ArticleStructureTemplates.Add(articleStructureTemplate);

            _db.Logs.Add(
                _logHelper.CreateArticleStructureTemplateApiControllerLog(
                    LogHelper.LogType.Create,
                    "ArticleStructureTemplateApiController.PostArticleStructureTemplate",
                    "api/article/template/post/",
                    articleStructureTemplate,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok(articleStructureTemplateDto);
        }

        [Route("delete/{id:int}")]
        [ResponseType(typeof(ArticleStructureTemplate))]
        public IHttpActionResult DeleteArticleStructureTemplate(int id)
        {
            var articleStructureTemplate = _db.ArticleStructureTemplates.Find(id);
            if (articleStructureTemplate == null)
            {
                return NotFound();
            }

            if (!IsUserSiteAdmin() && !IsUserLeadClinician())
            {
                return Unauthorized();
            }

            _db.ArticleStructureTemplates.Remove(articleStructureTemplate);

            _db.Logs.Add(
                _logHelper.CreateArticleStructureTemplateApiControllerLog(
                    LogHelper.LogType.Delete,
                    "ArticleStructureTemplateApiController.DeleteArticleStructureTemplate",
                    $"api/article/template/delete/{id}",
                    articleStructureTemplate,
                    User.GetUsernameWithoutDomain()
                )
            );

            _db.SaveChanges();

            return Ok(articleStructureTemplate);
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

        private bool ArticleStructureTemplateExists(int id)
        {
            return _db.ArticleStructureTemplates.Count(e => e.Id == id) > 0;
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