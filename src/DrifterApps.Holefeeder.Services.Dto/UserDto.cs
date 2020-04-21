using System;

namespace DrifterApps.Holefeeder.Services.Dto
{
    public class UserDto
    {
        public string Id { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string EmailAddress { get; }

        public string GoogleId { get; }

        public DateTime DateJoined { get; }

        public UserDto(string id, string firstName, string lastName, string emailAddress, string googleId, DateTime dateJoined)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            GoogleId = googleId;
            DateJoined = dateJoined;
        }
    }
}
