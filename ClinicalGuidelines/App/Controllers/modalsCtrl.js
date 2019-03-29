//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("modalsCtrl", ["$scope", "$rootScope", "httpService", "modelService", "$uibModalInstance", "cfpLoadingBar", "scope", "user", "department", "siteAdmin", "leadClinician", "template", "templateItem", "article", "staticPhoneNumber", function ($scope, $rootScope, httpService, modelService, $uibModalInstance, cfpLoadingBar, scope, user, department, siteAdmin, leadClinician, template, templateItem, article, staticPhoneNumber) {
    $scope.apiErrors = null;
    $scope.siteAdmin = siteAdmin;
    $scope.leadClinician = leadClinician;

    //#region General Modal Functions
    $scope.closeModal = function () {
        $uibModalInstance.dismiss();
    };

    $scope.searchDepartments = function() {
        console.log("Searching...");
    };
    //#endregion

    //#region Helper Functions
    var getStaticPhoneNumberOptions = function() {
        $rootScope.errorRetry = false;
        var successCallback = function (response) {
            for (var i = 0; i < response.data.length; i++) {
                $scope.staticPhoneNumberTypeOptions.push({ Id: i, Name: response.data[i] });
            }
        };
        var errorCallback = function (error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getStaticPhoneNumberTypes(), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, please try again.";
                }
            }
        };
        httpService.getStaticPhoneNumberTypes().then(successCallback, errorCallback);
    }

    var initialise = function () {
        /**
         * If for user admin modal, if a user isn't in scope ie new shows search user field,
         * if a user is in scope loads the departments list which is a requirement for the form
         */
        if (!$scope.departments) $scope.getDepartments();

        if (!$scope.user) {
            $scope.searchUsers = true;
        }

        getStaticPhoneNumberOptions();
    }

    $scope.checkUserPermissions = function () {
        return $scope.siteAdmin || $scope.leadClinician;
    };
    //#endregion

    //#region Model Specific Functions
    $scope.saveModel = function (type) {
        modelService.saveModel(type, $scope);
    };

    $scope.modelUpdateSuccessful = function () {
        $uibModalInstance.close();
    }
    //#endregion

    //#region User Admin Functions
    $scope.user = user;
    $scope.searchUsers = null;
    $scope.selectedUser = null;

    $scope.toggleUserSearch = function (toggle) {
        $scope.searchUsers = toggle;
    }

    $scope.getUsersByName = function (searchUser) {
        if (!searchUser) return;
        $rootScope.errorRetry = false;
        var successCallback = function(response) {
            $scope.activeDirectoryUsers = response.data;
        };
        var errorCallback = function(error) {
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

    $scope.addUserToForm = function () {
        $scope.user = {
            "SamAccountName": $scope.selectedUser.SamAccountName,
            "Forename": $scope.selectedUser.Forename,
            "Surname": $scope.selectedUser.Surname,
            "JobTitle": $scope.selectedUser.JobTitle,
            "PhoneNumber": $scope.selectedUser.PhoneNumber,
            "EmailAddress": $scope.selectedUser.EmailAddress
        }
        if (!$scope.departments) $scope.getDepartments();
        $scope.searchUsers = false;
    }
    //#endregion

    //#region Static Phone Number Admin Functions
    $scope.staticPhoneNumber = staticPhoneNumber;
    $scope.staticPhoneNumberTypeOptions = [];
    //#endregion

    //#region Department Admin Functions
    $scope.department = department;
    $scope.departments = null;

    $scope.getDepartments = function () {
        $scope.loadComplete = false;
        $rootScope.errorRetry = false;
        var successCallback = function(response) {
            $scope.departments = response.data;
            $scope.loadComplete = true;
        };
        var errorCallback = function(error) {
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

    //#region Template Admin Functions
    $scope.template = template;
    $scope.templates = null;

    $scope.getTemplates = function () {
        $scope.loadComplete = false;
        $rootScope.errorRetry = false;
        var successCallback = function(response) {
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
                    $scope.apiErrors = "Connection lost, unable to load templates list.";
                }
            }
        };
        httpService.getTemplates().then(successCallback, errorCallback);
    };
    //#endregion

    //#region Template Items Admin Functions 
    $scope.templateItem = templateItem;
    $scope.templateItems = null;
    $scope.blockTypeValues = ["Text", "Image", "MultipleImages", "Link", "MultipleLinks"];
    $scope.alignmentValues = ["Left", "Center", "Right"];
    //#endregion

    //#region Article Admin Functions
    $scope.article = article;

    $scope.initialiseArticlesForm = function() {
        $scope.getTemplates();
        $scope.getDepartments();
        if ($scope.article) $scope.getDepartmentSubSectionTabs($scope.article.DepartmentId);
    }

    $scope.loadArticleTemplateItems = function () {
        $rootScope.errorRetry = false;
        var successCallback = function(response) {
            $scope.article.Body.TemplateItems = response.data;
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getTemplateItemsByTemplateId.bind(httpService.getTemplateItemsByTemplateId, $scope.article.Body.TemplateId), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, unable to load template items.";
                }
            }
        };
        httpService.getTemplateItemsByTemplateId($scope.article.Body.TemplateId).then(successCallback, errorCallback);
    };

    $scope.loadDepartmentSubSectionTabs = function() {
        $scope.getDepartmentSubSectionTabs($scope.article.DepartmentId);
    };
    //#endregion

    //#region Sub Section Tabs Admin Functions
    $scope.subSectionTabs = [];

    $scope.getDepartmentSubSectionTabs = function (departmentId) {
        $scope.loadComplete = false;
        $rootScope.errorRetry = false;
        var successCallback = function(response) {
            $scope.subSectionTabs = response.data;
            $scope.loadComplete = true;
        };
        var errorCallback = function(error) {
            if (!$scope.errorRetry) {
                $rootScope.retryApiCallAfterError(httpService.getDepartmentSubSectionTabs.bind(httpService.getDepartmentSubSectionTabs, departmentId), successCallback, errorCallback);
            } else {
                cfpLoadingBar.complete();
                if (error && error.data && error.data.Message) {
                    $scope.apiErrors = "Failed: " + error.data.Message;
                } else {
                    $scope.apiErrors = "Connection lost, unable to load list of departments sub sections.";
                }
            }
        };
        httpService.getDepartmentSubSectionTabs(departmentId).then(successCallback, errorCallback);
    }
    //#endregion

    initialise();
}]);