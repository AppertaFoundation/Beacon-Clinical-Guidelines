//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
var BASIC_EXAMPLE_URL = 'http://localhost:9000/#/basic-example';

var BasicExamplePageNode = function (nodeLocation) {
  var subnodesXpath = 'ol/li[contains(@class,"angular-ui-tree-node")]';
  var nodeHandlesLocator = by.css('[ui-tree-handle]');

  var nodeElement = element(by.xpath(xpathStringForNodeAtPosition(nodeLocation)));
  var handle = nodeElement.all(nodeHandlesLocator).first();

  this.getElement = function() { return nodeElement; }
  this.getHandle = function() { return handle; };
  this.getText = function () {
    return handle.getText();
  };
  this.getSubnodes = function() {
    return nodeElement.all(by.xpath(subnodesXpath));
  }

  function xpathStringForNodeAtPosition(nodeLocation) {
    var xpathChunks = ['//*[@id="tree-root"]'];
    nodeLocation.forEach(function(index) {
      xpathChunks.push(subnodesXpath + '[' + index + ']')
    });
    return xpathChunks.join('/');
  }
}

var BasicExamplePage = function () {
  this.get = function () {
    browser.get(BASIC_EXAMPLE_URL);
  };

  this.getRootNodes = function() {
    return element.all(by.repeater('node in data'));
  };

  this.getNodeAtPosition = function () {
    return new BasicExamplePageNode([].slice.call(arguments));
  };
};

module.exports = new BasicExamplePage();
