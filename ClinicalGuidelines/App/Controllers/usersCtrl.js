//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("usersCtrl", ["$scope", "$rootScope", "$location", "$uibModal", "cfpLoadingBar", "httpService", "$filter", "$state", "modelService", function ($scope, $rootScope, $location, $uibModal, cfpLoadingBar, httpService, $filter, $state, modelService) {
    $scope.loadComplete = false;
    $scope.apiErrors = null;
    $scope.debug = $rootScope.debug;

    $scope.users = [];
    $scope.user = $rootScope.user;
    $scope.editingUser = null;
    $scope.searchUsers = null;
    $scope.selectedUser = {};

    //#region Layout functions - called on all pages
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

    $scope.checkUserPermissions = function () {
        if (!$scope.user) return false;
        return $scope.user.SiteAdmin || $scope.user.LeadClinician;
    };

    $scope.$on("user:updated", function (event, user) {
        $scope.user = user;
    });

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

    $scope.toggleUserSearch = function (toggle) {
        $scope.searchUsers = toggle;
    }
    //#endregion

    //#region Users Admin
    $scope.getUser = function (userId) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        $scope.loadComplete = false;
        cfpLoadingBar.start();
        var successCallback = function (response) {
            cfpLoadingBar.complete();
            $scope.editingUser = response.data;

            if ($scope.departments
                && $scope.departments.length > 0
                && $scope.editingUser
                && $scope.editingUser.AdministationDepartments
                && $scope.editingUser.AdministationDepartments.length > 0) {
                for (var i = 0; i < $scope.editingUser.AdministationDepartments.length; i++) {
                    for (var y = 0; y < $scope.departments.length; y++) {
                        if ($scope.editingUser.AdministationDepartments[i].Id === $scope.departments[y].Id) {
                            $scope.departments[y].selectedAsAdmin = true;
                        }
                    }
                }
            }

            $scope.loadComplete = true;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getUser, successCallback, errorCallback, userId);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getUser(userId).then(successCallback, errorCallback);
    };

    $scope.getUsersByName = function (searchUser) {
        if (!searchUser) return;
        $rootScope.errorRetry = false;
        var successCallback = function (response) {
            $scope.activeDirectoryUsers = response.data;
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getUsersByName.bind(httpService.getUsersByName, searchUser), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getUsersByName(searchUser).then(successCallback, errorCallback);
    }

    $scope.addUserToForm = function (selectedUser) {
        if (!selectedUser) return;
        $scope.editingUser = {
            "SamAccountName": selectedUser.SamAccountName,
            "Forename": selectedUser.Forename,
            "Surname": selectedUser.Surname,
            "JobTitle": selectedUser.JobTitle,
            "PhoneNumber": selectedUser.PhoneNumber,
            "EmailAddress": selectedUser.EmailAddress
        }
        $scope.searchUsers = false;
    }

    $scope.addUser = function (scope) {
        $scope.department = null;
        $scope.editingUser = null;
        $scope.searchUsers = true;
        $state.go("users.form");
    };

    $scope.deleteUser = function (id) {
        $scope.apiErrors = null;
        $rootScope.errorRetry = false;
        if (confirm("Are you sure you want to delete this?")) {
            cfpLoadingBar.start();
            var successCallback = function () {
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

    $scope.saveUser = function () {
        modelService.saveModel("User", $scope);

        var isAlreadySet = function(department) {
            for (var i = 0; i < $scope.editingUser.AdministationDepartments.length; i++) {
                if ($scope.editingUser.AdministationDepartments[i].Id === department.Id) {
                    return true;
                }
            }
            return false;
        };
      
        for (var y = 0; y < $scope.departments.length; y++) {
            //New
            if ($scope.departments[y].selectedAsAdmin) {
                if (!isAlreadySet($scope.departments[y])) {
                    console.log('should add ' + $scope.departments[y].Name);

                    var userId = $scope.editingUser.Id;
                    var departmentId = $scope.departments[y].Id;

                    var successCallback = function () {
                        console.log('success');
                        cfpLoadingBar.complete();
                    };
                    var errorCallback = function (error) {
                        if (error.status === 401) {
                            $scope.apiErrors = 'Unauthorised, you are not allowed to make this change';
                            cfpLoadingBar.complete();
                            return;
                        }
                        if (!$scope.errorRetry) {
                            $rootScope.retryApiCallAfterError(httpService.postUserAdministrationDepartment.bind(httpService.postUserAdministrationDepartment, userId, departmentId), successCallback, errorCallback);
                        } else {
                            cfpLoadingBar.complete();
                            if (error && error.data && error.data.Message) {
                                $scope.apiErrors = 'Failed:' + error.data.Message;
                            } else {
                                $scope.apiErrors = 'Connection lost, please try again.';
                            }
                        }
                    };
                    httpService.postUserAdministrationDepartment(userId, departmentId).then(successCallback, errorCallback);
                }
            }
            //Removed
            else {
                if (isAlreadySet($scope.departments[y])) {
                    console.log('should remove' + $scope.departments[y].Name);

                    var userId = $scope.editingUser.Id;
                    var departmentId = $scope.departments[y].Id;
                    

                    var successCallback = function () {
                        console.log('success');
                        cfpLoadingBar.complete();
                    };
                    var errorCallback = function (error) {
                        if (error.status === 401) {
                            $scope.apiErrors = 'Unauthorised, you are not allowed to make this change';
                            cfpLoadingBar.complete();
                            return;
                        }
                        if (!$scope.errorRetry) {
                            $rootScope.retryApiCallAfterError(httpService.deleteUserAdministrationDepartment.bind(httpService.deleteUserAdministrationDepartment, userId, departmentId), successCallback, errorCallback);
                        } else {
                            cfpLoadingBar.complete();
                            if (error && error.data && error.data.Message) {
                                $scope.apiErrors = 'Failed:' + error.data.Message;
                            } else {
                                $scope.apiErrors = 'Connection lost, please try again.';
                            }
                        }
                    };
                    httpService.deleteUserAdministrationDepartment(userId, departmentId).then(successCallback, errorCallback);
                }
            }
        }
    };

    $scope.modelUpdateSuccessful = function () {
        $scope.editingUser = null;
        $state.go("users.list");
    };

    $scope.initialiseUserForm = function () {
        $scope.getDepartments();
    };

    $scope.departments = null;
    $scope.containers = null;
    $scope.getDepartments = function () {
        $scope.loadComplete = false;
        $rootScope.errorRetry = false;
        var successCallback = function (response) {
            $scope.departments = $filter("filter")(response.data, { Container: false });
            $scope.containers = $filter("filter")(response.data, { Container: true });
            $scope.loadComplete = true;

            var userId = $location.search().userId;

            if (userId) {
                $scope.getUser(userId);
                $scope.searchUsers = false;
            } else {
                $scope.searchUsers = true;
            }
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getAllDepartments, successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, unable to load departments list.";
                }
            }
        };
        httpService.getAllDepartments().then(successCallback, errorCallback);
    };
    //#endregion
}]);