//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
Package.describe({
  name: "danialf:ng-file-upload",
  "version": "12.0.1",
  summary: "Lightweight Angular directive to upload files with optional FileAPI shim for cross browser support",
  git: "https://github.com/danialfarid/ng-file-upload.git"
});

Package.onUse(function (api) {
  api.use('angular:angular@1.2.0', 'client');
  api.addFiles('ng-file-upload-all.js', 'client');
});

