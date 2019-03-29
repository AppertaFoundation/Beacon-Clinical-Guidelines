//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿app.controller("emailCopyDetailsModalCtrl", ["$scope", "$uibModalInstance", "emailTitle", "emailBody", "emailLink", "departmentBackgroundColour", "departmentMainColour", function ($scope, $uibModalInstance, emailTitle, emailBody, emailLink, departmentBackgroundColour, departmentMainColour) {
    $scope.returnValue = null;
    $scope.emailTitle = null;
    $scope.emailBody = null;
    $scope.emailLink = null;
    $scope.departmentBackgroundColour = null;
    $scope.departmentMainColour = null;

    $scope.dismissModal = function () {
        $uibModalInstance.dismiss();
    };

    $scope.copyToClipboard = function (text) {
        console.log('name: ', name);
        var copyElement = document.createElement("textarea");
        copyElement.style.position = 'fixed';
        copyElement.style.opacity = '0';

        copyElement.textContent = text;

        console.log('copyElement.textContent: ', copyElement.textContent);

        var body = document.getElementsByTagName('body')[0];

        body.appendChild(copyElement);

        copyElement.select();
        document.execCommand('copy');

        body.removeChild(copyElement);
    }

    $scope.init = function () {
        $scope.emailTitle = emailTitle;
        console.log('email title: ', $scope.emailTitle);

        $scope.emailBody = emailBody;
        console.log('email body: ', $scope.emailBody);

        $scope.emailLink = emailLink;
        console.log('email link: ', $scope.emailLink);

        $scope.departmentBackgroundColour = departmentBackgroundColour;
        console.log($scope.departmentBackgroundColour);
        $scope.departmentMainColour = departmentMainColour;
        console.log($scope.departmentMainColour);
    };

    $scope.init();
}]);