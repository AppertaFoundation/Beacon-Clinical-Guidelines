//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Configuration;
using System.Linq;
using System.Web;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.Helpers
{
    public static class GeneralHelper
    {
        private static readonly string ExternalTestIps = ConfigurationManager.AppSettings["ExternalIPTestAddresses"];
        public static bool IsRequestInternal(this HttpRequestBase source)
        {
            var clientIp = source.UserHostAddress;
            var iPAddress = clientIp.Split('.');
            var externalTestIpsArray = ExternalTestIps.Split(';');

            if (externalTestIpsArray.Any(s => s == clientIp))
            {
                return false;
            }

            return (clientIp == "::1" || clientIp == "127.0.0.1") || (iPAddress[0] == "10" && iPAddress[1] == "177");
        }

        public static string[] SplitStringByWhiteSpace(string initialValue)
        {
            return initialValue.Split(null);
        }

        public static string GetJobTitleFromDisplayName(string displayName)
        {
            var splitDisplayName = displayName.Split(',');

            return splitDisplayName.Length < 2 ? "" : splitDisplayName[1].Trim();
        }

        public static Server GetCurrentHostServer()
        {
            var host = HttpContext.Current.Request.Url.Host;
            var port = HttpContext.Current.Request.Url.Port;

            switch (host)
            {
                case "vm-app-webap01":
                case "beacon.plymouth.nhs.uk":
                    return Server.Live;
                case "vm-app-webap02":
                case "agile.plymouth.nhs.uk":
                case "10.177.51.141":
                    if (port == 448) return Server.Test;
                    if (port == 447) return Server.UAT;
                    break;
                case "localhost":
                    return Server.Debug;
            }

            return Server.NotRecognised;
        }
    }
}