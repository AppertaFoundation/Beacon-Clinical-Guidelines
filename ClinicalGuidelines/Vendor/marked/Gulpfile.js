//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
var gulp = require('gulp');
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');

var preserveFirstComment = function() {
  var set = false;

  return function() {
     if (set) return false;
     set = true;
     return true;
  };
};

gulp.task('uglify', function() {
  gulp.src('lib/marked.js')
    .pipe(uglify({preserveComments: preserveFirstComment()}))
    .pipe(concat('marked.min.js'))
    .pipe(gulp.dest('.'));
});

gulp.task('default', ['uglify']);
