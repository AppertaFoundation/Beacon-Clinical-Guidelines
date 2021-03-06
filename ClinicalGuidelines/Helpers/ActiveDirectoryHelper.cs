//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using ClinicalGuidelines.DTOs;

namespace ClinicalGuidelines.Helpers
{
    public class ActiveDirectoryHelper
    {
        private readonly List<UserDto> _userDtos = new List<UserDto>();
        private const string Domain = "DERRIFORD";

        public bool IsMemberOfGroup(List<string> groups, string samAccountName)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain, Domain))
            {
                using (var userPrincipal = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, samAccountName))
                {
                    if (userPrincipal == null) return false;
                    foreach (var group in groups)
                    {
                        using (var groupPrinicipal = GroupPrincipal.FindByIdentity(principalContext, @group))
                        {
                            if (groupPrinicipal == null) continue;
                            if (userPrincipal.IsMemberOf(groupPrinicipal))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public UserDto FindUsersBySamAccountName(string samAccountName)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain, Domain))
            {
                using (var userPrincipal = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, samAccountName))
                {
                    if (userPrincipal != null)
                    {
                        return new UserDto()
                        {
                            SamAccountName = samAccountName,
                            DisplayName = userPrincipal.DisplayName,
                            JobTitle = GeneralHelper.GetJobTitleFromDisplayName(userPrincipal.DisplayName),
                            Forename = userPrincipal.GivenName,
                            Surname = userPrincipal.Surname,
                            EmailAddress = userPrincipal.EmailAddress
                        };
                    }
                    else
                    {
                        return new UserDto();
                    }
                }
            }
        }

        public List<UserDto> FindUsersByName(string searchString)
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain, Domain))
            {
                SetSearchObject(principalContext, searchString);
            }

            return _userDtos;
        }

        private void SetSearchObject(PrincipalContext principalContext, string searchString)
        {
            string[] searchArray = GeneralHelper.SplitStringByWhiteSpace(searchString);
            using (var searchUser = new UserPrincipal(principalContext))
            {
                searchUser.GivenName = searchArray[0] + "*";
                if (searchArray.Length == 2)
                {
                    searchUser.Surname = searchArray[1] + "*";
                }

                SearchUsingUserPrincipal(principalContext, searchUser);
            }
        }

        private void SearchUsingUserPrincipal(PrincipalContext principalContext, Principal searchUser)
        {
            using (var userSearcher = new PrincipalSearcher())
            {
                userSearcher.QueryFilter = searchUser;
                using (var searchResults = userSearcher.FindAll())
                {
                    ProcessSearchResults(principalContext, searchResults);
                }
            }
        }

        private void ProcessSearchResults(PrincipalContext principalContext, IEnumerable<Principal> searchResults)
        {
            foreach (var searchResult in searchResults)
            {
                using (var userAccount = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, searchResult.SamAccountName))
                {
                    if (userAccount == null)
                    {
                        continue;
                    }

                    _userDtos.Add(new UserDto()
                    {
                        SamAccountName = searchResult.SamAccountName,
                        DisplayName = userAccount.DisplayName,
                        JobTitle = GeneralHelper.GetJobTitleFromDisplayName(userAccount.DisplayName),
                        Forename = userAccount.GivenName,
                        Surname = userAccount.Surname,
                        EmailAddress = userAccount.EmailAddress
                    });
                }
            }
        }
    }
}