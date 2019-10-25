//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// See LICENSE in the project root for license information.
﻿/**
 * This was originally code from git hub however the controller didn't do everything I needed it to the original is here
 * https://github.com/ONE-LOGIC/md-color-menu
 *
 * Changes made
 *
 * 1. used a newer version of angular that needed a bug fix below showTooltips: '=?' added ? to make optional
 * 2. changed directive colour swatch template to show a remove colour option
 * 3. changed directive template to accept a callback to update the parent controller when a colour is selected or removed
 *       needed this as i am looking for selected options to prove the form is completed
**/
(function () {
    angular
      .module('mdColorMenu', ['ngAria', 'ngAnimate', 'ngMaterial'])
      .factory('mdPickerColors', ['$mdColorPalette', mdPickerColors])
      .directive('mdColorMenu', mdColorMenuDirective);

    function mdPickerColors($mdColorPalette) {
        var service = {
            colors: [],
            getColor: getColor
        };

        var hexToColor = {};

        angular.forEach($mdColorPalette, function (swatch, swatchName) {
            var swatchColors = [];
            angular.forEach(swatch, function (color, colorName) {
                if (isAccentColors(colorName) || isBlack(colorName))
                    return;
                var foregroundColor = listToRgbString(color.contrast);
                var backgroundColor = listToRgbString(color.value);
                var name = swatchName + ' ' + colorName;
                var hex = listToHexString(color.value);
                var colorObject = { name: name, hex: hex, style: { 'color': foregroundColor, 'background-color': backgroundColor } };
                swatchColors.push(colorObject);
                hexToColor[hex] = colorObject;
            });
            service.colors.push(swatchColors);
        });

        return service;

        function getColor(hex) {
            return hexToColor[hex.toLowerCase()];
        }

        function isAccentColors(colorName) {
            return colorName[0] === 'A';
        }

        function isBlack(colorName) {
            return colorName === '1000';
        }

        function listToRgbString(rgbList) {
            if (rgbList.length === 4) {
                return 'rgba(' + rgbList.join(',') + ')';
            }
            return 'rgb(' + rgbList.join(',') + ')';
        }

        function listToHexString(rgbList) {
            return '#' + rgbList.map(toHex).join('');
        }

        function toHex(number) {
            hex = number.toString(16);
            if (hex.length < 2) {
                hex = '0' + hex;
            }
            return hex;
        }
    }

    function mdColorMenuDirective() {
        return {
            restrict: 'E',
            transclude: true,
            scope: {
                onSuccessCallback: '&'
            },
            controller: mdColorMenuController,
            controllerAs: 'vm',
            bindToController: {
                color: '=',
                showTooltips: '=?'
            },
            template: [
              '<md-menu md-position-mode="target-right target">',
              '  <div ng-click="vm.openMenu($mdOpenMenu, $event)" ng-transclude=""></div>',
              '  <md-menu-content class="md-cm">',
              '    <div><span ng-if="vm.color" ng-click="vm.removeColor();onSuccessCallback();">&#10008; Remove Current Selection</span></div>',
              '    <div class="md-cm-swatches" layout="row">',
              '      <div ng-repeat="swatch in vm.colors" layout=column>',
              '        <div ng-repeat="color in swatch" class="md-cm-color" ng-style="color.style" ng-click="vm.selectColor(color);onSuccessCallback();" layout="row" layout-align="center center">',
              '          <span ng-if="color.name == vm.color.name">&#10004;</span>',
              '          <md-tooltip ng-if="vm.showTooltips">{{color.name}}</md-tooltip>',
              '        </div>',
              '      </div>',
              '    </div>',
              '  </md-menu-content>',
              '</md-menu>'
            ].join('')
        }
    }

    function mdColorMenuController(mdPickerColors) {
        var vm = this;

        vm.showTooltips = vm.showTooltips || false;
        vm.openMenu = openMenu;
        vm.colors = mdPickerColors.colors;
        vm.selectColor = selectColor;
        vm.removeColor = removeColor;

        function openMenu($mdOpenMenu, $event) {
            $mdOpenMenu($event);
        }

        function selectColor(color) {
            vm.color = color;
        }

        function removeColor() {
            vm.color = null;
        }
    }

})();
