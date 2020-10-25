using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, UserViewModel>
    {
        private readonly IUserQueriesRepository _repository;

        public GetUserHandler(IUserQueriesRepository repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public async Task<UserViewModel> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            return await _repository.GetUserByEmailAsync(query.Email, cancellationToken);
        }
    }
}
