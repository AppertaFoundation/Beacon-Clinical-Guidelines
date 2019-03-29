//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.Controllers
{
    [Authorize]
    [RoutePrefix("api/article/revision")]
    public class ArticleRevisionApiController : ApiController
    {
        private readonly IClinicalGuidelinesAppContext _db = new ClinicalGuidelinesAppContext();

        public ArticleRevisionApiController()
        {
        }

        public ArticleRevisionApiController(IClinicalGuidelinesAppContext context)
        {
            _db = context;
        }

        [Route("get")]
        public IQueryable<ArticleRevision> GetArticleRevisions()
        {
            return _db.ArticleRevisions;
        }

        [Route("get/{id:int}")]
        [ResponseType(typeof(ArticleRevision))]
        public IHttpActionResult GetArticleRevision(int id)
        {
            var articleRevision = _db.ArticleRevisions.Find(id);
            if (articleRevision == null)
            {
                return NotFound();
            }

            return Ok(articleRevision);
        }

        [Route("put/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutArticleRevision(int id, ArticleRevision articleRevision)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != articleRevision.Id)
            {
                return BadRequest();
            }

            _db.MarkArticleRevisionAsModified(articleRevision);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleRevisionExists(id))
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
        [ResponseType(typeof(ArticleRevision))]
        public IHttpActionResult PostArticleRevision(ArticleRevision articleRevision)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.ArticleRevisions.Add(articleRevision);
            _db.SaveChanges();

            return Ok(articleRevision);
        }

        [Route("delete/{id:int}")]
        [ResponseType(typeof(ArticleRevision))]
        public IHttpActionResult DeleteArticleRevision(int id)
        {
            var articleRevision = _db.ArticleRevisions.Find(id);
            if (articleRevision == null)
            {
                return NotFound();
            }

            _db.ArticleRevisions.Remove(articleRevision);
            _db.SaveChanges();

            return Ok(articleRevision);
        }

        private bool ArticleRevisionExists(int id)
        {
            return _db.ArticleRevisions.Count(e => e.Id == id) > 0;
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