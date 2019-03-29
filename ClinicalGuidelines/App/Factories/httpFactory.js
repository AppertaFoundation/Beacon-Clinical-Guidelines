//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.factory("httpService", ["$http", "$rootScope", function ($http, $rootScope) {
    var httpService = {};

    //GET
    httpService.getUsers = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/user/get");
    };

    httpService.getUser = function (userId) {
        return $http.get($rootScope.httpServiceLocation + "/api/user/get/" + userId);
    };

    httpService.getPaginatedUsers = function (pageSize, pageNumber, searchTerm) {
        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/user/get/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/user/get/" + pageSize + "/" + pageNumber);
    };

    httpService.getPaginatedStaticPhoneNumbers = function (pageSize, pageNumber, searchTerm) {
        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/staticphonenumber/get/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/staticphonenumber/get/" + pageSize + "/" + pageNumber);
    };

    httpService.getStaticPhoneNumberTypes = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/staticphonenumber/get/types");
    };

    httpService.getStaticPhoneNumberByDepartmentId = function (departmentId) {
        return $http.get($rootScope.httpServiceLocation + "api/department/get/" + departmentId + "/phone/all");
    };

    httpService.getStaticPhoneNumberByContainerId = function (containerId) {
        return $http.get($rootScope.httpServiceLocation + "api/department/get/" + containerId + "/phone/all/containerId");
    };

    httpService.getDepartments = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/department/get");
    };

    httpService.getEditableDepartments = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/department/get/editable");
    };
    
    httpService.getAllDepartments = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/department/get/all");
    };

    httpService.getContainerDepartments = function (containerId) {
        return $http.get($rootScope.httpServiceLocation + "/api/department/container/" + containerId + "/get");
    };

    httpService.getDepartmentContainers = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/department/get/containers");
    };

    httpService.getAllDepartmentContainers = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/department/get/containers/all");
    };

    httpService.getEditabeDepartmentContainers = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/department/get/editable/containers");
    };

    httpService.getPaginatedDepartments = function (pageSize, pageNumber, searchTerm) {
        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/department/get/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/department/get/" + pageSize + "/" + pageNumber);
    };

    httpService.getDepartment = function (departmentId) {
        return $http.get($rootScope.httpServiceLocation + "/api/department/get/" + departmentId);
    };

    httpService.getDepartmentSubSectionTabs = function(departmentId) {
        return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/department/" + departmentId);
    };

    httpService.getGlobalSubSectionTabs = function() {
        return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/global/");
    };

    httpService.getDepartmentNonGlobalSubSectionTabs = function (departmentId) {
        return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/nonglobal/" + departmentId);
    };

    httpService.getSubSectionTabs = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/");
    };

    httpService.getDepartmentSubSectionTabs = function(departmentId) {
        return $http.get($rootScope.httpServiceLocation + "api/subsectiontab/get/department/" + departmentId);
    }

    httpService.getPaginatedSubSectionTabs = function (pageSize, pageNumber, searchTerm) {
        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/" + pageSize + "/" + pageNumber);
    };

    httpService.getPaginatedLiveSubSectionTabs = function (pageSize, pageNumber, searchTerm) {
        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/live/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/live/" + pageSize + "/" + pageNumber);
    };

    httpService.getSubSectionTab = function (subSectionTabId) {
        return $http.get($rootScope.httpServiceLocation + "/api/subsectiontab/get/" + subSectionTabId);
    };

    httpService.getArticles = function() {
        return $http.get($rootScope.httpServiceLocation + "/api/article/get");
    };

    httpService.getPaginatedArticles = function (pageSize, pageNumber, container, department, subSectionTab, searchTerm) {
        if (!container) container = 0;
        if (!department) department = 0;
        if (!subSectionTab) subSectionTab = 0;

        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/article/get/" + container + "/" + department + "/" + subSectionTab + "/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/article/get/" + container + "/" + department + "/" + subSectionTab + "/" + pageSize + "/" + pageNumber);
    };

    httpService.getPaginatedLiveArticles = function (pageSize, pageNumber, container, department, subSectionTab, searchTerm) {
        if (!container) container = 0;
        if (!department) department = 0;
        if (!subSectionTab) subSectionTab = 0;

        console.log($rootScope.httpServiceLocation + "/api/article/get/live/" + container + "/" + department + "/" + subSectionTab + "/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/article/get/live/" + container + "/" + department + "/" + subSectionTab + "/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/article/get/live/" + container + "/" + department + "/" + subSectionTab + "/" + pageSize + "/" + pageNumber);
    };

    httpService.getPaginatedArticleApprovals = function (pageSize, pageNumber, searchTerm) {
        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/article/approvals/get/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/article/approvals/get/" + pageSize + "/" + pageNumber);
    };

    httpService.getArticle = function(articleId) {
        return $http.get($rootScope.httpServiceLocation + "/api/article/get/" + articleId);
    };

    httpService.getArticleRevision = function (articleId, revisionId) {
        return $http.get($rootScope.httpServiceLocation + "/api/article/get/" + articleId + "/revision/" + revisionId);
    };

    httpService.getArticleHistory = function(articleId) {
        return $http.get($rootScope.httpServiceLocation + "/api/article/get/" + articleId + "/history");
    };

    httpService.getArticleTemplates = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/article/template/get");
    };

    httpService.getPaginatedArticleTemplates = function (pageSize, pageNumber, searchTerm) {
        if (searchTerm) {
            return $http.get($rootScope.httpServiceLocation + "/api/article/template/get/" + pageSize + "/" + pageNumber + "/" + searchTerm);
        }
        return $http.get($rootScope.httpServiceLocation + "/api/article/template/get/" + pageSize + "/" + pageNumber);
    };

    httpService.getArticleTemplate = function (articleTemplateId) {
        return $http.get($rootScope.httpServiceLocation + "/api/article/template/get/" + articleTemplateId);
    };

    httpService.loadCurrentUser = function () {
        return $http.get($rootScope.httpServiceLocation + "/api/user/get/current");
    };

    httpService.getUsersByName = function (searchName) {
        return $http({
            method: "get",
            url: $rootScope.httpServiceLocation + "/api/user/get/list/",
            params: {
                "searchName": searchName
            }
        });
    };

    //POST
    httpService.postDepartment = function (department) {
        return $http({
            method: "post",
            url: $rootScope.httpServiceLocation + "/api/department/post",
            data: department
        });
    };

    httpService.postUser = function (user) {
        return $http({
            method: "post",
            url: $rootScope.httpServiceLocation + "/api/user/post",
            data: user
        });
    };

    httpService.postStaticPhoneNumber = function (staticPhoneNumber) {
        return $http({
            method: "post",
            url: $rootScope.httpServiceLocation + "/api/staticphonenumber/post",
            data: staticPhoneNumber
        });
    };

    httpService.postArticleNew = function (article) {
        return $http({
            method: "post",
            url: $rootScope.httpServiceLocation + "/api/article/post",
            data: article
        });
    };
    
    httpService.postArticleTemplate = function (articleTemplate) {
        return $http({
            method: "post",
            url: $rootScope.httpServiceLocation + "/api/article/template/post",
            data: articleTemplate
        });
    };

    httpService.postSubSectionTab = function (subSectionTab) {
        return $http({
            method: "post",
            url: $rootScope.httpServiceLocation + "/api/subsectiontab/post",
            data: subSectionTab
        });
    };

    httpService.postUserAdministrationDepartment = function (userId, departmentId) {
        return $http.post($rootScope.httpServiceLocation + "/api/user/post/administration-department/" + userId + "/" + departmentId);
    };

    httpService.postDepartmentSubSection = function (departmentId, subSectionTabId) {
        return $http.post($rootScope.httpServiceLocation + "/api/department/post/" + departmentId + "/subsectiontab/" + subSectionTabId);
    };

    //PUT
    httpService.putDepartment = function (id, department) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/department/put/" + id,
            data: department
        });
    };

    httpService.putUser = function (id, user) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/user/put/" + id,
            data: user
        });
    };

    httpService.putStaticPhoneNumber = function (id, staticPhoneNumber) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/staticphonenumber/put/" + id,
            data: staticPhoneNumber
        });
    };

    httpService.putArticle = function (id, article) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/article/put/" + id,
            data: article
        });
    };

    httpService.putArticleNew = function (id, article) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/article/put/" + id + "/revision",
            data: article
        });
    };

    httpService.putArticleReject = function (id, article) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/article/reject/" + id,
            data: article
        });
    };

    httpService.putArticleApprove = function (id, article) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/article/approve/" + id,
            data: article
        });
    };

    httpService.putArticleTemplate = function (id, articleTemplate) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/article/template/put/" + id,
            data: articleTemplate
        });
    };

    httpService.putSubSectionTab = function (id, subSectionTab) {
        return $http({
            method: "put",
            url: $rootScope.httpServiceLocation + "/api/subsectiontab/put/" + id,
            data: subSectionTab
        });
    };

    httpService.archiveDepartment = function (id) {
        return $http.put($rootScope.httpServiceLocation + "/api/department/archive/" + id);
    };

    httpService.unArchiveDepartment = function (id) {
        return $http.put($rootScope.httpServiceLocation + "/api/department/unarchive/" + id);
    };

    httpService.archiveSubSectionTab = function (id) {
        return $http.put($rootScope.httpServiceLocation + "/api/subsectiontab/archive/" + id);
    };

    httpService.unArchiveSubSectionTab = function (id) {
        return $http.put($rootScope.httpServiceLocation + "/api/subsectiontab/unarchive/" + id);
    };

    httpService.archiveArticle = function (id) {
        return $http.put($rootScope.httpServiceLocation + "/api/article/archive/" + id);
    };

    httpService.unArchiveArticle = function (id) {
        return $http.put($rootScope.httpServiceLocation + "/api/article/unarchive/" + id);
    };

    //DELETE
    httpService.deleteArticleTemplate = function (id) {
        return $http.delete($rootScope.httpServiceLocation + "/api/article/template/delete/" + id);
    };

    httpService.deleteUser = function (id) {
        return $http.delete($rootScope.httpServiceLocation + "/api/user/delete/" + id);
    };

    httpService.deleteStaticPhoneNumber = function (id) {
        return $http.delete($rootScope.httpServiceLocation + "/api/staticphonenumber/delete/" + id);
    };

    httpService.deleteUserAdministrationDepartment = function (userId, departmentId) {
        return $http.delete($rootScope.httpServiceLocation + "/api/user/delete/administration-department/" + userId + "/" + departmentId);
    };

    httpService.deleteDepartmentSubSection = function (departmentId, subSectionTabId) {
        return $http.delete($rootScope.httpServiceLocation + "/api/department/delete/" + departmentId + "/subsectiontab/" + subSectionTabId);
    };

    return httpService;
}]);