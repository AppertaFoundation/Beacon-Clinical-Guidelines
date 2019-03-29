//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("disclaimerModalCtrl", ["$scope", "$uibModalInstance", "departmentBackgroundColour", "departmentMainColour", function ($scope, $uibModalInstance, departmentBackgroundColour, departmentMainColour) {
    $scope.returnValue = null;
    $scope.departmentBackgroundColour = null;
    $scope.departmentMainColour = null;

    $scope.dismissModal = function () {
        $uibModalInstance.dismiss();
    };

    $scope.init = function () {
        $scope.departmentBackgroundColour = departmentBackgroundColour;
        console.log($scope.departmentBackgroundColour);
        $scope.departmentMainColour = departmentMainColour;
        console.log($scope.departmentMainColour);
    };

    $scope.init();
}]);