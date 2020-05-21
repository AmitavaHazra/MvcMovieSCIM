using System.Diagnostics.CodeAnalysis;

namespace MvcMovie.Models
{
    public class UserViewModel
    {
        public string DisplayName { get; set; }

        public string UserName { get; set; }

        public bool Active { get; set; }

        public static explicit operator UserViewModel([NotNull] User user)
        {
            return new UserViewModel
            {
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Active = user.Active
            };
        }
    }
}
