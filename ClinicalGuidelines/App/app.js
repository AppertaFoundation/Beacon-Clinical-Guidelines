//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿marked.setOptions({
    renderer: new marked.Renderer(),
    gfm: true,
    tables: true,
    breaks: false,
    pedantic: false,
    sanitize: true,
    smartLists: true,
    smartypants: false
});

var app = angular
    .module("app", [
        "cfp.loadingBar",
        "ui.bootstrap",
        "ui.router",
        "ngSanitize",
        "ngFileUpload",
        "mdColorMenu",
        "ui.tree",
        "angular.filter",
    ])
    .run(["$rootScope", "$timeout", "$location", "$templateCache", function ($rootScope, $timeout, $location, $templateCache) {
        $rootScope.debug = false;
        $rootScope.user = null;
        $rootScope.reloadAuthSrc = null;
        $rootScope.httpServiceLocation = ""; //blank for live/test/uat only need this prefix for localhost
        $rootScope.errorRetry = false;

        //This function is a check to see if the error was caused by the client 
        //auth running out, load the main page in the iframe settling the claims
        //then retry the call one time only
        $rootScope.retryApiCallAfterError = function (method, successCallback, errorCallback) {
            if (
                typeof method !== "function" ||
                typeof successCallback !== "function" ||
                typeof errorCallback !== "function"
            ) return; //Sanity check

            $rootScope.errorRetry = true;
            $rootScope.reloadAuthSrc = ($rootScope.httpServiceLocation) ? $rootScope.httpServiceLocation : $location.protocol() + "://" + $location.host() + ":" + $location.port();
            console.log($rootScope.reloadAuthSrc);

            $timeout(function () {
                method().then(successCallback, errorCallback);
                $rootScope.reloadAuthSrc = null; //flush it for any subsequent errors
            }, 2000);
        }

        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            //console.log("Route Change");
            //console.log(toState);
            if (typeof (toState) !== 'undefined') {
                //console.log("Removing template cache for " + toState.templateUrl);
                $templateCache.remove(toState.templateUrl);
            }
        });
    }])
    .config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state("web", {
                cache: false,
                abstract: true,
                templateUrl: "./App/Views/web.html"
            })
            .state("web.list", {
                cache: false,
                url: "/?:containerId&:departmentId&:subSectionTabId&:articleId&:searchText",
                templateUrl: "./App/Views/web/webList.html"
            })
            .state("web.image", {
                cache: false,
                url: "/image",
                templateUrl: "./App/Views/web/webImage.html"
            })
            .state("users", {
                cache: false,
                url: "/users",
                templateUrl: "./App/Views/users.html"
            })
            .state("users.list", {
                cache: false,
                url: "/list",
                templateUrl: "./App/Views/users/usersList.html"
            })
            .state("users.form", {
                cache: false,
                url: "/form?:userId",
                templateUrl: "./App/Views/users/usersForm.html"
            })
            .state("staticphonenumbers", {
                cache: false,
                url: "/staticphonenumbers",
                templateUrl: "./App/Views/staticPhoneNumbers.html"
            })
            .state("departments", {
                cache: false,
                url: "/departments",
                abstract: true,
                templateUrl: "./App/Views/departments.html"
            })
            .state("departments.list", {
                cache: false,
                url: "/list",
                templateUrl: "./App/Views/departments/departmentsList.html"
            })
            .state("departments.form", {
                cache: false,
                url: "/form?:departmentId",
                templateUrl: "./App/Views/departments/departmentsForm.html"
            })
            .state("subSectionTabs", {
                cache: false,
                url: "/subSectionTabs",
                abstract: true,
                templateUrl: "./App/Views/subSectionTabs.html"
            })
            .state("subSectionTabs.list", {
                cache: false,
                url: "/list",
                templateUrl: "./App/Views/subSectionTabs/subSectionTabsList.html"
            })
            .state("subSectionTabs.form", {
                cache: false,
                url: "/form?:subSectionTabId",
                templateUrl: "./App/Views/subSectionTabs/subSectionTabsForm.html"
            })
            .state("articles", {
                cache: false,
                url: "/articles",
                abstract: true,
                templateUrl: "./App/Views/articles.html"
            })
            .state("articles.list", {
                cache: false,
                url: "/list",
                templateUrl: "./App/Views/articles/articlesList.html"
            })
            .state("articles.approvals", {
                cache: false,
                url: "/approvals",
                templateUrl: "./App/Views/articles/articlesApproval.html"
            })
            .state("articles.form", {
                cache: false,
                url: "/form?:articleId&:revisionId",
                templateUrl: "./App/Views/articles/articlesForm.html"
            })
            .state("articles.image", {
                cache: false,
                url: "/image",
                templateUrl: "./App/Views/articles/articlesImage.html"
            })
            .state("articleTemplates", {
                cache: false,
                url: "/articleTempates",
                abstract: true,
                templateUrl: "./App/Views/articleTemplates.html"
            })
            .state("articleTemplates.list", {
                cache: false,
                url: "/list",
                templateUrl: "./App/Views/articleTemplates/articleTemplatesList.html"
            })
            .state("articleTemplates.form", {
                cache: false,
                url: "/form?:articleTemplateId",
                templateUrl: "./App/Views/articleTemplates/articleTemplatesForm.html"
            });

        $urlRouterProvider.otherwise("/");
    }]);