using System;

namespace DrifterApps.Holefeeder.Application.Models
{
    public class UserViewModel
    {
        public Guid Id { get; }
        
        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }

        public string GoogleId { get; }

        public DateTime DateJoined { get; }

        public UserViewModel(Guid id, string firstName, string lastName, string email, string googleId, DateTime dateJoined)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            GoogleId = googleId;
            DateJoined = dateJoined;
        }
    }
}
