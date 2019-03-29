//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("viewOnlyStaticPhoneNumbersModalCtrl", ["$scope", "$uibModalInstance", "contacts", "departmentBackgroundColour", "departmentMainColour", function ($scope, $uibModalInstance, contacts, departmentBackgroundColour, departmentMainColour) {
    $scope.returnValue = null;
    $scope.contacts = [];
    $scope.departmentBackgroundColour = null;
    $scope.departmentMainColour = null;

    $scope.dismissModal = function () {
        $uibModalInstance.dismiss();
    };

    $scope.init = function () {
        $scope.contacts = contacts;
        console.log($scope.contacts);
        $scope.departmentBackgroundColour = departmentBackgroundColour;
        console.log($scope.departmentBackgroundColour);
        $scope.departmentMainColour = departmentMainColour;
        console.log($scope.departmentMainColour);
    };

    $scope.init();
}]);