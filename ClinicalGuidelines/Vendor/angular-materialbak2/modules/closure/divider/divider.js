//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
/*!
 * AngularJS Material Design
 * https://github.com/angular/material
 * @license MIT
 * v1.1.6
 */
goog.provide('ngmaterial.components.divider');
goog.require('ngmaterial.core');
/**
 * @ngdoc module
 * @name material.components.divider
 * @description Divider module!
 */
MdDividerDirective['$inject'] = ["$mdTheming"];
angular.module('material.components.divider', [
  'material.core'
])
  .directive('mdDivider', MdDividerDirective);

/**
 * @ngdoc directive
 * @name mdDivider
 * @module material.components.divider
 * @restrict E
 *
 * @description
 * Dividers group and separate content within lists and page layouts using strong visual and spatial distinctions. This divider is a thin rule, lightweight enough to not distract the user from content.
 *
 * @param {boolean=} md-inset Add this attribute to activate the inset divider style.
 * @usage
 * <hljs lang="html">
 * <md-divider></md-divider>
 *
 * <md-divider md-inset></md-divider>
 * </hljs>
 *
 */
function MdDividerDirective($mdTheming) {
  return {
    restrict: 'E',
    link: $mdTheming
  };
}

ngmaterial.components.divider = angular.module("material.components.divider");