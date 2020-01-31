using System;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class UserEntity : BaseEntity, IIdentityEntity<UserEntity>
    {
        public string FirstName { get; }

        public string LastName { get; }

        public string EmailAddress { get; }

        public string GoogleId { get; }

        public DateTime DateJoined { get; }

        public UserEntity(string id, string firstName, string lastName, string emailAddress, string googleId, DateTime dateJoined) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            GoogleId = googleId;
            DateJoined = dateJoined;
        }

        public UserEntity With(string id = null, string firstName = null, string lastName = null, string emailAddress = null, string googleId = null, DateTime? dateJoined = null) =>
            new UserEntity(
                id ?? Id,
                firstName ?? FirstName,
                lastName ?? LastName,
                emailAddress ?? EmailAddress,
                googleId ?? GoogleId,
                dateJoined ?? DateJoined);

        public UserEntity WithId(string id) => this.With(id: id ?? Id);
    }
}
