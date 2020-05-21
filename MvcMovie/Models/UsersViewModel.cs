using System.Collections.Generic;

namespace MvcMovie.Models
{
    internal class UsersViewModel
    {
        public ICollection<UserViewModel> Users { get; set; }
    }
}
