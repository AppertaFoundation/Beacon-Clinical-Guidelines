//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.factory("modelService", ["$http", "$rootScope", "httpService", "cfpLoadingBar", function ($http, $rootScope, httpService, cfpLoadingBar) {
    var modelService = {};

    var postModel = function (type, model, scope) {
        var serviceToCall = null;

        switch (type) {
            case "Department":
                serviceToCall = httpService.postDepartment;
                break;
            case "User":
                serviceToCall = httpService.postUser;
                break;
            case "StaticPhoneNumber":
                serviceToCall = httpService.postStaticPhoneNumber;
                break;
            case "ArticleNew":
                serviceToCall = httpService.postArticleNew;
                break;
            case "ArticleTemplate":
                serviceToCall = httpService.postArticleTemplate;
                break;
            case "SubSectionTab":
                serviceToCall = httpService.postSubSectionTab;
                break;
        }

        if (typeof serviceToCall === "function") {
            cfpLoadingBar.start();
            $rootScope.errorRetry = false;
            var successCallback = function () {
                cfpLoadingBar.complete();
                scope.modelUpdateSuccessful();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$rootScope.errorRetry) {
                    $rootScope.retryApiCallAfterError(serviceToCall.bind(serviceToCall, model), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            serviceToCall(model).then(successCallback, errorCallback);
        }
    };

    var putModel = function (type, model, putId, scope) {
        var serviceToCall = null;

        switch (type) {
            case "Department":
                serviceToCall = httpService.putDepartment;
                break;
            case "User":
                serviceToCall = httpService.putUser;
                break;
            case "StaticPhoneNumber":
                serviceToCall = httpService.putStaticPhoneNumber;
                break;
            case "Article":
                serviceToCall = httpService.putArticle;
                break;
            case "ArticleNew":
                serviceToCall = httpService.putArticleNew;
                break;
            case "ArticleReject":
                serviceToCall = httpService.putArticleReject;
                break;
            case "ArticleApprove":
                serviceToCall = httpService.putArticleApprove;
                break;
            case "ArticleTemplate":
                serviceToCall = httpService.putArticleTemplate;
                break;
            case "SubSectionTab":
                serviceToCall = httpService.putSubSectionTab;
                break;
        }

        if (typeof serviceToCall === "function" && putId) {
            cfpLoadingBar.start();
            $rootScope.errorRetry = false;
            var successCallback = function () {
                cfpLoadingBar.complete();
                scope.modelUpdateSuccessful();
            };
            var errorCallback = function (error) {
                if (error.status === 401) {
                    scope.apiErrors = "Unauthorised, you are not allowed to make this change";
                    cfpLoadingBar.complete();
                    return;
                }
                if (!$rootScope.errorRetry) {
                    $rootScope.retryApiCallAfterError(serviceToCall.bind(serviceToCall, putId, model), successCallback, errorCallback);
                } else {
                    cfpLoadingBar.complete();
                    if (error && error.data && error.data.Message) {
                        scope.apiErrors = "Failed: " + error.data.Message;
                    } else {
                        scope.apiErrors = "Connection lost, please try again.";
                    }
                }
            };
            serviceToCall(putId, model).then(successCallback, errorCallback);
        }
    };

    var convertCsharpDateToJavacript = function (cSharpDate) {
        console.log('c# date before: ', cSharpDate);
        var date = new Date(Date.parse(cSharpDate));
        console.log('c# date after: ', date);
        return date;
    }

    modelService.saveModel = function (type, scope) {
        var model;
        var files = [];
        var articleDepartments = [];
        var i;
        var approval = null;
        var approvalComments = null;
        switch (type) {
            case "Department":
                if (scope.department.Id) {
                    model = {
                        "Id": scope.department.Id,
                        "_id": scope.department._id,
                        "Name": scope.department.Name,
                        "ShortName": scope.department.ShortName,
                        "MainColour": (scope.department.MainColour && scope.department.MainColour.hex) ? scope.department.MainColour.hex : null,
                        "SubColour": (scope.department.SubColour && scope.department.SubColour.hex) ? scope.department.SubColour.hex : null,
                        "BackgroundColour": (scope.department.BackgroundColour && scope.department.BackgroundColour.hex) ? scope.department.BackgroundColour.hex : null,
                        "SideColourVariationOne": (scope.department.SideColourVariationOne && scope.department.SideColourVariationOne.hex) ? scope.department.SideColourVariationOne.hex : null,
                        "SideColourVariationTwo": (scope.department.SideColourVariationTwo && scope.department.SideColourVariationTwo.hex) ? scope.department.SideColourVariationTwo.hex : null,
                        "SideColourVariationThree": (scope.department.SideColourVariationThree && scope.department.SideColourVariationThree.hex) ? scope.department.SideColourVariationThree.hex : null,
                        "SideColourVariationFour": (scope.department.SideColourVariationFour && scope.department.SideColourVariationFour.hex) ? scope.department.SideColourVariationFour.hex : null,
                        "SideColourVariationFive": (scope.department.SideColourVariationFive && scope.department.SideColourVariationFive.hex) ? scope.department.SideColourVariationFive.hex : null,
                        "Container": scope.department.Container,
                        "ContainerId": scope.department.ContainerId,
                        "Live": scope.department.Live
                    };
                    putModel(type, model, scope.department.Id, scope);
                } else {
                    model = {
                        "Name": scope.department.Name,
                        "ShortName": scope.department.ShortName,
                        "MainColour": (scope.department.MainColour && scope.department.MainColour.hex) ? scope.department.MainColour.hex : null,
                        "SubColour": (scope.department.SubColour && scope.department.SubColour.hex) ? scope.department.SubColour.hex : null,
                        "BackgroundColour": (scope.department.BackgroundColour && scope.department.BackgroundColour.hex) ? scope.department.BackgroundColour.hex : null,
                        "SideColourVariationOne": (scope.department.SideColourVariationOne && scope.department.SideColourVariationOne.hex) ? scope.department.SideColourVariationOne.hex : null,
                        "SideColourVariationTwo": (scope.department.SideColourVariationTwo && scope.department.SideColourVariationTwo.hex) ? scope.department.SideColourVariationTwo.hex : null,
                        "SideColourVariationThree": (scope.department.SideColourVariationThree && scope.department.SideColourVariationThree.hex) ? scope.department.SideColourVariationThree.hex : null,
                        "SideColourVariationFour": (scope.department.SideColourVariationFour && scope.department.SideColourVariationFour.hex) ? scope.department.SideColourVariationFour.hex : null,
                        "SideColourVariationFive": (scope.department.SideColourVariationFive && scope.department.SideColourVariationFive.hex) ? scope.department.SideColourVariationFive.hex : null,
                        "Container": scope.department.Container,
                        "ContainerId": scope.department.ContainerId,
                        "Live": scope.department.Live
                    };
                    postModel(type, model, scope);
                }
                break;
            case "User":
                if (scope.editingUser.Id) {
                    model = {
                        "Id": scope.editingUser.Id,
                        "_id": scope.editingUser._id,
                        "DepartmentId": scope.editingUser.DepartmentId,
                        "UserName": scope.editingUser.SamAccountName,
                        "Forename": scope.editingUser.Forename,
                        "Surname": scope.editingUser.Surname,
                        "JobTitle": scope.editingUser.JobTitle,
                        "PhoneNumber": scope.editingUser.PhoneNumber,
                        "EmailAddress": scope.editingUser.EmailAddress,
                        "SiteAdmin": scope.editingUser.SiteAdmin,
                        "LeadClinician": scope.editingUser.LeadClinician,
                        "ContentApprover": scope.editingUser.ContentApprover,
                        "ContentEditor": scope.editingUser.ContentEditor,
                        "Contact": scope.editingUser.Contact
                    };
                    putModel(type, model, scope.editingUser.Id, scope);
                } else {
                    model = {
                        "DepartmentId": scope.editingUser.DepartmentId,
                        "UserName": scope.editingUser.SamAccountName,
                        "Forename": scope.editingUser.Forename,
                        "Surname": scope.editingUser.Surname,
                        "JobTitle": scope.editingUser.JobTitle,
                        "PhoneNumber": scope.editingUser.PhoneNumber,
                        "EmailAddress": scope.editingUser.EmailAddress,
                        "SiteAdmin": scope.editingUser.SiteAdmin,
                        "LeadClinician": scope.editingUser.LeadClinician,
                        "ContentApprover": scope.editingUser.ContentApprover,
                        "ContentEditor": scope.editingUser.ContentEditor,
                        "Contact": scope.editingUser.Contact
                    };
                    postModel(type, model, scope);
                }
                break;
            case "StaticPhoneNumber":
                if (scope.staticPhoneNumber.Id) {
                    model = {
                        "Id": scope.staticPhoneNumber.Id,
                        "_id": scope.staticPhoneNumber._id,
                        "DepartmentId": scope.staticPhoneNumber.DepartmentId,
                        "DepartmentBackgroundColour": scope.staticPhoneNumber.DepartmentBackgroundColour,
                        "DepartmentMainColour": scope.staticPhoneNumber.DepartmentMainColour,
                        "Title": scope.staticPhoneNumber.Title,
                        "PhoneNumber": scope.staticPhoneNumber.PhoneNumber,
                        "Type": scope.staticPhoneNumber.Type
                    };
                    putModel(type, model, scope.staticPhoneNumber.Id, scope);
                } else {
                    model = {
                        "DepartmentId": scope.staticPhoneNumber.DepartmentId,
                        "DepartmentBackgroundColour": scope.staticPhoneNumber.DepartmentBackgroundColour,
                        "DepartmentMainColour": scope.staticPhoneNumber.DepartmentMainColour,
                        "Title": scope.staticPhoneNumber.Title,
                        "PhoneNumber": scope.staticPhoneNumber.PhoneNumber,
                        "Type": scope.staticPhoneNumber.Type
                    };
                    postModel(type, model, scope);
                }
                break;
            case "ArticleNew": //either a new revision or completely new article
                for (i in scope.article.images) {
                    if (scope.article.images.hasOwnProperty(i)) {
                        files.push({
                            "Id": scope.article.images[i].Id,
                            "Type": scope.article.images[i].Type,
                            "Host": scope.article.images[i].Host,
                            "Extension": scope.article.images[i].Extension,
                            "UploadedBy": scope.article.images[i].UploadedBy,
                            "OriginalFileName": scope.article.images[i].OriginalFileName,
                            "OriginalFileSize": scope.article.images[i].OriginalFileSize
                        });
                    }
                }

                for (i in scope.articleDepartmentOptions) {
                    if (scope.articleDepartmentOptions.hasOwnProperty(i)) {
                        articleDepartments.push({
                            "Id": scope.articleDepartmentOptions[i]
                        });
                    }
                } 

                approval = (scope.article.Approval) ? scope.article.Approval : null;
                approvalComments = (scope.article.ApprovalComments) ? scope.article.ApprovalComments : null;
                
                if (scope.article.Id) {
                    model = {
                        "Id": scope.article.Id,
                        "_id": scope.article._id,
                        "RevisionId": scope.article.RevisionId,
                        "Title": scope.article.Title,
                        "Body": scope.article.Body,
                        "Structure": scope.article.Structure,
                        "UserId": $rootScope.user.Id,
                        "Departments": articleDepartments,
                        "SubSectionTabId": scope.article.SubSectionTabId,
                        "Template": scope.article.Template,
                        "Terms": scope.article.Terms,
                        "Live": scope.article.Live,
                        "Approval": approval,
                        "ApprovalComments": approvalComments,
                        "Files": files,
                        "ShowGallery": scope.article.ShowGallery,
                        "ExpiryDateTime": convertCsharpDateToJavacript(scope.article.ExpiryDateTime)
                };
                    
                    putModel(type, model, scope.article.Id, scope);
                } else {
                    model = {
                        "Title": scope.article.Title,
                        "Body": scope.article.Body,
                        "Structure": scope.article.Structure,
                        "UserId": $rootScope.user.Id,
                        "Departments": articleDepartments,
                        "SubSectionTabId": scope.article.SubSectionTabId,
                        "Template": scope.article.Template,
                        "Terms": scope.article.Terms,
                        "Live": scope.article.Live,
                        "Approval": approval,
                        "ApprovalComments": approvalComments,
                        "Files": files,
                        "ShowGallery": scope.article.ShowGallery,
                        "ExpiryDateTime": convertCsharpDateToJavacript(scope.article.ExpiryDateTime)
                    };
                    postModel(type, model, scope);
                }
                break;
            case "Article": //updating a specific revision
                for (i in scope.article.images) {
                    if (scope.article.images.hasOwnProperty(i)) {
                        files.push({
                            "Id": scope.article.images[i].Id,
                            "Type": scope.article.images[i].Type,
                            "Host": scope.article.images[i].Host,
                            "Extension": scope.article.images[i].Extension,
                            "UploadedBy": scope.article.images[i].UploadedBy,
                            "OriginalFileName": scope.article.images[i].OriginalFileName,
                            "OriginalFileSize": scope.article.images[i].OriginalFileSize
                        });
                    }
                }

                for (i in scope.articleDepartmentOptions) {
                    if (scope.articleDepartmentOptions.hasOwnProperty(i)) {
                        articleDepartments.push({
                            "Id": scope.articleDepartmentOptions[i]
                        });
                    }
                }

                approval = (scope.article.Approval) ? scope.article.Approval : null;
                approvalComments = (scope.article.ApprovalComments) ? scope.article.ApprovalComments : null;

                model = {
                    "Id": scope.article.Id,
                    "_id": scope.article._id,
                    "RevisionId": scope.article.RevisionId,
                    "Title": scope.article.Title,
                    "Body": scope.article.Body,
                    "Structure": scope.article.Structure,
                    "UserId": $rootScope.user.Id,
                    "Departments": articleDepartments,
                    "SubSectionTabId": scope.article.SubSectionTabId,
                    "Template": scope.article.Template,
                    "Terms": scope.article.Terms,
                    "Live": scope.article.Live,
                    "Approval": approval,
                    "ApprovalComments": approvalComments,
                    "Files": files,
                    "ShowGallery": scope.article.ShowGallery,
                    "ExpiryDateTime": convertCsharpDateToJavacript(scope.article.ExpiryDateTime)
                };
                
                putModel(type, model, scope.article.Id, scope);
                break;
            case "ArticleReject":
                console.log(scope.article);
                approval = (scope.article.Approval) ? scope.article.Approval : null;
                approvalComments = (scope.article.ApprovalComments) ? scope.article.ApprovalComments : null;

                model = {
                    "Id": scope.article.Id,
                    "_id": scope.article._id,
                    "RevisionId": scope.article.RevisionId,
                    "Live": scope.article.Live,
                    "Approval": approval,
                    "ApprovalComments": approvalComments
                };

                putModel(type, model, scope.article.Id, scope);
                break;
            case "ArticleApprove":
                approval = (scope.article.Approval) ? scope.article.Approval : null;
                approvalComments = (scope.article.ApprovalComments) ? scope.article.ApprovalComments : null;

                model = {
                    "Id": scope.article.Id,
                    "_id": scope.article._id,
                    "RevisionId": scope.article.RevisionId,
                    "Live": scope.article.Live,
                    "Approval": approval,
                    "ApprovalComments": approvalComments
                };

                putModel(type, model, scope.article.Id, scope);
                break;
            case "ArticleTemplate":
                if (scope.articleTemplate.Id) {
                    model = {
                        "Id": scope.articleTemplate.Id,
                        "Title": scope.articleTemplate.Title,
                        "Structure": scope.articleTemplate.Structure,
                    };
                    putModel(type, model, scope.articleTemplate.Id, scope);
                } else {
                    model = {
                        "Title": scope.articleTemplate.Title,
                        "Structure": scope.articleTemplate.Structure,
                    };
                    postModel(type, model, scope);
                }
                break;
            case "SubSectionTab":
                if (scope.subSectionTab.Id) {
                    model = {
                        "Id": scope.subSectionTab.Id,
                        "_id": scope.subSectionTab._id,
                        "Name": scope.subSectionTab.Name,
                        "Icon": scope.subSectionTab.Icon,
                        "Live": scope.subSectionTab.Live
                    };
                    putModel(type, model, scope.subSectionTab.Id, scope);
                } else {
                    model = {
                        "Name": scope.subSectionTab.Name,
                        "Icon": scope.subSectionTab.Icon,
                        "Live": scope.subSectionTab.Live
                    };
                    postModel(type, model, scope);
                }
                break;
        }
    };

    return modelService;
}]);