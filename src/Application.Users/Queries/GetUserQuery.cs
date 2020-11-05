using DrifterApps.Holefeeder.Application.Users.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Users.Queries
{
    public class GetUserQuery : IRequest<UserViewModel>
    {
        public string Email { get; }

        public GetUserQuery(string email)
        {
            Email = email;
        }
    }
}
