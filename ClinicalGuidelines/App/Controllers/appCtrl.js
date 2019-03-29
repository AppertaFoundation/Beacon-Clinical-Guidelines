//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("appCtrl", ["$scope", "$rootScope", "$location", "$state", "$uibModal", "cfpLoadingBar", "httpService", function ($scope, $rootScope, $location, $state, $uibModal, cfpLoadingBar, httpService) {
    $scope.loadComplete = false;
    $scope.apiErrors = null;
    $scope.debug = $rootScope.debug;

    $scope.headerType = null;

    $scope.webViewSearching = {
        Text: ''
    };

    $scope.previousLocationSearchText = null;
 
    //#region Layout functions - called on all pages
    $scope.user = $rootScope.user;

    var userSuffix = " users per page";
    $scope.pageSizeOptions = [{ Id: 5, Name: 5 + userSuffix }, { Id: 10, Name: 10 + userSuffix }, { Id: 25, Name: 25 + userSuffix }, { Id: 50, Name: 50 + userSuffix }, { Id: 100, Name: 100 + userSuffix }];
    $scope.pagination = { pageSize: 10, currentPage: 1, totalItems: 10, maxSize: 5, searchTerm: null }

    $scope.setPage = function (pageNo) {
        console.log("select page called:" + pageNo);
        $scope.pagination.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log("Page changed to: " + $scope.pagination.currentPage);
        $scope.paginateUsers();
    };

    $scope.pageStaticPhoneNumberChanged = function () {
        console.log("Page changed to: " + $scope.pagination.currentPage);
        $scope.paginateStaticPhoneNumbers();
    };

    $scope.checkUserPermissions = function () {
        if (!$scope.user) return false;
        return $scope.user.SiteAdmin || $scope.user.LeadClinician;
    };

    $scope.isWebView = function () {
        if ($location.path() === '/') {
            $scope.initialiseWebView();
            return true;
        } else {
            return false;
        }
    }

    $scope.initialiseWebView = function () {
        if ($location.search().searchText && $scope.previousLocationSearchText !== $location.search().searchText) {
            $scope.webViewSearching.Text = ($location.search().searchText) ? $location.search().searchText : null;
            $scope.previousLocationSearchText = $scope.webViewSearching.Text;
        }        
    }

    $scope.webViewSearch = function () {
        if (!$scope.isWebView()) return;

        var containerId = ($location.search().containerId) ? $location.search().containerId : null;
        var departmentId = ($location.search().departmentId) ? $location.search().departmentId : null;
        var articleId = ($location.search().articleId) ? $location.search().articleId : null;
        var subSectionTabId = ($location.search().subSectionTabId) ? $location.search().subSectionTabId : null;

        if ($scope.webViewSearching.Text) {
            $state.go('web.list', { 'containerId': null, 'departmentId': null, 'subSectionTabId': null, 'articleId': null, 'searchText': $scope.webViewSearching.Text });        
        }
    }

    $scope.paginateUsers = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            console.log(response);
            $scope.users = response.data.Users;
            $scope.pagination.totalItems = response.data.TotalCount;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getPaginatedUsers.bind(httpService.getPaginatedUsers, $scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getPaginatedUsers($scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm).then(successCallback, errorCallback);
    };

    $scope.loadCurrentUser = function () {
        if ($scope.user) return;
        $rootScope.errorRetry = false;
        cfpLoadingBar.start();
        var successCallback = function(response) {
            $scope.user = response.data;
            $rootScope.user = $scope.user;
            $rootScope.$broadcast("user:updated", response.data);
            $scope.setCurrentServer($scope.user.CurrentServer);
            cfpLoadingBar.complete();
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.loadCurrentUser, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.loadCurrentUser().then(successCallback, errorCallback);
    };

    $scope.setCurrentServer = function (currentServer) {
        switch (currentServer) {
            case 0:
                $scope.headerType = "DEBUG";
                break;
            case 1:
                $scope.headerType = "TEST";
                break;
            case 2:
                $scope.headerType = "UAT";
                break;
            case 3:
                $scope.headerType = "LIVE";
                break;
            default:
                $scope.headerType = "UNKNOWN";
                break;
        };
    };

    $scope.loadCurrentUser();

    $scope.paginateStaticPhoneNumbers = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            console.log(response);
            $scope.staticPhoneNumbers = response.data.StaticPhoneNumbers;
            $scope.pagination.totalItems = response.data.TotalCount;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getPaginatedStaticPhoneNumbers.bind(httpService.getPaginatedStaticPhoneNumbers, $scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getPaginatedStaticPhoneNumbers($scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm).then(successCallback, errorCallback);
    };
    //#endregion

    //#region Helper function
    var getModalVariables = function (scope, type, value) {
        var user, department, template, templateItem, article, staticPhoneNumber = null;
        switch (type) {
            case "user":
                user = value;
                break;
            case "department":
                department = value;
                break;
            case "template":
                template = value;
                break;
            case "templateItem":
                templateItem = value;
                break;
            case "article":
                article = value;
                break;
            case "staticPhoneNumber":
                staticPhoneNumber = value;
                break;
        }

        return {
            scope: function() {
                return scope;
            },
            user: function() {
                return user;
            },
            department: function() {
                return department;
            },
            siteAdmin: function() {
                return $scope.user.SiteAdmin;
            },
            leadClinician: function() {
                return $scope.user.LeadClinician;
            },
            template: function () {
                return template;
            },
            templateItem: function () {
                return templateItem;
            },
            article: function () {
                return article;
            },
            staticPhoneNumber: function() {
                return staticPhoneNumber;
            }
        }
    };
    //#endregion

    //#region Users Admin
    $scope.users = [];

    $scope.addUser = function (scope) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/usersModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "user", null)
        });

        modalInstance.result.then(function () {
            $scope.paginateUsers();
        });
    };

    $scope.editUser = function (scope, user) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/usersModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "user", user)
        });

        modalInstance.result.then(function () {
            $scope.paginateUsers();
        });
    };

    $scope.deleteUser = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to delete this?")) {
            cfpLoadingBar.start();
            var successCallback = function() {
                $scope.paginateUsers();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.deleteUser.bind(httpService.deleteUser, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.deleteUser(id).then(successCallback, errorCallback);
        }
    }
    //#endregion

    //#region Static Phone Number Admin
    $scope.staticPhoneNumbers = [];

    $scope.addStaticPhoneNumber = function (scope) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/staticPhoneNumbersModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "staticPhoneNumber", null)
        });

        modalInstance.result.then(function () {
            $scope.paginateStaticPhoneNumbers();
        });
    };

    $scope.viewStaticPhoneNumber = function (scope) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/staticPhoneNumbersModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "staticPhoneNumber", null)
        });

        modalInstance.result.then(function () {
            $scope.paginateStaticPhoneNumbers();
        });
    };

    $scope.editStaticPhoneNumber = function (scope, staticPhoneNumber) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/staticPhoneNumbersModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "staticPhoneNumber", staticPhoneNumber)
        });

        modalInstance.result.then(function () {
            $scope.paginateStaticPhoneNumbers();
        });
    };

    $scope.deleteStaticPhoneNumber = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to delete this?")) {
            cfpLoadingBar.start();
            var successCallback = function () {
                $scope.paginateStaticPhoneNumbers();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.deleteStaticPhoneNumber.bind(httpService.deleteStaticPhoneNumber, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.deleteStaticPhoneNumber(id).then(successCallback, errorCallback);
        }
    }
    //#endregion

    //#region Templates Admin
    $scope.templates = [];

    $scope.getTemplates = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function(response) {
            cfpLoadingBar.complete();
            $scope.templates = response.data;
            $scope.loadComplete = true;
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getTemplates, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getTemplates().then(successCallback, errorCallback);
    };

    $scope.addTemplate = function (scope) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/templatesModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "template", null)
        });

        modalInstance.result.then(function () {
            $scope.getTemplates();
        });
    };

    $scope.editTemplate = function (scope, template) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/templatesModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "template", template)
        });

        modalInstance.result.then(function () {
            $scope.getTemplates();
        });
    };

    $scope.archiveTemplate = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to delete this?\n\nThis template will be deleted if it hasn't been used or archived along with its related content if it has.")) {
            cfpLoadingBar.start();
            var successCallback = function() {
                $scope.getTemplates();
                cfpLoadingBar.complete();
            };
            var errorCallback = function(error) {
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.archiveTemplate.bind(httpService.archiveTemplate, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.archiveTemplate(id).then(successCallback, errorCallback);
        }
    }
    //#endregion

    //#region Template Items Admin
    $scope.templateId = null;
    $scope.templateName = null;
    $scope.templateItems = [];

    $scope.getTemplateItems = function () {
        $scope.templateId = $location.search().templateId;
        $scope.templateName = $location.search().templateName;
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function(response) {
            cfpLoadingBar.complete();
            $scope.templateItems = response.data;
            $scope.loadComplete = true;
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getTemplateItemsByTemplateId.bind(httpService.getTemplateItemsByTemplateId, $scope.templateId), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getTemplateItemsByTemplateId($scope.templateId).then(successCallback, errorCallback);
    };

    $scope.addTemplateItem = function (scope) {
        var templateItem = {
            "TemplateId": $scope.templateId
        }
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/templateItemsModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "templateItem", templateItem)
        });

        modalInstance.result.then(function () {
            $scope.getTemplateItems();
        });
    };

    $scope.editTemplateItem = function (scope, templateItem) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "./App/Views/Modals/templateItemsModal.html",
            controller: "modalsCtrl",
            size: "lg",
            resolve: getModalVariables(scope, "templateItem", templateItem)
        });

        modalInstance.result.then(function () {
            $scope.getTemplateItems();
        });
    };

    $scope.archiveTemplateItem = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to delete this?\n\nThis template item will be deleted if it hasn't been used or archived along with its related content if it has.")) {
            cfpLoadingBar.start();
            var successCallback = function() {
                $scope.getTemplateItems();
                cfpLoadingBar.complete();
            };
            var errorCallback = function(error) {
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.archiveTemplateItem.bind(httpService.archiveTemplateItem, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.archiveTemplateItem(id).then(successCallback, errorCallback);
        }
    }

    $scope.changeTemplateItemSort = function(direction, id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        cfpLoadingBar.start();
        var successCallback = function() {
            $scope.getTemplateItems();
            cfpLoadingBar.complete();
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.changeTemplateItemSort.bind(httpService.changeTemplateItemSort, direction, id), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.changeTemplateItemSort(direction, id).then(successCallback, errorCallback);
    }
    //#endregion
}]);