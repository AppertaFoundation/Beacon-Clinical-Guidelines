//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("articleTemplatesCtrl", ["$scope", "$rootScope", "$timeout", "$uibModal", "$sce", "$state", "$location", "cfpLoadingBar", "httpService", "modelService", function ($scope, $rootScope, $timeout, $uibModal, $sce, $state, $location, cfpLoadingBar, httpService, modelService) {
    $scope.loadComplete = false;
    $scope.apiErrors = null;
    $scope.debug = $rootScope.debug;

    $scope.articleTemplates = [];
    $scope.articleTemplate = {};

    var optionSuffix = " templates per page";
    $scope.pageSizeOptions = [{ Id: 5, Name: 5 + optionSuffix }, { Id: 10, Name: 10 + optionSuffix }, { Id: 25, Name: 25 + optionSuffix }, { Id: 50, Name: 50 + optionSuffix }, { Id: 100, Name: 100 + optionSuffix }];
    $scope.pagination = { pageSize: 10, currentPage: 1, totalItems: 10, maxSize: 5, searchTerm: null }

    $scope.setPage = function (pageNo) {
        console.log("select page called:" + pageNo);
        $scope.pagination.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log("Page changed to: " + $scope.pagination.currentPage);
        $scope.paginateArticleTemplates();
    };

    $scope.checkArticleTemplatePermissions = function () {
        if (!$scope.user) return false;
        return $scope.user.SiteAdmin || $scope.user.LeadClinician;
    };

    $scope.paginateArticleTemplates = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            console.log(response);
            $scope.articleTemplates = response.data.ArticleTemplates;
            $scope.pagination.totalItems = response.data.TotalCount;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getPaginatedArticleTemplates.bind(httpService.getPaginatedArticleTemplates, $scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getPaginatedArticleTemplates($scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm).then(successCallback, errorCallback);
    };

    $scope.getArticleTemplate = function (articleTemplateId) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.articleTemplate = response.data;
            if (!$scope.articleTemplate.Structure) $scope.articleTemplate.Structure = [];
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getArticleTemplate, successCallback, errorCallback, articleTemplateId);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getArticleTemplate(articleTemplateId).then(successCallback, errorCallback);
    };

    $scope.addArticleTemplate = function () {
        $scope.articleTemplate = null;
        $state.go("articleTemplates.form");
    };

    $scope.saveArticleTemplate = function () {
        modelService.saveModel("ArticleTemplate", $scope);
    };

    $scope.modelUpdateSuccessful = function () {
        $scope.articleTemplate = null;
        $state.go("articleTemplates.list");
    };
    
    $scope.initialiseArticleTemplatesForm = function () {
        var articleTemplateId = $location.search().articleTemplateId;
        $scope.articleTemplate = {};
        if (articleTemplateId) {
            $scope.getArticleTemplate(articleTemplateId);
        } else {
            if (!$scope.articleTemplate) $scope.articleTemplate = {};
            if (!$scope.articleTemplate.Structure) $scope.articleTemplate.Structure = [];
        }
    };

    $scope.deleteArticleTemplate = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to delete this?")) {
            cfpLoadingBar.start();
            var successCallback = function (success) {
                console.log(success);
                $scope.paginateArticleTemplates();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.deleteArticleTemplate.bind(httpService.deleteArticleTemplate, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.deleteArticleTemplate(id).then(successCallback, errorCallback);
        }
    };

    $scope.addEditBlock = function (type, block) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/articlesModalPopups.html",
            controller: "articleModalsCtrl",
            resolve: {
                type: function () {
                    return type;
                },
                block: function () {
                    return block;
                },
                images: function() {
                    return null;
                }
            }
        });

        modalInstance.result.then(function (result) {
            if (result.value) {
                var block = {};
                if (result.url) {
                    block = {
                        type: type,
                        bold: false,
                        italic: false,
                        input: result.value,
                        url: result.url,
                        template: true,
                        blocks: []
                    };
                } else {
                    block = {
                        type: type,
                        bold: false,
                        italic: false,
                        input: result.value,
                        template: true,
                        blocks: []
                    };
                }

                $scope.articleTemplate.Structure.push(block);
            }
        });
    };

    $scope.removeBlock = function (scope) {
        scope.remove();
    };

    $scope.bold = function (block) {
        block.bold = (block.bold) ? false : true;
    };

    $scope.italic = function (block) {
        block.italic = (block.italic) ? false : true;
    };

    $scope.getIcon = function (type) {
        switch (type) {
            case "text":
                return "fa fa-font";
            case "unordered":
                return "fa fa-list-ul";
            case "ordered":
                return "fa fa-list-ol";
            case "link":
                return "fa fa-link";
        }
        return null;
    };
}]);