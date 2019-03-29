//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.factory("fileService", ["$http", "$rootScope", "Upload", function ($http, $rootScope, Upload) {
    var fileService = {};

    var thumbnailWidth = 250;
    var getDownloadUrl = function (image, size) {
        return $rootScope.httpServiceLocation + "/api/file/download/" + image + "/thumb/height/" + size;
    }

    var getDownloadUrlWidth = function (image, size) {
        return $rootScope.httpServiceLocation + "/api/file/download/" + image + "/thumb/width/" + size;
    }

    var getFullDownloadUrl = function (image) {
        return $rootScope.httpServiceLocation + "/api/file/download/" + image + "/";
    }

    fileService.downloadImage = function (image, size) {
        if (size === 0) size = thumbnailWidth;
        return $http.get(getDownloadUrl(image, size));
    };

    fileService.downloadImageWidth = function (image, size) {
        if (size === 0) size = thumbnailWidth;
        return $http.get(getDownloadUrlWidth(image, size));
    };

    fileService.downloadImageWithIndex = function (image, size, arrayIndex) {
        if (size === 0) size = thumbnailWidth;
        return $http.get(getDownloadUrl(image, size));
    };

    fileService.downloadFullImage = function(image) {
        return $http.get(getFullDownloadUrl(image));
    };

    fileService.uploadImage = function (file, article, size) {
        if (size === 0) size = thumbnailWidth;
        Upload
            .upload(
                {
                    url: $rootScope.httpServiceLocation + "/api/file/upload/",
                    data: {
                        file: file
                    }
                }
            )
            .then(
                function (resp) {
                    var fileName = resp.config.data.file.name;
                    var downloadUrl = getDownloadUrl(resp.data.Id + resp.data.Extension, size);
                    article.images[fileName] = resp.data;
                    var callback = function (response) {
                        console.log(response);
                        article.images[fileName].image = response.data;
                        article.images[fileName].PreviewId = Object.keys(article.images).length - 1;
                    }
                    $http.get(downloadUrl).then(callback);
                },
                function (error) {
                    delete article.images[error.config.data.file.name];
                    article.uploadError = "Error uploading file (" + error.config.data.file.name + "): " + error.data.Message;
                },
                function (evt) {
                    article.images[evt.config.data.file.name] = {
                        "Percentage": parseInt(100.0 * evt.loaded / evt.total)
                    };
                }
            );
    };

    return fileService;
}]);