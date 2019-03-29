//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
/*!
 * Angular Material Design
 * https://github.com/angular/material
 * @license MIT
 * v1.0.6
 */
goog.provide('ng.material.components.swipe');
goog.require('ng.material.core');
/**
 * @ngdoc module
 * @name material.components.swipe
 * @description Swipe module!
 */
/**
 * @ngdoc directive
 * @module material.components.swipe
 * @name mdSwipeLeft
 *
 * @restrict A
 *
 * @description
 * The md-swipe-left directive allows you to specify custom behavior when an element is swiped
 * left.
 *
 * @usage
 * <hljs lang="html">
 * <div md-swipe-left="onSwipeLeft()">Swipe me left!</div>
 * </hljs>
 */
/**
 * @ngdoc directive
 * @module material.components.swipe
 * @name mdSwipeRight
 *
 * @restrict A
 *
 * @description
 * The md-swipe-right directive allows you to specify custom behavior when an element is swiped
 * right.
 *
 * @usage
 * <hljs lang="html">
 * <div md-swipe-right="onSwipeRight()">Swipe me right!</div>
 * </hljs>
 */
/**
 * @ngdoc directive
 * @module material.components.swipe
 * @name mdSwipeUp
 *
 * @restrict A
 *
 * @description
 * The md-swipe-up directive allows you to specify custom behavior when an element is swiped
 * up.
 *
 * @usage
 * <hljs lang="html">
 * <div md-swipe-up="onSwipeUp()">Swipe me up!</div>
 * </hljs>
 */
/**
 * @ngdoc directive
 * @module material.components.swipe
 * @name mdSwipeDown
 *
 * @restrict A
 *
 * @description
 * The md-swipe-down directive allows you to specify custom behavior when an element is swiped
 * down.
 *
 * @usage
 * <hljs lang="html">
 * <div md-swipe-down="onSwipDown()">Swipe me down!</div>
 * </hljs>
 */

angular.module('material.components.swipe', ['material.core'])
    .directive('mdSwipeLeft', getDirective('SwipeLeft'))
    .directive('mdSwipeRight', getDirective('SwipeRight'))
    .directive('mdSwipeUp', getDirective('SwipeUp'))
    .directive('mdSwipeDown', getDirective('SwipeDown'));

function getDirective(name) {
  var directiveName = 'md' + name;
  var eventName = '$md.' + name.toLowerCase();

    DirectiveFactory.$inject = ["$parse"];
  return DirectiveFactory;

  /* ngInject */
  function DirectiveFactory($parse) {
      return { restrict: 'A', link: postLink };
      function postLink(scope, element, attr) {
        var fn = $parse(attr[directiveName]);
        element.on(eventName, function(ev) {
          scope.$apply(function() { fn(scope, { $event: ev }); });
        });
      }
    }
}



ng.material.components.swipe = angular.module("material.components.swipe");