//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Models;
using Newtonsoft.Json;

namespace ClinicalGuidelines.Helpers
{
    public class MappingHelper : IMappingHelper
    {
        public List<ArticleRevisionDto> MapArticlesToArticleRevisionDtos(List<Article> articles, bool mapRelated, IClinicalGuidelinesAppContext db = null)
        {
            var articleRevisionDtos = articles.Select(MapArticleToArticleRevisionDto).ToList();
            return (mapRelated) ? MapRelatedArticlesToArticleRevisionDto(articleRevisionDtos, db) : articleRevisionDtos;
        }

        private ArticleRevisionDto MapArticleToArticleRevisionDto(Article article)
        {
            var articleRevisionDto = new ArticleRevisionDto
            {
                Id = article.Id,
                _id = article._id,
                RevisionId = article.CurrentArticleRevision.Id,
                Title = article.CurrentArticleRevision.Title,
                SubTitle = article.CurrentArticleRevision.SubSectionTab.Name,
                Body = article.CurrentArticleRevision.Body,
                RevisionDateTime = article.CurrentArticleRevision.RevisionDateTime,
                SubSectionTabId = article.CurrentArticleRevision.SubSectionTab.Id,
                Terms = new List<TermDto>(),
                Files = (article.CurrentArticleRevision.Files != null) ? JsonConvert.DeserializeObject(article.CurrentArticleRevision.Files) : null,
                Live = article.CurrentArticleRevision.Live,
                Approval = article.CurrentArticleRevision.Approval,
                ApprovalComments = article.CurrentArticleRevision.ApprovalComments,
                Rejected = article.CurrentArticleRevision.Rejected,
                Departments = new List<DepartmentDto>()
            };

            foreach (var department in article.CurrentArticleRevision.Departments)
            {
                articleRevisionDto.Departments.Add(new DepartmentDto()
                {
                    Id = department.Id,
                    _id = department._id,
                    Name = department.Name,
                    ShortName = department.ShortName
                });
            }

            if (article.CurrentArticleRevision.Terms == null) return articleRevisionDto;

            foreach (var term in article.CurrentArticleRevision.Terms)
            {
                articleRevisionDto.Terms.Add(new TermDto()
                {
                    Id = term.Id,
                    Name = term.Name
                });
            }

            return articleRevisionDto;
        }

        public List<ArticleDto> MapArticlesToArticleRevisionDtosAsList(List<Article> articles, IClinicalGuidelinesAppContext db = null)
        {
            return articles.Select(MapArticleToArticleRevisionDtoAsList).ToList();
        }

        private ArticleDto MapArticleToArticleRevisionDtoAsList(Article article)
        {
            var articleRevisionDto = new ArticleDto
            {
                Id = article.Id,
                _id = article._id,
                Title = article.CurrentArticleRevision.Title,
                SubTitle = article.CurrentArticleRevision.SubSectionTab.Name
            };

            return articleRevisionDto;
        }

        public List<ArticleRevisionDto> MapRelatedArticlesToArticleRevisionDto(List<ArticleRevisionDto> articleRevisionDtos, IClinicalGuidelinesAppContext db = null)
        {
            foreach (var articleRevisionDto in articleRevisionDtos)
            {
                articleRevisionDto.RelatedArticleDtos = GetRelatedArticlesFromTaxonomyTerms(articleRevisionDto, db);
            }

            return articleRevisionDtos;
        }

        public ArticleRevisionDto MapRelatedArticleToArticleRevisionDto(ArticleRevisionDto articleRevisionDto, IClinicalGuidelinesAppContext db = null)
        {
            articleRevisionDto.RelatedArticleDtos = GetRelatedArticlesFromTaxonomyTerms(articleRevisionDto, db);

            return articleRevisionDto;
        }

        private List<RelatedArticlesDto> GetRelatedArticlesFromTaxonomyTerms(ArticleRevisionDto articleRevisionDto, IClinicalGuidelinesAppContext db = null)
        {
            var relatedArticleDtos = new List<RelatedArticlesDto>();
            if (articleRevisionDto.Terms == null) return relatedArticleDtos;

            foreach (var term in articleRevisionDto.Terms)
            {
                relatedArticleDtos.AddRange(GetRelatedArticlesFromTaxonomyTermDto(term, articleRevisionDto.Id, db));
            }

            return relatedArticleDtos;
        }

        private List<RelatedArticlesDto> GetRelatedArticlesFromTaxonomyTermDto(TermDto term, int id, IClinicalGuidelinesAppContext db = null)
        {
            var relatedArticleDtos = new List<RelatedArticlesDto>();

            if (db == null) return relatedArticleDtos;
            
            var relatedArticles = db
                    .Articles
                    .Include(x => x.CurrentArticleRevision)
                    .Include(x => x.CurrentArticleRevision.SubSectionTab)
                    .Include(x => x.CurrentArticleRevision.Departments)
                    .Include(x => x.CurrentArticleRevision.Terms)
                    .Where(x => x.CurrentArticleRevision.Terms.Any(y => y.Id == term.Id))
                    .Where(x => x.Id != id)
                    .Where(x => x.CurrentArticleRevision.Live);

            if (!relatedArticles.Any()) return relatedArticleDtos;

            foreach (var relatedArticle in relatedArticles)
            {
                var relatedArticleDto = new RelatedArticlesDto()
                {
                    Id = relatedArticle.Id,
                    _id = relatedArticle._id,
                    SubSectionTabId = relatedArticle.CurrentArticleRevision.SubSectionTab.Id,
                    Name = relatedArticle.CurrentArticleRevision.Title,
                    Departments = new List<DepartmentDto>(),
                    Term = new TermDto()
                    {
                        Id = term.Id,
                        Name = term.Name
                    }
                };

                foreach (var department in relatedArticle.CurrentArticleRevision.Departments)
                {
                    relatedArticleDto.Departments.Add(new DepartmentDto()
                    {
                        Id = department.Id,
                        _id = department._id,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        ContainerId = department.ContainerId,
                        BackgroundColour = department.BackgroundColour
                    });
                }

                relatedArticleDtos.Add(relatedArticleDto);
            }

            return relatedArticleDtos;
        }
    }
}