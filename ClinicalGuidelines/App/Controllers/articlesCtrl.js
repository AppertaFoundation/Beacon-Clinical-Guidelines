//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("articlesCtrl", ["$scope", "$rootScope", "$q", "mdPickerColors", "$timeout", "$window", "$filter", "$uibModal", "$sce", "$state", "$location", "cfpLoadingBar", "httpService", "modelService", "fileService", function ($scope, $rootScope, $q, mdPickerColors, $timeout, $window, $filter, $uibModal, $sce, $state, $location, cfpLoadingBar, httpService, modelService, fileService ) {
    $scope.loadComplete = false;
    $scope.apiErrors = null;
    $scope.debug = $rootScope.debug;

    $scope.templates = ["Standard", "Simple"];

    $scope.articles = [];
    $scope.article = { "Terms": [], "images": [] };
    $scope.articleFormLocked = false;
    $scope.uneditedArticle = null;

    $scope.articleHistory = [];
    $scope.articleHistoryToggle = false;
    $scope.articleUnliveRevisions = false;

    $scope.articleDepartmentOptions = [];

    $scope.departments = [];
    $scope.department = {};
    $scope.departmentContainers = [];

    $scope.articleTemplates = [];
    
    $scope.subSectionTabs = [];
    $scope.globalSubSectionTabs = [];

    $scope.dateRb12 = false;
    $scope.dateRb24 = false;
    $scope.dateRb36 = false;

    $scope.calendarSelectedDate = { date: new Date };
    $scope.monthFromTodayDate = new Date();
    $scope.newProposedDate = new Date();
    $scope.viewCalendar = false;
    $scope.expiryStatusSet = false;

    $scope.data = {
        group1: 'datesAhead'
    };

    $scope.radioButtonData = [
        { label: '12 Months', value: 12, checked: false },
        { label: '24 Months', value: 24, checked: false },
        { label: '36 Months', value: 36, checked: false }
    ];

    $scope.getGlobalSubSectionTabs = function() {
        $scope.loadComplete = false;
        $rootScope.errorRetry = false;
        $scope.returnedSubSections = [];
        var successCallback = function (response) {
            $scope.loadComplete = true;
            $scope.globalSubSectionTabs = response.data;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getGlobalSubSectionTabs, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, unable to load list of departments sub sections.";
                }
            }
        };
        httpService.getGlobalSubSectionTabs().then(successCallback, errorCallback);
    }

    $scope.getGlobalSubSectionTabs();

    $scope.getDepartmentContainers = function () {
        $scope.loadComplete = false;
        $rootScope.errorRetry = false;
        $scope.returnedSubSections = [];
        var successCallback = function (response) {
            $scope.loadComplete = true;
            $scope.departmentContainers = response.data;
            console.log($scope.departmentContainers);
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getDepartmentContainers, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, unable to load list of departments sub sections.";
                }
            }
        };
        httpService.getDepartmentContainers().then(successCallback, errorCallback);
    }
    //$scope.getDepartmentContainers();

    $scope.getEditabeDepartmentContainers = function () {
        $scope.loadComplete = false;
        $rootScope.errorRetry = false;
        $scope.returnedSubSections = [];
        var successCallback = function (response) {
            $scope.loadComplete = true;
            $scope.departmentContainers = response.data;
            console.log($scope.departmentContainers);
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getEditabeDepartmentContainers, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, unable to load list of departments sub sections.";
                }
            }
        };
        httpService.getEditabeDepartmentContainers().then(successCallback, errorCallback);
    }
    $scope.getEditabeDepartmentContainers();

    var optionSuffix = " articles per page";
    $scope.pageSizeOptions = [{ Id: 5, Name: 5 + optionSuffix }, { Id: 10, Name: 10 + optionSuffix }, { Id: 25, Name: 25 + optionSuffix }, { Id: 50, Name: 50 + optionSuffix }, { Id: 100, Name: 100 + optionSuffix }];
    $scope.pagination = { pageSize: 10, currentPage: 1, totalItems: 10, maxSize: 5, searchTerm: null };

    $scope.setPage = function (pageNo) {
        console.log("select page called:" + pageNo);
        $scope.pagination.currentPage = pageNo;
    };

    $scope.pageChanged = function (type) {
        console.log("Page changed to: " + $scope.pagination.currentPage + "; type: " + type);
        $scope.paginateArticles();
    };

    $scope.resetSubSectionTabs = function() {
        console.log("Resetting subsectiontabs");
        $scope.subSectionTabs = [];
    };

    $scope.pageChangedApprovals = function () {
        console.log("Page changed to: " + $scope.pagination.currentPage);
        console.log($scope.pagination);
        $scope.paginateArticleApprovals();
    };

    $scope.toggleExtraFilters = function() {
        if (!$scope.pagination) return;
        $scope.pagination.showExtraFilters = ($scope.pagination.showExtraFilters) ? false : true;
    };

    $scope.paginateArticles = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.article = null;
            $scope.uneditedArticle = null;
            $scope.articles = response.data.Articles;
            $scope.pagination.totalItems = response.data.TotalCount;
            $scope.loadComplete = true;

            //if there is a area selected departments is filtered otherwise all departments are shown
            if ($scope.pagination.container) {
                $scope.getContainerDepartments();
            } else {

                if (!$scope.subSectionTabs.length) $scope.getEditableDepartments();
            }

            if ($scope.pagination.department) {
                $scope.getDepartmentSubSectionTabs($scope.pagination.department);
            } 

            for (var i = 0; i < $scope.articles.length; i++) {
                if ($scope.articles[i].Departments) {
                    for (var j = 0; j < $scope.articles[i].Departments.length; j++) {
                        $scope.articles[i].Departments[j].Container = $filter("filter")($scope.departmentContainers, { Id: $scope.articles[i].Departments[j].ContainerId }, true)[0];
                    }
                }
            }
        };
        var errorCallback = function (error) {
            console.log(error);
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getPaginatedArticles.bind(httpService.getPaginatedArticles, $scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.container, $scope.pagination.department, $scope.pagination.subSectionTab, $scope.pagination.searchTerm), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getPaginatedArticles($scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.container, $scope.pagination.department, $scope.pagination.subSectionTab, $scope.pagination.searchTerm).then(successCallback, errorCallback);
    };

    $scope.paginateArticleApprovals = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            console.log(response);
            $scope.article = null;
            $scope.uneditedArticle = null;
            $scope.articles = response.data.Articles;
            $scope.pagination.totalItems = response.data.TotalCount;
            $scope.loadComplete = true;

            $scope.getEditableDepartments();

            for (var i = 0; i < $scope.articles.length; i++) {
                if ($scope.articles[i].Departments) {
                    for (var j = 0; j < $scope.articles[i].Departments.length; j++) {
                        $scope.articles[i].Departments[j].Container = $filter("filter")($scope.departmentContainers, { Id: $scope.articles[i].Departments[j].ContainerId }, true)[0];
                    }
                }
            }
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getPaginatedArticleApprovals.bind(httpService.getPaginatedArticleApprovals, $scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getPaginatedArticleApprovals($scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm).then(successCallback, errorCallback);
    };

    $scope.getArticles = function() {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function(response) {
            cfpLoadingBar.complete();
            $scope.article = null;
            $scope.uneditedArticle = null;
            $scope.articles = response.data;
            $scope.loadComplete = true;
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getArticles, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getArticles().then(successCallback, errorCallback);
    };

    var downloadAndUpdateImage = function (index, article, fileName) {
        setTimeout(function() {
            var livePreviewWidth = document.getElementById("mobilePreviewArea").offsetWidth;
            fileService.downloadImage(article.Files[index].Id + article.Files[index].Extension, livePreviewWidth).then(
                function (response) {
                    $scope.article.images[fileName].image = response.data;
                }
            );
        }, 500);
    };

    $scope.downloadFullImage = function (image) {
        $scope.fullImage = null;
        fileService.downloadFullImage(image.Id + image.Extension).then(
            function (response) {
                $scope.fullImage = response.data;
                $state.go("articles.image");
            }
        );
    };

    $scope.downloadFullImageByFileName = function (fileName) {
        $scope.fullImage = null;

        fileService.downloadFullImage(fileName).then(
            function (response) {
                $scope.fullImage = response.data;
                $state.go("articles.image");
            }
        );
    };

    $scope.historyBack = function() {
        $window.history.back();
    };

    var createImagesArrayFromFilesDto = function (article) {
        if (!article.Files || !Array.isArray(article.Files)) return;
        if (!article.images) article.images = {};

        for (var i = 0; i < article.Files.length; i++) {
            if (parseInt(article.Files[i].Type) === 0) { //Enum 0 = Image
                var fileName = article.Files[i].OriginalFileName;
                $scope.article.images[fileName] = article.Files[i];
                $scope.article.images[fileName].PreviewId = i;
                downloadAndUpdateImage(i, article, fileName);
            }
        }
    };

    $scope.toggleArticleHistory = function() {
        $scope.articleHistoryToggle = ($scope.articleHistoryToggle) ? false : true;
    };

    $scope.articleExpiryDate;

    $scope.getArticle = function (articleId) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        $scope.articleFormLocked = false;
        cfpLoadingBar.start();
        $scope.subSectionTabs = [];
        var successCallback = function(response) {
            cfpLoadingBar.complete();
            $scope.article = response.data;

            if ($scope.article.ExpiryDateTime) {
                $scope.article.ExpiryDateTime = $scope.convertCsharpDateToJavacript($scope.article.ExpiryDateTime);
                $scope.newProposedDate = $scope.article.ExpiryDateTime;
                $scope.expiryStatusSet = true;
            } else {
                $scope.expiryStatusSet = false;
                $scope.article.ExpiryDateTime = Date();
            }


            if ($scope.article.Approval) $scope.articleFormLocked = true;
            $scope.uneditedArticle = angular.copy($scope.article);
            if (!$scope.article.Structure) $scope.article.Structure = [];
            if ($scope.article.Departments[0].Id) $scope.getDepartment($scope.article.Departments[0].Id);

            $scope.articleDepartmentOptions = [];

            $scope.subSectionTabs = angular.copy($scope.globalSubSectionTabs);

            for (var i = 0; i < $scope.article.Departments.length; i++) {
                $scope.articleDepartmentOptions.push($scope.article.Departments[i].Id);
                $scope.getDepartmentSubSectionTabs($scope.article.Departments[i].Id);
            }

            $scope.getEditableDepartments();
            $scope.getArticleTemplates();
            $scope.getArticleHistory(articleId);

            createImagesArrayFromFilesDto($scope.article);

            $scope.loadComplete = true;

            $scope.processedBody = null;
            processBody();
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getArticle, successCallback, errorCallback, articleId);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getArticle(articleId).then(successCallback, errorCallback);
    };

    $scope.getArticleRevision = function (articleId, revisionId) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        $scope.articleFormLocked = false;
        cfpLoadingBar.start();
        $scope.subSectionTabs = [];
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.article = response.data;

            if ($scope.article.ExpiryDateTime) {
                $scope.article.ExpiryDateTime = $scope.convertCsharpDateToJavacript($scope.article.ExpiryDateTime);
                $scope.newProposedDate = $scope.article.ExpiryDateTime;
                $scope.expiryStatusSet = true;
            } else {
                $scope.expiryStatusSet = false;
                $scope.article.ExpiryDateTime = Date();
            }

            if ($scope.article.Approval) $scope.articleFormLocked = true;
            $scope.uneditedArticle = angular.copy($scope.article);
            if (!$scope.article.Structure) $scope.article.Structure = [];
            if ($scope.article.Departments[0].Id) $scope.getDepartment($scope.article.Departments[0].Id);

            $scope.articleDepartmentOptions = [];
            $scope.subSectionTabs = angular.copy($scope.globalSubSectionTabs);

            for (var i = 0; i < $scope.article.Departments.length; i++) {
                $scope.articleDepartmentOptions.push($scope.article.Departments[i].Id);
                $scope.getDepartmentSubSectionTabs($scope.article.Departments[i].Id);
            }

            $scope.getEditableDepartments();
            $scope.getArticleTemplates();
            $scope.getArticleHistory(articleId);

            createImagesArrayFromFilesDto($scope.article);

            $scope.loadComplete = true;

            $scope.processedBody = null;
            processBody();
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getArticleRevision, successCallback, errorCallback, articleId, revisionId);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getArticleRevision(articleId, revisionId).then(successCallback, errorCallback);
    };

    $scope.removeArticleImage = function(imageKey) {
        delete $scope.article.images[imageKey];
    };
    
    $scope.getArticleHistory = function(articleId) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.articleHistory = response.data;
            //If the last revision isn't live set a flag to show this onscreen as the live revision is the default loaded.
            $scope.articleUnliveRevisions = ($scope.articleHistory[$scope.articleHistory.length - 1].Live) ? false : true;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getArticleHistory, successCallback, errorCallback, articleId);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getArticleHistory(articleId).then(successCallback, errorCallback);
    };

    $scope.addArticle = function() {
        $scope.article = null;
        $scope.uneditedArticle = {};
        console.log('ARTICLES FORM: ' + "articles.form");
        $state.go("articles.form");
    };

    $scope.processedBody = null;
    $scope.processingBodyStarted = false;

    var processBody = function () {
        var articleBody = $scope.article.Body;

        if (!$scope.processedBody && !$scope.processingBodyStarted) {
            // 1. This hasn't been processed or was cleared and hasn't already started
            var splitArticle = articleBody.split("\n");
            // Array for putting the index of all found images
            var foundImageIndexes = [];
            for (var i = 0; i < splitArticle.length; i++) {
                if (splitArticle[i].indexOf("@Image-URL=") !== -1) {
                    foundImageIndexes.push(i);
                }
            }

            if (foundImageIndexes.length === 0) {
                // 1a. No found images so no need to process the body set it to processed body and sent back
                $scope.processedBody = $sce.trustAsHtml(marked(articleBody));
            } else {
                // 1b. There are images set up some promises to download the new images
                var promises = [];
                var livePreviewWidth = document.getElementById("mobilePreviewArea").offsetWidth;
                console.log('Live preview width: ' + livePreviewWidth);

                // Starting to download the images is the labour intensive part so added toggle to not redo this if already begun
                $scope.processingBodyStarted = true;
                for (var j = 0; j < foundImageIndexes.length; j++) {
                    var thisIndex = foundImageIndexes[j];
                    var splitImage = splitArticle[thisIndex].split("=").pop();
                    
                    var promise = (function () {
                        var key = thisIndex;
                        var thisImage = splitImage;
                        return fileService
                            .downloadImageWidth(thisImage, livePreviewWidth)
                            .then(
                            function (response) {
                                // Response to http call processed then passed back wrapped in a $q.when so that the $q.all can resolve all promises
                                console.log(response);
                                return $q.when({
                                    key: key,
                                    image: "![" + thisImage + "](data:image/png;base64," + response.data + ")"
                                });
                            },
                            function (error) {
                                console.log(error);
                                return $q.when(error);
                            }
                        );
                    })();
                    promises.push(promise);
                };

                // When all promises are resolved then rebuild the article
                return $q.all(promises).then(
                    function (promiseArrayResponses) {
                        $scope.processingBodyStarted = false;
                        for (var i = 0; i < promiseArrayResponses.length; i++) {
                            if (promiseArrayResponses[i].key && promiseArrayResponses[i].image) {
                                splitArticle[promiseArrayResponses[i].key] = promiseArrayResponses[i].image;
                            }
                        }
                        updatedArticle = splitArticle.join("\n");
                        $scope.processedBody = $sce.trustAsHtml(marked(updatedArticle));
                    }
                );
            }
        } 
    }

    var isArticleChangedAgainstUneditedVersion = function() {
        var changed = false;

        if ($scope.uneditedArticle.Title !== $scope.article.Title) changed = true;
        if ($scope.uneditedArticle.DepartmentName !== $scope.article.DepartmentName) changed = true;
        if ($scope.uneditedArticle.Template !== $scope.article.Template) changed = true;
        if ($scope.uneditedArticle.SubTitle !== $scope.article.SubTitle) changed = true;
        if ($scope.uneditedArticle.SubSectionTabId !== $scope.article.SubSectionTabId) changed = true;
        if ($scope.uneditedArticle.Body !== $scope.article.Body) changed = true;
        if (Array.isArray($scope.uneditedArticle.Terms) &&
                Array.isArray($scope.article.Terms) &&
                $scope.uneditedArticle.Terms.length !== $scope.article.Terms.length) changed = true;

        return changed;
    };

    $scope.saveArticle = function () {
        console.log('saveArticle - $scope.expiryStatusSet', $scope.expiryStatusSet);
        if ($scope.expiryStatusSet) {
            $scope.article.ExpiryDateTime = $scope.newProposedDate;
        } else {
            $scope.article.ExpiryDateTime = null;
            console.log('saveArticle - $scope.article.ExpiryDateTime: ', $scope.article.ExpiryDateTime);
        }

        $scope.article.revisionId = null;
        for (var i = 0; i < $scope.articleHistory.length; i++) {
            if ($scope.articleHistory[i].RevisionDateTime === $scope.article.RevisionDateTime) {
                $scope.article.revisionId = $scope.articleHistory[i].Id;
            }
        }

        //1. Completely new article
        if (!$scope.article.Id) {
            return modelService.saveModel("ArticleNew", $scope);
        }

        var liveRevisionsFilterArray = $filter("filter")($scope.articleHistory, { Live: true });
        var currentLiveRevision = (liveRevisionsFilterArray) ? liveRevisionsFilterArray[0] : null;
        var lastElement = $scope.articleHistory.length - 1;
        var latestRevision = $scope.articleHistory[lastElement];

        //There is no current live revision
        if (!currentLiveRevision) {
            //2 & 3. The latest revision is the one being edited - save over the top of that revision, else save it as a new revision
            return (latestRevision.RevisionDateTime === $scope.article.RevisionDateTime) ? modelService.saveModel("Article", $scope) : modelService.saveModel("ArticleNew", $scope);
        } else {
            //4. If the revision being edited is the same as the live one and it is only changing to offline save over the top of current revision
            //      if there were edits to the page this shouldn't be picked up here and should be created as a new revision below
            if (currentLiveRevision.RevisionDateTime === $scope.article.RevisionDateTime &&
                !isArticleChangedAgainstUneditedVersion() &&
                $scope.uneditedArticle.Live === true && $scope.article.Live === false) return modelService.saveModel("Article", $scope);

            //5 & 6. The revision is newer save it over the top, else save it as a new revision 
            return (currentLiveRevision.RevisionDateTime < $scope.article.RevisionDateTime) ? modelService.saveModel("Article", $scope) : modelService.saveModel("ArticleNew", $scope);
        }
    };

    $scope.approveArticle = function() {
        return modelService.saveModel("ArticleApprove", $scope);
    };
    
    $scope.rejectArticle = function () {
        return modelService.saveModel("ArticleReject", $scope);
    };

    $scope.modelUpdateSuccessful = function() {
        $state.go("articles.list");
    };

    $scope.getArticleTemplates = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.articleTemplates = response.data;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getArticleTemplates, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getArticleTemplates().then(successCallback, errorCallback);
    };

    $scope.getDepartment = function (departmentId) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.department = response.data;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getDepartment, successCallback, errorCallback, departmentId);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getDepartment(departmentId).then(successCallback, errorCallback);
    };

    $scope.getDepartments = function() {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        $scope.subSectionTabs = [];
        
        var successCallback = function(response) {
            cfpLoadingBar.complete();
            $scope.departments = response.data;

            for (var i = 0; i < $scope.departments.length; i++) {
                $scope.departments[i].Container = $filter("filter")($scope.departmentContainers, { Id: $scope.departments[i].ContainerId }, true)[0];
            }

            $scope.subSectionTabs = angular.copy($scope.globalSubSectionTabs);
            console.log("getDepartments - Sub section tabs");
            console.log($scope.subSectionTabs);

            console.log($scope.departments);
            $scope.loadComplete = true;
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getDepartments, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getDepartments().then(successCallback, errorCallback);
    };

    $scope.getEditableDepartments = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        $scope.subSectionTabs = [];

        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.departments = response.data;

            for (var i = 0; i < $scope.departments.length; i++) {
                $scope.departments[i].Container = $filter("filter")($scope.departmentContainers, { Id: $scope.departments[i].ContainerId }, true)[0];
            }

            $scope.subSectionTabs = angular.copy($scope.globalSubSectionTabs);
            console.log("getEditableDepartments - Sub section tabs");
            console.log($scope.subSectionTabs);

            console.log($scope.departments);
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getEditableDepartments, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getEditableDepartments().then(successCallback, errorCallback);
    };

    $scope.getContainerDepartments = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();

        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.departments = response.data;

            for (var i = 0; i < $scope.departments.length; i++) {
                $scope.departments[i].Container = $filter("filter")($scope.departmentContainers, { Id: $scope.departments[i].ContainerId }, true)[0];
            }

            console.log("$scope.subSectionTabs *************************************");
            console.log($scope.subSectionTabs);

            if (!$scope.subSectionTabs.length) {
                $scope.subSectionTabs = angular.copy($scope.globalSubSectionTabs); 
            }

            console.log($scope.departments);
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getContainerDepartments, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getContainerDepartments($scope.pagination.container).then(successCallback, errorCallback);
    };


    $scope.initialiseArticlesForm = function() {
        var articleId = $location.search().articleId;
        var revisionId = $location.search().revisionId;

        $scope.article = {};
        $scope.uneditedArticle = {};
        $scope.articleDepartmentOptions = [];
        if (articleId) {
            if (!$scope.article.Terms) $scope.article.Terms = [];
            if (revisionId) {
                $scope.getArticleRevision(articleId, revisionId);
            } else {
                $scope.getArticle(articleId);
            }
        } else {
            if (!$scope.article) $scope.article = {};
            if (!$scope.article.Structure) $scope.article.Structure = [];
            if (!$scope.article.Terms) $scope.article.Terms = [];
            $scope.getEditableDepartments();
            $scope.getArticleTemplates();
            
            console.log("sub section tabs");
            console.log($scope.subSectionTabs);
        }
    };

    $scope.loadDepartmentSubSectionTabs = function (articleDepartmentOptions) {
        $scope.articleDepartmentOptions = articleDepartmentOptions;

        for (var i = 0; i < $scope.articleDepartmentOptions.length; i++) {
            $scope.getDepartmentSubSectionTabs($scope.articleDepartmentOptions[i]);
        }

        if (articleDepartmentOptions && articleDepartmentOptions.length > 0) {
            $scope.getDepartment($filter("filter")($scope.departments, { Id: articleDepartmentOptions[0] }, true)[0].Id);
        } else {
            $scope.getDepartment($scope.departments[0].Id);
        }
    };
    
    $scope.getDepartmentSubSectionTabs = function (departmentId) {
        $scope.loadComplete = false;
        $rootScope.errorRetry = false;
        $scope.returnedSubSections = [];
        var successCallback = function(response) {
            $scope.loadComplete = true;
            //$scope.subSectionTabs = response.data;
            if (response.data && response.data.length > 0) {
                for (var i = 0; i < response.data.length; i++) {
                    var exists = $filter("filter")($scope.subSectionTabs, { Id: response.data[i].Id }, true)[0];
                    if (!exists) {
                        $scope.subSectionTabs.splice($scope.subSectionTabs.length, 0, response.data[i]);
                    }
                }
            }
        };
        var errorCallback = function(error) {
            cfpLoadingBar.complete();
        };
        httpService.getDepartmentSubSectionTabs(departmentId).then(successCallback, errorCallback);
    };

    $scope.archiveArticle = function(id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to archive this?")) {
            cfpLoadingBar.start();
            var successCallback = function() {
                $scope.paginateArticles();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.archiveArticle.bind(httpService.archiveArticle, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.archiveArticle(id).then(successCallback, errorCallback);
        }
    };

    $scope.unArchiveArticle = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to unarchive this?")) {
            cfpLoadingBar.start();
            var successCallback = function () {
                $scope.paginateArticles();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.unArchiveArticle.bind(httpService.unArchiveArticle, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.unArchiveArticle(id).then(successCallback, errorCallback);
        }
    };

    var padLeft = function (str, len) {
        var i = -1;
        while (++i < len) {
            str = " " + str;
        }
        console.log('padLeft: str: ', str);
        console.log('padLeft: len: ', len);
        return str;
    };

    var addExtraMarkdownStyling = function (block, markdownText) {
        if (block.bold) markdownText = "**" + markdownText + "**";
        if (block.italic) markdownText = "_" + markdownText + "_";
        return markdownText;
    };

    $scope.previousBlockLevel = 0;

    var transformBlock = function (block, level, previousBlock) {

        var preBlock = previousBlock;
        var initialEol = true;
        var paddingIncrement = 4;
        if (previousBlock && previousBlock.type === "text") initialEol = false;
        if (previousBlock && previousBlock.type === "unordered" && block.type !== "unordered") paddingIncrement = 3;

        var markdownText = block.input;
        var eol = "\n";
        var padding = level * paddingIncrement;

        switch (block.type) {
            case "text":
                var markDownTextArray = markdownText.split(eol);
                markdownText = "";
                for (var i = 0; i < markDownTextArray.length; i++) {
                    if (markDownTextArray[i] !== "") {
                        markdownText = markdownText + addExtraMarkdownStyling(block, markDownTextArray[i]) + eol + eol; 
                    }
                }
                if (level > 0) markdownText = padLeft(markdownText, padding);
                if (initialEol) markdownText = eol + markdownText;
                break;
            case "unordered":
                markdownText = addExtraMarkdownStyling(block, markdownText);
                markdownText = "* " + markdownText;
                markdownText = markdownText + eol;
                break;
            case "ordered":
                markdownText = addExtraMarkdownStyling(block, markdownText);
                markdownText = "1. " + markdownText;
                markdownText = markdownText + eol;
                if (level > 0) markdownText = padLeft(markdownText, padding);
                break;
            case "link":
                markdownText = "[" + markdownText + "](" + block.url + ")";
                markdownText = addExtraMarkdownStyling(block, markdownText);
                if (level > 0) markdownText = padLeft(markdownText, padding);
                if (initialEol) markdownText = eol + markdownText + eol + eol;
                break;
            case "image":
                markdownText = "@Image-URL=" + block.filename;
                if (initialEol) markdownText = eol + markdownText;
                markdownText = markdownText + eol + eol;
                break;
            case "table":
                var tableStructure = block.structure;
                markdownText = eol;
                for (var j = 0; j < tableStructure.length; j++) {
                    var tableRow = "|";
                    for (var k = 0; k < tableStructure[j].length; k++) {
                        console.log(tableStructure[j][k]);
                        var cellValue = (tableStructure[j][k].value) ? tableStructure[j][k].value : "";
                        tableRow = tableRow + cellValue + "|";
                    }
                    tableRow = tableRow + eol;

                    //Headings need extra formating
                    if (j === 0) {
                        tableRow = tableRow + "|";
                        for (var l = 0; l < tableStructure[j].length; l++) {
                            tableRow = tableRow + "---|";
                        }
                        tableRow = tableRow + eol;
                    }

                    markdownText = markdownText + tableRow;
                }
                console.log(markdownText);
                break;
        }
        return markdownText;
    };

    var transformStructureToBody = function (structure, body, level) {

        for (var i = 0; i < structure.length; i++) {
            var previousBlock = (structure[i - 1]) ? structure[i - 1] : null;
            body += transformBlock(structure[i], level, previousBlock);
            if (structure[i].blocks.length > 0) {
                var nextLevel = level + 1;
                body = transformStructureToBody(structure[i].blocks, body, nextLevel);
            }
        }
        return body;
    };

    var startTransform = function(structure) {
        $scope.article.Body = "";
        var body = $scope.article.Body;

        $scope.article.Body = transformStructureToBody(structure, body, 0);

        $scope.processedBody = null;
        processBody();
    };

    $scope.setArticleTemplate = function () {
        var selectedTemplate = $filter("filter")($scope.articleTemplates, { Id: $scope.article.Template }, true)[0];
        $scope.article.Structure = selectedTemplate.Structure;
        startTransform($scope.article.Structure);
    };

    $scope.addBlockToSection = function (type, blockKey, block) {
        var thisBlock = block;

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
                images: function () {
                    return $scope.article.images;
                }
            }
        });

        modalInstance.result.then(function (result) {

            var block = {};

            if (result.value) {
                if (result.url) {
                    block = {
                        type: type,
                        bold: false,
                        italic: false,
                        input: result.value,
                        url: result.url,
                        image: null,
                        revisionEditedId: -1,
                        blocks: []
                    };
                    console.log('addBlockToSection: modal: ', block);
                } else {
                    block = {
                        type: type,
                        bold: false,
                        italic: false,
                        input: result.value,
                        image: null,
                        revisionEditedId: -1,
                        blocks: []
                    };
                    console.log('addBlockToSection: modal: ', block);
                }

                $scope.article.Structure.splice(blockKey+1, 0, block);
            } else if (result.table) {
                block = {
                    type: type,
                    image: null,
                    structure: result.table,
                    revisionEditedId: -1,
                    blocks: []
                };
                console.log('addBlockToSection: modal: ', block);

                $scope.article.Structure.splice(blockKey + 1, 0, block);

            } else if (result.image) {
                var newBlock = false;
                if (thisBlock) {
                    thisBlock.input = null;
                    thisBlock.structure = null;
                    thisBlock.filename = result.image;
                    thisBlock.revisionEditedId = -1;
                    console.log('addBlockToSection: modal: ', thisBlock);
                } else {
                    newBlock = true;
                    thisBlock = {
                        type: 'image',
                        filename: result.image,
                        revisionEditedId: -1,
                        blocks: []
                    };
                    console.log('addBlockToSection: modal: ', thisBlock);
                }
                console.log('thisBlock.filename: ' + thisBlock.filename);
                var splitFilename = thisBlock.filename.split(".");

                var found = false;

                console.log('$scope.article.images: ' + $scope.article.images);
                for (var key in $scope.article.images) {
                    if ($scope.article.images[key].Id === splitFilename[0] &&
                        $scope.article.images[key].Extension === "." + splitFilename[1]) {

                        thisBlock.image = $scope.article.images[key].image;
                        found = true;
                    }
                }

                if (!found) {
                    fileService.downloadImage(thisBlock.filename, 250).then(
                        function(response) {
                            thisBlock.image = response.data;
                            if (newBlock) $scope.article.Structure.splice(blockKey + 1, 0, thisBlock);
                            startTransform($scope.article.Structure);
                        }
                    );
                } else {
                    if (newBlock) $scope.article.Structure.splice(blockKey + 1, 0, thisBlock);
                    startTransform($scope.article.Structure);
                }
            }
            startTransform($scope.article.Structure);
            console.log('$scope.article.Structure: ', $scope.article.Structure);
        });
    };

    $scope.addEditBlock = function (type, block) {
        var thisBlock = block;

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/articlesModalPopups.html",
            controller: "articleModalsCtrl",
            resolve: {
                type: function() {
                    return type;
                },
                block: function() {
                    return block;
                },
                images: function() {
                    return $scope.article.images;
                }
            }
        });

        modalInstance.result.then(function (result) {
            var block = {};

            console.log('addEditBlock: result: ', result.value);
            console.log('addEditBlock: block: ', block);
            if (result.value) {
                if (result.url) {
                    block = {
                        type: type,
                        bold: false,
                        italic: false,
                        input: result.value,
                        filename: null,
                        images: null,
                        url: result.url,
                        revisionEditedId: -1,
                        blocks: []
                    };
                } else {
                    block = {
                        type: type,
                        bold: false,
                        italic: false,
                        filename: null,
                        images: null,
                        input: result.value,
                        revisionEditedId: -1,
                        blocks: []
                    };
                }

                $scope.article.Structure.push(block);
                startTransform($scope.article.Structure);
            } else if (result.table) {
                block = {
                    type: type,
                    filename: null,
                    images: null,
                    structure: result.table,
                    revisionEditedId: -1,
                    blocks: []
                };

                $scope.article.Structure.push(block);
                startTransform($scope.article.Structure);
            } else if (result.image) {
                var newBlock = false;
                if (thisBlock) {
                    thisBlock.input = null;
                    thisBlock.structure = null;
                    thisBlock.filename = result.image;
                    thisBlock.revisionEditedId = -1;
                } else {
                    newBlock = true;
                    thisBlock = {
                        type: 'image',
                        filename: result.image,
                        revisionEditedId: -1,
                        blocks: []
                    };
                }

                var splitFilename = thisBlock.filename.split(".");

                var found = false;
                for (var key in $scope.article.images) {
                    if ($scope.article.images[key].Id === splitFilename[0] &&
                        $scope.article.images[key].Extension === "." + splitFilename[1]) {

                        thisBlock.image = $scope.article.images[key].image;
                        found = true;
                    }
                }
                if (!found) {
                    fileService.downloadImage(thisBlock.filename, 250).then(
                        function(response) {
                            thisBlock.image = response.data;
                            if (newBlock) $scope.article.Structure.push(thisBlock);
                            startTransform($scope.article.Structure);
                        }
                    );
                } else {
                    if (newBlock) $scope.article.Structure.push(thisBlock);
                    startTransform($scope.article.Structure);
                }
            } else {
                startTransform($scope.article.Structure);
            }
        });
    };

    $scope.removeBlock = function (scope) {
        scope.remove();
        startTransform($scope.article.Structure);
    };

    $scope.bold = function (block) {
        block.bold = (block.bold) ? false : true;
        startTransform($scope.article.Structure);
    };

    $scope.italic = function (block) {
        block.italic = (block.italic) ? false : true;
        startTransform($scope.article.Structure);
    };

    $scope.getIcon = function(type) {
        switch (type) {
            case "text":
                return "fa fa-font";
            case "unordered":
                return "fa fa-list-ul";
            case "ordered":
                return "fa fa-list-ol";
            case "link":
                return "fa fa-link";
            case "table":
                return "fa fa-table";
            case "image":
                return "fa fa-picture-o";
        }
        return null;
    };

    $scope.treeOptions = {
        dropped: function (e) {
            startTransform($scope.article.Structure);
        }
    };

    $scope.newTerm = function (term) {
        return {
            Name: term
        };
    };

    /* Upload stuff */
    $scope.$watch("article.files", function () {
        if ($scope.article && $scope.article.files)
            $scope.upload($scope.article.files);
    });

    $scope.upload = function (files) {
        $scope.article.uploadError = null;
        if (!$scope.article.images) $scope.article.images = {};

        if (files && files.length) {
            var livePreviewWidth = document.getElementById("mobilePreviewArea").offsetWidth;

            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                if (!file.$error) {
                    fileService.uploadImage(file, $scope.article, livePreviewWidth);
                }
            }
        }
    };

    $scope.setArticleExpiryDateByMonthsAhead = function(monthsAhead) {
        console.log('monthsAhead: ', monthsAhead);

        var momentDate = moment();
        momentDate.add(monthsAhead, 'months').format('Do MMMM YYYY');

        $scope.monthFromTodayDate = momentDate.toDate();
        $scope.newProposedDate = momentDate.toDate();
    };

    $scope.clearRadioButtons = function () {
        console.log('clearRadioButtons: ');

        $scope.data = {
            group1: 'datesAhead'
        };
    };

    $scope.setExpiryDateFromCalendar = function () {
        console.log('$scope.article.ExpiryDateTime: ', $scope.article.ExpiryDateTime);
        console.log('$scope.calendarSelectedDate: ', $scope.calendarSelectedDate);

        $scope.newProposedDate = $scope.calendarSelectedDate.date;

    }

    $scope.toggleCalendar = function() {
        $scope.viewCalendar = !$scope.viewCalendar;
        console.log('$scope.viewCalendar: ', $scope.viewCalendar);
    }

     $scope.toggleExpiry = function() {
         $scope.expiryStatusSet = !$scope.expiryStatusSet;
         console.log('expiryStatus: ', $scope.expiryStatusSet);

         if ($scope.expiryStatusSet) {
             $scope.article.ExpiryDateTime = new Date();
             $scope.calendarSelectedDate = new Date();
             $scope.newProposedDate = new Date();
         }
     }

    $scope.articleExpired = function (articleExpiryDate) {

        var todayDate = new Date();

        if (moment(articleExpiryDate) < moment(todayDate)) {

            return true;
        } else {

            return false;
        };
    }

    $scope.articleExpiring = function (articleExpiryDate) {

        var todayDate = new Date();

        var dateTwoMonthsBeforeExpiry = moment(articleExpiryDate);

        dateTwoMonthsBeforeExpiry = dateTwoMonthsBeforeExpiry.subtract(2, 'months');

        if (moment(todayDate) > dateTwoMonthsBeforeExpiry && moment(todayDate) < moment(articleExpiryDate)) {

            return true;

        } else {

            return false;
        };
    }

    $scope.applyExpiredClassToArticleExpiryDate = function () {

        var todayDate = new Date();

        var dateTwoMonthsBeforeExpiry = moment($scope.article.ExpiryDateTime);

        dateTwoMonthsBeforeExpiry = dateTwoMonthsBeforeExpiry.subtract(2, 'months');

        if (moment($scope.article.ExpiryDateTime) < moment(todayDate)) {

            return "expired";
        } else if (moment(todayDate) > dateTwoMonthsBeforeExpiry && moment(todayDate) < moment($scope.article.ExpiryDateTime)) {

            return "expirying";
        };
    }

    $scope.applyExpiredClassToProposedExpiryDate = function () {

        var todayDate = new Date();

        var dateTwoMonthsBeforeExpiry = moment($scope.newProposedDate);

        dateTwoMonthsBeforeExpiry = dateTwoMonthsBeforeExpiry.subtract(2, 'months');

        if (moment($scope.newProposedDate) < moment(todayDate)) {

            return "expired";
        } else if (moment(todayDate) > dateTwoMonthsBeforeExpiry && moment(todayDate) < moment($scope.newProposedDate)) {

            return "expirying";
        };
    }

    $scope.convertCsharpDateToJavacript = function(cSharpDate) {
        console.log('c# date before: ', cSharpDate);
        var date = new Date(Date.parse(cSharpDate));
        console.log('c# date after: ', date);
        return date;
    };

    $scope.blockEdited = function (block) {
        if (block.edited) {
            return "block-edited";
        }
    }

    $scope.blockEditedOnThisRevision = function (block) {
        if (block.revisionEditedId) {
            if (block.revisionEditedId == $scope.article.RevisionId) {
                return "block-edited";
            }
        }
    }
}]);