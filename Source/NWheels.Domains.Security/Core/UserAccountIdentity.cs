﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Hapil;
using NWheels.Extensions;
using NWheels.Authorization;
using NWheels.Authorization.Claims;

namespace NWheels.Domains.Security.Core
{
    public class UserAccountIdentity : ClaimsIdentity, IIdentityInfo
    {
        public static readonly string AuthenticationTypeString = "NWheels.Domains.Security";

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        private readonly IUserAccountEntity _userAccount;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public UserAccountIdentity(IUserAccountEntity userAccount, IEnumerable<Claim> claims)
            : base(claims.SelectMany(ExpandWithImpliedClaims))
        {
            _userAccount = userAccount;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public IUserAccountEntity GetUserAccount()
        {
            return _userAccount;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override bool IsAuthenticated
        {
            get { return true; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override string AuthenticationType
        {
            get { return AuthenticationTypeString; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override string Name
        {
            get { return _userAccount.LoginName; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        bool IIdentityInfo.IsOfType(Type accountEntityType)
        {
            return accountEntityType.IsAssignableFrom(_userAccount.GetType());
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        bool IIdentityInfo.IsInRole(string userRole)
        {
            return Claims.Any(c => c.Type == UserRoleClaim.UserRoleClaimTypeString && c.Value == userRole);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        string[] IIdentityInfo.GetUserRoles()
        {
            return Claims.Where(c => c.Type == UserRoleClaim.UserRoleClaimTypeString).Select(c => c.Value).ToArray();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        string IIdentityInfo.LoginName
        {
            get { return _userAccount.LoginName; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        string IIdentityInfo.QualifiedLoginName
        {
            get { return _userAccount.LoginName; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        string IIdentityInfo.PersonFullName
        {
            get { return _userAccount.FullName; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        string IIdentityInfo.EmailAddress
        {
            get
            {
                var frontEndAccount = _userAccount as IFrontEndUserAccountEntity;
                return (frontEndAccount != null ? frontEndAccount.EmailAddress : null);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static IEnumerable<Claim> ExpandWithImpliedClaims(Claim claim)
        {
            var implyMore = claim as IImplyMoreClaims;

            if ( implyMore != null )
            {
                var moreClaims = implyMore.GetImpliedClaims();
                return new[] { claim }.Concat(moreClaims.SelectMany(ExpandWithImpliedClaims));
            }
            else
            {
                return new[] { claim };
            }
        }
    }
}
