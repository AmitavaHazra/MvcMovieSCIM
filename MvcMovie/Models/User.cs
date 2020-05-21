using Microsoft.SCIM;
using System.Diagnostics.CodeAnalysis;

namespace MvcMovie.Models
{
    /// <summary>
    /// Internal application user representation
    /// </summary>
    public class User
    {
        /// <summary>
        /// Primary key for user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// SCIM external Identifier
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// User's display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Azure AD sends user's UPN here
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Whether the Azure AD user is Active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Conversion from our <see cref="Microsoft.SCIM.User"/> model
        /// to SCIM data contract type.
        /// </summary>
        public static explicit operator Core2EnterpriseUser([NotNull] User user)
        {
            return new Core2EnterpriseUser
            {
                Identifier = user.Id.ToString(),
                ExternalIdentifier = user.ExternalId,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Active = user.Active
            };
        }

        /// <summary>
        /// Conversion from SCIM data contract type to our
        /// <see cref="Microsoft.SCIM.User"/> type.
        /// </summary>
        public static explicit operator User([NotNull] Core2EnterpriseUser user)
        {
            return new User
            {
                Id = int.TryParse(user.Identifier, out int id) ? id : default,
                ExternalId = user.ExternalIdentifier,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Active = user.Active
            };
        }
    }
}
