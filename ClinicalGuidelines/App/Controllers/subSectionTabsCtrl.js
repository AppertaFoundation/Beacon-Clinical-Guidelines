//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("subSectionTabsCtrl", ["$scope", "$rootScope", "$q", "$filter", "$state", "$location", "cfpLoadingBar", "httpService", "modelService", function ($scope, $rootScope, $q, $filter, $state, $location, cfpLoadingBar, httpService, modelService) {
    $scope.loadComplete = false;
    $scope.apiErrors = null;
    $scope.debug = $rootScope.debug;

    $scope.subSectionTabs = [];
    $scope.subSectionTab = {};
    $scope.departments = [];

    var optionSuffix = " sections per page";
    $scope.pageSizeOptions = [{ Id: 5, Name: 5 + optionSuffix }, { Id: 10, Name: 10 + optionSuffix }, { Id: 25, Name: 25 + optionSuffix }, { Id: 50, Name: 50 + optionSuffix }, { Id: 100, Name: 100 + optionSuffix }];
    $scope.pagination = { pageSize: 10, currentPage: 1, totalItems: 10, maxSize: 5, searchTerm: null }

    $scope.setPage = function (pageNo) {
        console.log("select page called:" + pageNo);
        $scope.pagination.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log("Page changed to: " + $scope.pagination.currentPage);
        $scope.paginateSubSectionTabs();
    };

    $scope.checkSubSectionPermissions = function () {
        if (!$scope.user) return false;
        return $scope.user.SiteAdmin || $scope.user.LeadClinician;
    };

    $scope.paginateSubSectionTabs = function() {
        $q.when($scope.getDepartments()).then(
            function () {
                $scope.apiErrors = null;
                $rootScope.errorRetry = false;
                $scope.loadComplete = false;
                cfpLoadingBar.start();
                var successCallback = function(response) {
                    cfpLoadingBar.complete();
                    console.log(response);
                    $scope.subSectionTabs = response.data.SubSectionTabs;
                    for (var i = 0; i < $scope.subSectionTabs.length; i++) {
                        $scope.subSectionTabs[i].Department = $scope.getDepartmentName($scope.subSectionTabs[i]);
                    }
                    $scope.pagination.totalItems = response.data.TotalCount;
                    $scope.loadComplete = true;
                };
                var errorCallback = function(error) {
                    if (!$scope.errorRetry) {
                        $rootScope.retryApiCallAfterError(httpService.getPaginatedSubSectionTabs.bind(httpService.getPaginatedSubSectionTabs, $scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm), successCallback, errorCallback);
                    } else {
                        cfpLoadingBar.complete();
                        if (error && error.data && error.data.Message) {
                            $scope.apiErrors = "Failed: " + error.data.Message;
                        } else {
                            $scope.apiErrors = "Connection lost, please try again.";
                        }
                    }
                };
                httpService.getPaginatedSubSectionTabs($scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm).then(successCallback, errorCallback);
            }
        );
    };

    $scope.getSubSectionTabs = function () {
        $q.when($scope.getDepartments()).then(
            function() {
                $scope.loadComplete = false;
                $rootScope.errorRetry = false;
                var successCallback = function (response) {
                    $scope.subSectionTabs = response.data;

                    for (var i = 0; i < $scope.subSectionTabs.length; i++) {
                        $scope.subSectionTabs[i].Department = $scope.getDepartmentName($scope.subSectionTabs[i]);
                    }

                    $scope.loadComplete = true;
                };
                var errorCallback = function (error) {
                    if (!$scope.errorRetry) {
                        $rootScope.retryApiCallAfterError(httpService.getSubSectionTabs, successCallback, errorCallback);
                    } else {
                        cfpLoadingBar.complete();
                        if (error && error.data && error.data.Message) {
                            $scope.apiErrors = "Failed: " + error.data.Message;
                        } else {
                            $scope.apiErrors = "Connection lost, unable to load list of departments sub sections.";
                        }
                    }
                };
                httpService.getSubSectionTabs().then(successCallback, errorCallback);
            }
        );
    };

    $scope.getSubSectionTab = function(subSectionTabId) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function(response) {
            cfpLoadingBar.complete();
            $scope.subSectionTab = response.data;
            $("#Icon").iconpicker({
                icon: $scope.subSectionTab.Icon,
                iconset: "ionicon"
            });
            $("#Icon").on("change", function (e) {
                $scope.$apply(
                    function () {
                        $scope.subSectionTab.Icon = e.icon;
                    }
                );
            });
            $scope.loadComplete = true;
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getSubSectionTab, successCallback, errorCallback, subSectionTabId);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getSubSectionTab(subSectionTabId).then(successCallback, errorCallback);
    };

    $scope.addSubSectionTab = function() {
        $scope.subSectionTab = null;
        $state.go("subSectionTabs.form");
    };

    $scope.saveSubSectionTab = function() {
        modelService.saveModel("SubSectionTab", $scope);
    };

    $scope.modelUpdateSuccessful = function() {
        $scope.subSectionTab = null;
        $state.go("subSectionTabs.list");
    };

    $scope.getDepartments = function() {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function(response) {
            cfpLoadingBar.complete();

            $scope.departments = response.data;
            $scope.departments.push({
                Id: null,
                ShortName: "All"
            });
            $scope.departments = $filter("orderBy")($scope.departments, "ShortName", false);
            $scope.loadComplete = true;
            return $q.when(true);
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
            return $q.when(false);
        };
        return httpService.getDepartments().then(successCallback, errorCallback);
    };

    $scope.initialiseSubSectionTabsForm = function() {
        var subSectionTabId = $location.search().subSectionTabId;
        $scope.subSectionTab = {};
        if (subSectionTabId) {
            $scope.getSubSectionTab(subSectionTabId);
            $scope.getDepartments();
        } else {
            if (!$scope.subSectionTab) {
                $scope.subSectionTab = {};
            }
            $("#Icon").iconpicker({
                icon: "",
                iconset: "ionicon"
            });
            $("#Icon").on("change", function (e) {
                $scope.$apply(
                    function () {
                        $scope.subSectionTab.Icon = e.icon;
                    }
                );
            });           
            $scope.getDepartments();
        }
    };

    $scope.archiveSubSectionTab = function(id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to archive this?\n\nArchiving this subsection will hide any content associated with it in the app for whatever departments it is set to.")) {
            cfpLoadingBar.start();
            var successCallback = function() {
                $scope.paginateSubSectionTabs();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.archiveSubSectionTab.bind(httpService.archiveSubSectionTab, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.archiveSubSectionTab(id).then(successCallback, errorCallback);
        }
    };

    $scope.unArchiveSubSectionTab = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to unarchive this?")) {
            cfpLoadingBar.start();
            var successCallback = function () {
                $scope.paginateSubSectionTabs();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.unArchiveSubSectionTab.bind(httpService.unArchiveSubSectionTab, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.unArchiveSubSectionTab(id).then(successCallback, errorCallback);
        }
    };

    $scope.getDepartmentName = function (subSectionTab) {
        var departmentName;

        if (subSectionTab.DepartmentId === null) {
            departmentName = "All";
        } else {
            var department = $filter("filter")($scope.departments, { Id: subSectionTab.DepartmentId }, true)[0];
            departmentName = department.ShortName;
        }
        return departmentName;
    };
}]);

