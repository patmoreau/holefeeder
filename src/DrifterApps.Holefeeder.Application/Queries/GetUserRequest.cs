using DrifterApps.Holefeeder.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUserRequest : IRequest<UserViewModel>
    {
        public string Email { get; }

        public GetUserRequest(string email)
        {
            Email = email;
        }
    }
}
