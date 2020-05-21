using System.Collections.Generic;

namespace MvcMovie.Models
{
    public class UsersViewModel
    {
        public ICollection<UserViewModel> Users { get; set; }
    }
}
