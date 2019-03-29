//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Web.Optimization;
using ClinicalGuidelines.Extensions;

namespace ClinicalGuidelines
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css")
                .Include(
                    "~/Vendor/bootstrap/dist/css/bootstrap.min.css",
                    "~/Vendor/angular-loading-bar/build/loading-bar.min.css",
                    "~/Vendor/font-awesome/css/font-awesome.min.css",
                    "~/Vendor/md-color-menu/md-color-menu.css",
                    "~/Vendor/angular-material/angular-material.css",
                    "~/Vendor/angular-ui-tree/dist/angular-ui-tree.min.css",
                    "~/Vendor/bootstrap-iconpicker/icon-fonts/ionicons-2.0.1/css/ionicons.min.css",
                    "~/Vendor/bootstrap-iconpicker/bootstrap-iconpicker/css/bootstrap-iconpicker.min.css",
                    "~/CSS/site.css"
                ).WithLastVersionNumber()
            );

            bundles.Add(new ScriptBundle("~/bundles/angular")
                .Include(
                        "~/Vendor/angular/angular.min.js",
                        "~/Vendor/angular-ui-router/release/angular-ui-router.min.js",
                        "~/Vendor/angular-sanitize/angular-sanitize.min.js",
                        "~/Vendor/angular-aria/angular-aria.min.js",
                        "~/Vendor/angular-animate/angular-animate.min.js",
                        "~/Vendor/angular-material/angular-material.min.js",
                        "~/Vendor/angular-ui-tree/dist/angular-ui-tree.js",
                        "~/Vendor/ng-file-upload/ng-file-upload.min.js",
                        "~/Vendor/angular-filter/dist/angular-filter.min.js"
                ).WithLastVersionNumber()
            );

            bundles.Add(new ScriptBundle("~/bundles/tools")
                .Include(
                      "~/Vendor/jquery/dist/jquery.js",
                      "~/Vendor/bootstrap/dist/js/bootstrap.min.js",
                      "~/Vendor/angular-bootstrap/ui-bootstrap-tpls.min.js",
                      "~/Vendor/angular-loading-bar/build/loading-bar.min.js",
                      "~/Vendor/bootstrap-iconpicker/bootstrap-iconpicker/js/iconset/iconset-ionicon-2.0.1.min.js",
                      "~/Vendor/bootstrap-iconpicker/bootstrap-iconpicker/js/bootstrap-iconpicker.min.js",
                      "~/Vendor/marked/marked.min.js",
                      "~/Vendor/moment/moment.min.js"
                ).WithLastVersionNumber()
            );

            bundles.Add(new ScriptBundle("~/bundles/app")
                .Include(
                      "~/App/app.js",
                      "~/App/Factories/httpFactory.js",
                      "~/App/Factories/fileFactory.js",
                      "~/App/Factories/modelFactory.js",
                      "~/App/Filters/appFilters.js",
                      "~/App/Directives/appDirectives.js",
                      "~/App/Directives/mdColorMenu.js",
                      "~/App/Controllers/appCtrl.js",
                      "~/App/Controllers/modalsCtrl.js",
                      "~/App/Controllers/articleModalsCtrl.js",
                      "~/App/Controllers/articlesCtrl.js",
                      "~/App/Controllers/articleTemplatesCtrl.js",
                      "~/App/Controllers/subSectionTabsCtrl.js",
                      "~/App/Controllers/usersCtrl.js",
                      "~/App/Controllers/webCtrl.js",
                      "~/App/Controllers/departmentsCtrl.js",
                      "~/App/Controllers/viewOnlyStaticPhoneNumbersModalCtrl.js",
                      "~/App/Controllers/emailCopyDetailsModalCtrl.js",
                      "~/App/Controllers/disclaimerModalCtrl.js"
                ).WithLastVersionNumber()
            );
        }
    }
}
