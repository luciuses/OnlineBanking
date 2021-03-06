﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberShipExtension.cs" company="">
//   
// </copyright>
// <summary>
//   The member ship extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure
{
    using System.Web.Security;

    /// <summary>
    /// The member ship extension.
    /// </summary>
    public static class MemberShipExtension
    {
        /// <summary>
        /// The error code to string.
        /// </summary>
        /// <param name="createStatus">
        /// The create status.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ErrorCodeToString(this MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different username.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "The user name for this email address already exists. Enter a different email address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Invalid password. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Invalid email address. Verify the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "An invalid response to a question for password recovery. Verify the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "An invalid question for password recovery. Verify the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Invalid user name. Verify the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Check the value entered and try again. If the problem persists, contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "Create a user request has been canceled. Check the value entered and try again. If the problem persists, contact your system administrator. ";
                case MembershipCreateStatus.Success:
                    return "Account created successfully";

                default:
                    return
                        "An unknown error occurred. Check the value entered and try again. If the problem persists, contact your system administrator.";
            }
        }
    }
}