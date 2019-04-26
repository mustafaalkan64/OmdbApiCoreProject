using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Models
{
    public class UserDto: UserLoginDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class UserLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
