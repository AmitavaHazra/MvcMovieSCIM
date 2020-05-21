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
    }
}
