using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, UserViewModel>
    {
        private readonly IUserQueriesRepository _repository;

        public GetUserHandler(IUserQueriesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<UserViewModel> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            return await _repository.GetUserByEmailAsync(query.Email, cancellationToken);
        }
    }
}
