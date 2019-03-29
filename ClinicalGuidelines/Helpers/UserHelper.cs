//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;
using System.Security.Principal;
using ClinicalGuidelines.DTOs;

namespace ClinicalGuidelines.Helpers
{
    public static class UserHelper
    {
        public static UserDto GetUserBySamAccountName(this IPrincipal user, string samAccountName)
        {
            var activeDirectoryHelper = new ActiveDirectoryHelper();
            var userDto = activeDirectoryHelper.FindUsersBySamAccountName(samAccountName);

            return userDto;
        }
        public static bool CheckUserGroupMembership(this IPrincipal user, List<string> groups)
        {
            var activeDirectoryHelper = new ActiveDirectoryHelper();
            return activeDirectoryHelper.IsMemberOfGroup(groups, user.GetUsernameWithoutDomain());
        }

        public static List<UserDto> GetUsersByName(string searchName)
        {
            var activeDirectoryHelper = new ActiveDirectoryHelper();
            var userDtos = activeDirectoryHelper.FindUsersByName(searchName);

            return userDtos;
        }

        public static string GetUsernameWithoutDomain(this IPrincipal user)
        {
            if (!user.Identity.Name.Contains("\\")) return user.Identity.Name;
            var split = user.Identity.Name.Split('\\');
            return split[split.Length - 1];
        }
    }
}