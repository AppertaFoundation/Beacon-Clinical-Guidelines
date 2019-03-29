//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.Helpers
{
    public interface IMappingHelper
    {
        List<ArticleRevisionDto> MapArticlesToArticleRevisionDtos(List<Article> articles, bool mapRelated, IClinicalGuidelinesAppContext db = null);

        List<ArticleDto> MapArticlesToArticleRevisionDtosAsList(List<Article> articles, IClinicalGuidelinesAppContext db = null);

        List<ArticleRevisionDto> MapRelatedArticlesToArticleRevisionDto(List<ArticleRevisionDto> articleRevisionDtos, IClinicalGuidelinesAppContext db = null);

        ArticleRevisionDto MapRelatedArticleToArticleRevisionDto(ArticleRevisionDto articleRevisionDto, IClinicalGuidelinesAppContext db = null);
    }
}
