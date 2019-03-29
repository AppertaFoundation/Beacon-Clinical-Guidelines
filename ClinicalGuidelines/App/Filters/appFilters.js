//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.filter("hrefToJS", function ($sce, $sanitize) {
    return function (text) {
        var firstRegex = /\<\/a\>/g;
        var firstString = $sanitize(text).replace(firstRegex, " [External link]</a>");
        var secondRegex = /href="([\S]+)"/g;
        var secondString = $sanitize(firstString).replace(secondRegex, "href onClick=\"window.open('$1', '_system', 'location=yes');console.log('clicked');return false;\" class=\"btn btn-default app-button\"");

        return $sce.trustAsHtml(secondString);
    }
});

app.filter("getFullImageFunction", function ($sce, $sanitize) {
    return function (text) {
        var firstRegex = /\<img/g;
        var firstString = $sanitize(text).replace(firstRegex, '<script>function downloadFullImage2(scope) { console.log(scope);console.log("called");};</script><img onClick="downloadFullImage2(this)"');

        return $sce.trustAsHtml(firstString);
    }
});

app.filter("formatDateFunction",
    function() {
        return function(date) {
            date = formatDate(date);

            return date;
        }
    });

formatDate = function(date) {
    var momentDate = moment(date);
    return momentDate.format('dddd Do MMM YYYY');
}

hrefToJs = function (text) {
    var firstRegex = /\<\/a\>/g;
     text = text.replace(firstRegex, " [External link]</a>");
    var secondRegex = /href="([\S]+)"/g;
     text = text.replace(secondRegex, "href onClick=\"window.open('$1', '_system', 'location=yes');console.log('clicked');return false;\" class=\"btn btn-default app-button\"");

     return text;
};

getFullImageFunction = function (text) {
    var firstRegex = /\<img/g;
     text = text.replace(firstRegex, '<script>function downloadFullImage2(scope) { console.log(scope);console.log("called");};</script><img onClick="downloadFullImage2(this)"');
  
    return text;
};

app.filter("getFullImageAndHrefToJs", function ($sce, $sanitize) {
    return function (text) {
        text = $sanitize(text);
        text = hrefToJs(text);
        text = getFullImageFunction(text);

        return $sce.trustAsHtml(text);
    }
});
