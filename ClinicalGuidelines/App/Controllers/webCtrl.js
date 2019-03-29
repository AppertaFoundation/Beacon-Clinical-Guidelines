//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿downloadFullImage2 = function (scope) {
    console.log(scope);
    console.log(scope.alt);

    var s = angular.element(document.getElementById('webCtrl')).scope();

    if (s === undefined) {

        s = angular.element(document.getElementById('articlesCtrl')).scope();
    }
    console.log(s);
    var i = scope.alt;
    s.downloadFullImageByFileName(i);

};

app.controller("webCtrl", ["$scope", "$rootScope", "$filter", "$state", "$location", "$sce", "$window", "cfpLoadingBar", "httpService", "fileService", "$q", "$uibModal", function ($scope, $rootScope, $filter, $state, $location, $sce, $window, cfpLoadingBar, httpService, fileService, $q, $uibModal) {
    $scope.loadComplete = false;
    $scope.loadArticlesComplete = false;
    $scope.apiErrors = null;
    $scope.debug = $rootScope.debug;

    $scope.departments = [];
    $scope.departmentContainers = [];
    $scope.department = null;
    $scope.container = null;
    $scope.articles = [];
    $scope.article = null;

    $scope.departmentId = null;
    $scope.containerId = null;
    $scope.articleId = null;
    $scope.subSectionTabId = null;
    $scope.articleSearchTerm = null;
    $scope.departmentSearchTerm = null;
    $scope.departmentContacts = [];

    $scope.emailMessage = null;
    $scope.emailSubject = null;
    $scope.emailLinkUrl = null;

    $scope.pagination = { pageSize: 15, currentPage: 1, totalItems: 10, maxSize: 5, searchTerm: null };

    $scope.setPage = function (pageNo) {
        console.log("select page called:" + pageNo);
        $scope.pagination.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log("Page changed to: " + $scope.pagination.currentPage);
        $scope.paginateArticles();
    };

    $scope.getArticleBackgroundColour = function (departmentId) {
        if ($scope.departmentId) {
            var department = $filter('filter')($scope.departments, { Id: +$scope.departmentId }, true)[0];
            return (department) ? department.BackgroundColour : '';
        }
        var department = $filter('filter')($scope.departments, { Id: +departmentId }, true)[0];
        return (department) ? department.BackgroundColour : '';
    };

    $scope.getArticleAreaDetails = function (article) {
        if (!article || !article.Departments || !article.Departments[0]) return null;

        var container = null;
        var department = null;

        var articleDepartment = article.Departments[0];

        if (!$scope.containerId) {
            container = (articleDepartment.ContainerId)
                ? $filter('filter')($scope.departmentContainers, { Id: +articleDepartment.ContainerId }, true)[0]
                : null;
        }

        department = (articleDepartment.Id)
            ? $filter('filter')($scope.departments, { Id: +articleDepartment.Id }, true)[0]
            : null;

        if (container) {
            return 'within ' + container.Name + '/' + department.Name;
        } else {
            return 'within ' + department.Name;
        }
    };

    $scope.initialiseWeb = function () {
        $scope.processedBody = null;
        $scope.containerId = ($location.search().containerId) ? $location.search().containerId : null;

        $scope.departmentId = ($location.search().departmentId) ? $location.search().departmentId : null;
        $scope.articleId = ($location.search().articleId) ? $location.search().articleId : null;
        $scope.subSectionTabId = ($location.search().subSectionTabId) ? $location.search().subSectionTabId : null;        

        $scope.getDepartments();
        
        $scope.articles = null;
        $scope.pagination = { pageSize: 15, currentPage: 1, totalItems: 10, maxSize: 5, searchTerm: null };
        $scope.pagination.searchTerm = ($location.search().searchText) ? $location.search().searchText : null;
        $scope.pagination.container = $scope.containerId;

        $scope.paginateArticles();

        $scope.subSectionTabs = null;
        if ($scope.departmentId) {      
            $scope.getDepartmentSubSectionTabs();
        } else {
            $scope.getSubSectionTabs();
        }
    };

    $scope.getDepartments = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.departments = $filter('filter')(response.data, { Container: false }, true);
            $scope.departmentContainers = $filter('filter')(response.data, { Container: true }, true);

            $scope.department = null;
            $scope.container = null;

            if ($scope.departmentId) {
                $scope.department = $filter('filter')(response.data, { Id: +$scope.departmentId }, true)[0];
            }

            if ($scope.containerId) {
                $scope.container = $filter('filter')(response.data, { Id: +$scope.containerId }, true)[0];
            }

            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getAllDepartments, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = 'Failed: ' + error.data.Message;
                } else {
                    $scope.apiErrors = 'Connection lost, please try again.';
                }
            }
        };
        httpService.getAllDepartments().then(successCallback, errorCallback);
    };

    $scope.getDepartmentSubSectionTabs = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.subSectionTabs = response.data;

            $scope.subSectionTab = null;
            if ($scope.departmentId) {
                $scope.subSectionTab = $filter('filter')(response.data, { Id: +$scope.subSectionTabId }, true)[0];
            }

            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getDepartmentSubSectionTabs.bind($scope.departmentId), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = 'Failed: ' + error.data.Message;
                } else {
                    $scope.apiErrors = 'Connection lost, please try again.';
                }
            }
        };
        httpService.getDepartmentSubSectionTabs($scope.departmentId).then(successCallback, errorCallback);
    };

    $scope.getSubSectionTabs = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.subSectionTabs = response.data;
            $scope.subSectionTab = null;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getSubSectionTabs.bind(), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = 'Failed: ' + error.data.Message;
                } else {
                    $scope.apiErrors = 'Connection lost, please try again.';
                }
            }
        };
        httpService.getSubSectionTabs().then(successCallback, errorCallback);
    };

    $scope.paginateArticles = function () {
        if (!$scope.pagination.container && !$scope.departmentId && !$scope.subSectionTabId && !$scope.pagination.searchTerm) {
            return;
        }
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadArticlesComplete = false;
        $scope.articles = null;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.articles = response.data.Articles;
            $scope.pagination.totalItems = response.data.TotalCount;
            $scope.loadArticlesComplete = true;

            $scope.article = null;
            if ($scope.articleId) {
                $scope.getArticle($scope.articleId);
            }
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getPaginatedLiveArticles.bind(httpService.getPaginatedArticles, $scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.container, $scope.departmentId, $scope.subSectionTabId, $scope.pagination.searchTerm), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = 'Failed: ' + error.data.Message;
                } else {
                    $scope.apiErrors = 'Connection lost, please try again.';
                }
            }
        };
        httpService.getPaginatedLiveArticles($scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.container, $scope.departmentId, $scope.subSectionTabId, $scope.pagination.searchTerm).then(successCallback, errorCallback);
    };

    $scope.goTo = function (containerId, departmentId, subSectionTabId, articleId) {
        console.log('Container Id: ' + containerId);
        $state.go('web.list', { 'containerId': containerId, 'departmentId': departmentId, 'subSectionTabId': subSectionTabId, 'articleId': articleId, 'searchText': null, 'searchType': null });
    };

    $scope.getSubSectionTabIcon = function (subSectionTabId) {
        var filteredSubSectionTabs = $filter('filter')($scope.subSectionTabs, { Id: +subSectionTabId }, true);
        if (!filteredSubSectionTabs || !filteredSubSectionTabs[0] || !filteredSubSectionTabs[0].Icon) return null;
        return filteredSubSectionTabs[0].Icon;
    };

    $scope.shareViaEmail = function () {

        var message = 'Hi\r\r' +
            'I would like to share the clinical guidance for \'' +
            $scope.article.Title +
            '\' with you.\r\r' +
            'Click on the following link to view the article http://beacon.plymouth.nhs.uk/#/?articleId=' + $scope.article.Id + '\r\r';

        var link = "mailto:" +
            "&subject=" +
            escape('Sharing the clinical guidance for \'' + $scope.article.Title + '\'') +
            "&body=" +
            escape(message);

        window.location.href = link;
    }

    $scope.displayDepartmentContactsFromContainer = function () {
        $scope.departmentContacts = [];
        console.log('Getting contacts from container');
        console.log($scope.container);
        console.log('Container Id: ');
        console.log($scope.container.Id);
        console.log('Departments: ');
        console.log($scope.departments);

        cfpLoadingBar.start();

        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.departmentContacts = response.data.StaticPhoneNumbers;
            console.log(response.data);

            if ($scope.departmentContacts.length > 0) {
                $scope.viewStaticPhoneNumber($scope.departmentContacts,
                    $scope.departments[0].BackgroundColour,
                    $scope.departments[0].MainColour); 
            }
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getStaticPhoneNumberByContainerId($scope.container.Id), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = 'Failed: ' + error.data.Message;
                } else {
                    $scope.apiErrors = 'Connection lost, please try again.';
                }
            }
        };
        httpService.getStaticPhoneNumberByContainerId($scope.container.Id).then(successCallback, errorCallback);
    }

    $scope.displayDepartmentContactsFromDepartment = function () {
        $scope.departmentContacts = [];
        console.log('Getting contacts from department');
        console.log($scope.department);
        console.log('Department Id: ');
        console.log($scope.department.Id);

        cfpLoadingBar.start();

        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.departmentContacts = response.data.StaticPhoneNumbers;
            console.log(response.data);

            if ($scope.departmentContacts.length > 0) {
                $scope.viewStaticPhoneNumber($scope.departmentContacts,
                    $scope.departments[0].BackgroundColour,
                    $scope.departments[0].MainColour);
            };
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getStaticPhoneNumberByDepartmentId($scope.department.Id), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = 'Failed: ' + error.data.Message;
                } else {
                    $scope.apiErrors = 'Connection lost, please try again.';
                }
            }
        };
        httpService.getStaticPhoneNumberByDepartmentId($scope.department.Id).then(successCallback, errorCallback);
    }

    $scope.showDisclaimer = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/disclaimerModal.html",
            controller: "disclaimerModalCtrl",
            size: "lg",
            resolve: {
                departmentBackgroundColour: function () {
                    return $scope.department.BackgroundColour;
                },
                departmentMainColour: function () {
                    return $scope.department.MainColour;
                }
            }
        });
    };

    $scope.viewStaticPhoneNumber = function (contacts, departmentBackgroundColour, departmentMainColour) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/viewOnlyStaticPhoneNumberModal.html",
            controller: "viewOnlyStaticPhoneNumbersModalCtrl",
            size: "lg",
            resolve: {
                contacts: function () {
                    return contacts;
                },               
                departmentBackgroundColour: function () {
                    return departmentBackgroundColour;
                },           
                departmentMainColour: function () {
                    return departmentMainColour;
                }
            }
        });
    };

    $scope.viewEmailDetailsForCopying = function () {
        $scope.emailMessage = 'Hi \r\r' +
            'I would like to share the clinical guidance for \'' +
            $scope.article.Title +
            '\' with you.\r\r' +
            'Click on the article link below to view the article\r\r';

        $scope.emailSubject = 'Sharing the clinical guidance for \'' + $scope.article.Title + '\'';
        $scope.emailLinkUrl = "http://beacon.plymouth.nhs.uk/#/?articleId=" + $scope.article.Id;

        var emailModalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/emailCopyDetailsModal.html",
            controller: "emailCopyDetailsModalCtrl",
            size: "lg",
            resolve: {
                emailTitle: function () {
                    return $scope.emailSubject;
                },               
                emailBody: function () {
                    return $scope.emailMessage;
                },
                emailLink: function () {
                    return $scope.emailLinkUrl;
                },               
                departmentBackgroundColour: function () {
                    return $scope.department.BackgroundColour;
                },           
                departmentMainColour: function () {
                    return $scope.department.MainColour;
                }
            }
        });

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

                // Starting to download the images is the labour intensive part so added toggle to not redo this if already begun
                $scope.processingBodyStarted = true;
                for (var j = 0; j < foundImageIndexes.length; j++) {
                    var thisIndex = foundImageIndexes[j];
                    var splitImage = splitArticle[thisIndex].split("=").pop();

                    var promise = (function () {
                        var key = thisIndex;
                        var thisImage = splitImage;
                        return fileService
                            .downloadImage(thisImage, 250)
                            .then(
                                function (response) {
                                    // Response to http call processed then passed back wrapped in a $q.when so that the $q.all can resolve all promises
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

    $scope.bindArticleBody = function (articleBody) {
        if (!articleBody) return null;

        articleBody = processBody(articleBody);

        return $sce.trustAsHtml(marked(articleBody));
    };


    $scope.downloadFullImage = function (image) {
        $scope.fullImage = null;
        fileService.downloadFullImage(image.Id + image.Extension).then(
            function (response) {
                $scope.fullImage = response.data;
                $state.go("web.image");
            }
        );
    };

    $scope.downloadFullImageByFileName = function (fileName) {
        $scope.fullImage = null;
        fileService.downloadFullImage(fileName).then(
            function (response) {
                $scope.fullImage = response.data;
                $state.go("web.image");
            }
        );
    };

    $scope.historyBack = function () {
        $window.history.back();
    };

    $scope.getArticle = function (articleId) {     
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();

        var downloadAndUpdateImage = function (index, article, fileName) {
            setTimeout(function () {
                fileService.downloadImage(article.Files[index].Id + article.Files[index].Extension, 400).then(
                    function (response) {
                        $scope.article.images[fileName].image = response.data;
                    }
                );
            }, 500);
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

        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.article = response.data;

            var articleDepartment = ($scope.article.Departments && $scope.article.Departments[0])
                ? $scope.article.Departments[0]
                : null;

            if (!$scope.containerId) {
                $scope.container = (articleDepartment.ContainerId)
                    ? $filter('filter')($scope.departmentContainers, { Id: +articleDepartment.ContainerId }, true)[0]
                    : null;
            } 

            if (!$scope.departmentId) {
                $scope.department = (articleDepartment.Id)
                    ? $filter('filter')($scope.departments, { Id: +articleDepartment.Id }, true)[0]
                    : null;
            } 

            if (!$scope.subSectionTabId) {
                $scope.subSectionTab = ($scope.article.SubSectionTabId)
                    ? $filter('filter')($scope.subSectionTabs, { Id: +$scope.article.SubSectionTabId }, true)[0]
                    : null;
            } 
            
            createImagesArrayFromFilesDto($scope.article);

            $scope.loadComplete = true;
            processBody();
        };

        var errorCallback = function (error) {
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
}]);