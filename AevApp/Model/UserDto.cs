using System;

namespace AevApp.Model
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime Created { get; set; }

        public string AirportPermission { get; set; }

        public string GlobalPermission { get; set; }
    }
}
