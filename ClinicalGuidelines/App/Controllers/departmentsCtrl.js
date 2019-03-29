//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("departmentsCtrl", ["$scope", "$rootScope", "$filter", "$state", "$timeout", "mdPickerColors", "$location", "cfpLoadingBar", "httpService", "modelService", function ($scope, $rootScope, $filter, $state, $timeout, mdPickerColors, $location, cfpLoadingBar, httpService, modelService) {
    $scope.departments = [];
    $scope.department = null;
    $scope.departmentContainers = [];
    $scope.themeCorrect = false;
    $scope.containerFieldsValid = false;

    $scope.search = { subSection: '' };
    $scope.subSectionTabs = [];

    var optionSuffix = " departments per page";
    $scope.pageSizeOptions = [{ Id: 5, Name: 5 + optionSuffix }, { Id: 10, Name: 10 + optionSuffix }, { Id: 25, Name: 25 + optionSuffix }, { Id: 50, Name: 50 + optionSuffix }, { Id: 100, Name: 100 + optionSuffix }];
    $scope.pagination = { pageSize: 10, currentPage: 1, totalItems: 10, maxSize: 5, searchTerm: null };
    $scope.subSectionTabPagination = { pageSize: 10, currentPage: 1, totalItems: 10, maxSize: 5, searchTerm: null };    

    $scope.setPage = function (pageNo) {
        console.log("select page called:" + pageNo);
        $scope.pagination.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log("Page changed to: " + $scope.pagination.currentPage);
        $scope.paginateDepartments();
    };

    $scope.subSectionTabPageChanged = function () {
        console.log("Page changed to: " + $scope.subSectionTabPagination.currentPage);
        $scope.paginateLiveSubSectionTabs();
    };

    $scope.checkDepartmentPermissions = function () {
        if (!$scope.user) return false;
        return $scope.user.SiteAdmin || $scope.user.LeadClinician;
    };

    $scope.validateContainerFields = function () {
        if (!$scope.department) return;

        $scope.containerFieldsValid = false;

        if ($scope.department.Container) {
            $scope.department.ContainerId = null;
            $scope.containerFieldsValid = true;
        } else {
            if ($scope.department.ContainerId) $scope.containerFieldsValid = true;
        }
    };

    $scope.paginateDepartments = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            console.log(response);
            $scope.departments = response.data.Departments;
            $scope.getDepartmentContainers();
            $scope.pagination.totalItems = response.data.TotalCount;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getPaginatedDepartments.bind(httpService.getPaginatedDepartments, $scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getPaginatedDepartments($scope.pagination.pageSize, $scope.pagination.currentPage, $scope.pagination.searchTerm).then(successCallback, errorCallback);
    };

    $scope.getDepartments = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.departments = response.data;
            $scope.getDepartmentContainers();
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
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

    $scope.getDepartment = function (departmentId) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.department = response.data;
            $scope.department.MainColour = mdPickerColors.getColor(($scope.department.MainColour) ? $scope.department.MainColour.toLowerCase() : "");
            $scope.department.SubColour = mdPickerColors.getColor(($scope.department.SubColour) ? $scope.department.SubColour.toLowerCase() : "");
            $scope.department.BackgroundColour = mdPickerColors.getColor(($scope.department.BackgroundColour) ? $scope.department.BackgroundColour.toLowerCase() : "");
            $scope.department.SideColourVariationOne = mdPickerColors.getColor(($scope.department.SideColourVariationOne) ? $scope.department.SideColourVariationOne.toLowerCase() : "");
            $scope.department.SideColourVariationTwo = mdPickerColors.getColor(($scope.department.SideColourVariationTwo) ? $scope.department.SideColourVariationTwo.toLowerCase() : "");
            $scope.department.SideColourVariationThree = mdPickerColors.getColor(($scope.department.SideColourVariationThree) ? $scope.department.SideColourVariationThree.toLowerCase() : "");
            $scope.department.SideColourVariationFour = mdPickerColors.getColor(($scope.department.SideColourVariationFour) ? $scope.department.SideColourVariationFour.toLowerCase() : "");
            $scope.department.SideColourVariationFive = mdPickerColors.getColor(($scope.department.SideColourVariationFive) ? $scope.department.SideColourVariationFive.toLowerCase() : "");
            $scope.checkTheme();
            $scope.validateContainerFields();
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getDepartment.bind(httpService.getDepartment, departmentId), successCallback, errorCallback);
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

    $scope.getDepartmentContainers = function() {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.departmentContainers = response.data;
            for (var i = 0; i < $scope.departments.length; i++) {
                if ($scope.departments[i].Container === false && $scope.departments[i].ContainerId) {
                    $scope.departments[i].ContainerName = $filter("filter")($scope.departmentContainers, { Id: $scope.departments[i].ContainerId }, true)[0].Name;
                }
            }
            $scope.validateContainerFields();
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getAllDepartmentContainers, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getAllDepartmentContainers().then(successCallback, errorCallback);
    };

    $scope.addDepartment = function () {
        $scope.department = null;
        $state.go("departments.form");
    };

    $scope.saveDepartment = function () {
        modelService.saveModel("Department", $scope);
    };

    $scope.modelUpdateSuccessful = function() {
        $scope.department = null;
        $state.go("departments.list");
    };

    $scope.initialiseDepartmentsForm = function() {
        var departmentId = $location.search().departmentId;
        $scope.themeCorrect = false;
        if (departmentId) {
            $scope.getDepartment(departmentId);
            $scope.getDepartmentContainers();
        } else {
            if (!$scope.department) $scope.department = {};
        }
    };

    $scope.checkTheme = function() {
        $scope.themeCorrect = false;
        if ($scope.department.Container) {
            $timeout(function () {
                if ($scope.department.MainColour) $scope.themeCorrect = true;
            }, 100);
        } else {
            $timeout(function () {
                if ($scope.department.MainColour &&
                    $scope.department.SubColour &&
                    $scope.department.BackgroundColour &&
                    $scope.department.SideColourVariationOne) $scope.themeCorrect = true;
            }, 100);
        }
    };

    $scope.archiveDepartment = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to archive this?\n\nAll articles and contacts associated to this department will no longer be available in the app and if this is a container all departments associated with it will become unavailable in the app.")) {
            cfpLoadingBar.start();
            var successCallback = function () {
                $scope.paginateDepartments();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.archiveDepartment.bind(httpService.archiveDepartment, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.archiveDepartment(id).then(successCallback, errorCallback);
        }
    }

    $scope.unArchiveDepartment = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to unarchive this?")) {
            cfpLoadingBar.start();
            var successCallback = function () {
                $scope.paginateDepartments();
                cfpLoadingBar.complete();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    $scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$scope.errorRetry) {
                    $rootScope.retryApiCallAfterError(httpService.unArchiveDepartment.bind(httpService.unArchiveDepartment, id), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        $scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        $scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            httpService.unArchiveDepartment(id).then(successCallback, errorCallback);
        }
    }

    $scope.searchSubSections = function () {
        console.log($scope.search);
        return false;
    }

    $scope.paginateLiveSubSectionTabs = function () {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            console.log(response);
            $scope.subSectionTabs = response.data.SubSectionTabs;
            $scope.subSectionTabPagination.totalItems = response.data.TotalCount;
            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getPaginatedLiveSubSectionTabs.bind(httpService.getPaginatedLiveSubSectionTabs, $scope.subSectionTabPagination.pageSize, $scope.subSectionTabPagination.currentPage, $scope.subSectionTabPagination.searchTerm), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getPaginatedLiveSubSectionTabs($scope.subSectionTabPagination.pageSize, $scope.subSectionTabPagination.currentPage, $scope.subSectionTabPagination.searchTerm).then(successCallback, errorCallback);
    };

    $scope.addSubSectionToDepartment = function (subSectionTabId) {
        var departmentId = $scope.department.Id;

        var successCallback = function () {
            console.log('success');
            cfpLoadingBar.complete();
            $scope.paginateLiveSubSectionTabs();
            $scope.initialiseDepartmentsForm();
        };
        var errorCallback = function (error) {
            if (error.status === 401) {
                $scope.apiErrors = 'Unauthorised, you are not allowed to make this change';
                cfpLoadingBar.complete();
                return;
            }
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.postDepartmentSubSection.bind(httpService.postDepartmentSubSection, departmentId, subSectionTabId), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = 'Failed:' + error.data.Message;
                } else {
                    $scope.apiErrors = 'Connection lost, please try again.';
                }
            }
        };
        httpService.postDepartmentSubSection(departmentId, subSectionTabId).then(successCallback, errorCallback);
    };

    $scope.removeSubSectionFromDepartment = function (subSectionTabId) {
        var departmentId = $scope.department.Id;

        var successCallback = function () {
            console.log('success');
            cfpLoadingBar.complete();
            $scope.paginateLiveSubSectionTabs();
            $scope.initialiseDepartmentsForm();
        };
        var errorCallback = function (error) {
            if (error.status === 401) {
                $scope.apiErrors = 'Unauthorised, you are not allowed to make this change';
                cfpLoadingBar.complete();
                return;
            }
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.deleteDepartmentSubSection.bind(httpService.postDepartmentSubSection, departmentId, subSectionTabId), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = 'Failed:' + error.data.Message;
                } else {
                    $scope.apiErrors = 'Connection lost, please try again.';
                }
            }
        };
        httpService.deleteDepartmentSubSection(departmentId, subSectionTabId).then(successCallback, errorCallback);
    };

    $scope.isSubSectionTabAlreadyAssignedToDepartment = function (subSectionTabId) {
        if (!$scope.department.SubSectionTabs) return false;

        var subSection = $filter("filter")($scope.department.SubSectionTabs, { Id: subSectionTabId }, true);

        return (subSection && subSection.length > 0);
    };
}]);