//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("articleModalsCtrl", ["$scope", "$uibModalInstance", "type", "block", "images", function ($scope, $uibModalInstance, type, block, images) {
    $scope.returnValue = null;
    $scope.type = null;
    $scope.block = block;
    $scope.images = images;
    $scope.tableErrors = null;
    $scope.tableInputs = [];

    $scope.isLink = function() {
        return (type === "link") ? true : false;
    };

    $scope.isText = function () {
        return (type === "text") ? true : false;
    };

    $scope.isTable = function() {
        return (type === "table") ? true : false;
    };
    
    $scope.isImages = function() {
        return (type === "images") ? true : false;
    };
    
    $scope.dismissModal = function () {
        $uibModalInstance.dismiss();
    };

    $scope.returnValue = function (image) {
        if (image) {
            $uibModalInstance.close({ image: image.Id + image.Extension });
        } else if ($scope.type === "table") {
            $uibModalInstance.close({ table: $scope.tableInputs });
        } else {
            $uibModalInstance.close($scope.returnValue);
        }
    };

    $scope.returnBlock = function () {
        $scope.block.revisionEditedId = -1;
        console.log('returned block from modal: ', $scope.block);
        $uibModalInstance.close($scope.block);
    };

    $scope.blockType = function(blockType) {
        if (blockType !== "link") $scope.block.url = null;
        $scope.block.type = blockType;
    };

    $scope.createTable = function () {
        $scope.tableErrors = null;

        if ($scope.returnValue.numberColumns > 3) {
            $scope.tableErrors = "Tables should only be three columns in width maximum to work on small devices";
            return null;
        }

        if ($scope.returnValue.numberColumns <= 0 || $scope.returnValue.numberRows <= 0) {
            $scope.tableErrors = "Columns and rows should be more than 0";
            return null;
        }

        if (!$scope.returnValue.numberColumns || !$scope.returnValue.numberRows) {
            return null;
        }

        if (!$scope.tableInputs) $scope.tableInputs = [];
        for (var i = 0; i < $scope.returnValue.numberRows; i++) {
            if (!$scope.tableInputs[i]) $scope.tableInputs[i] = [];
            for (var j = 0; j < $scope.returnValue.numberColumns; j++) {
                var valueExists = ($scope.tableInputs[i] && $scope.tableInputs[i][j]) ? true : false;
                if (!valueExists) $scope.tableInputs[i].push({});
            }
        }

        //need to remove any elements that are over the specified columns/widths if they exist
        for (var k = 0; k < $scope.tableInputs.length; k++) {
            for (var l = 0; l < $scope.tableInputs[k].length; l++) {
                if (l > $scope.returnValue.numberColumns - 1) {
                    $scope.tableInputs[k].splice(l, $scope.tableInputs[k].length - 1);
                    break;
                }
            }
            if (k > $scope.returnValue.numberRows - 1) {
                $scope.tableInputs.splice(k, $scope.tableInputs.length - 1);
                break;
            }
        }

        console.log($scope.tableInputs);
    };

    $scope.init = function () {
        switch (type) {
            case "text":
                $scope.type = "text element";
                break;
            case "unordered":
                $scope.type = "unordered list item";
                break;
            case "ordered":
                $scope.type = "ordered list item";
                break;
            case "link":
                $scope.type = "link";
                break;
            case "table":
                $scope.type = "table";
                $scope.tableInputs = ($scope.block && $scope.block.structure) ? $scope.block.structure : [];
                break;
            case "images":
                $scope.type = "images";
                break;
        }
        console.log($scope.type);
    };

    $scope.init();
}]);