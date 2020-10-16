using DrifterApps.Holefeeder.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
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
