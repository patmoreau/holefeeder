using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUserHandler : IRequestHandler<GetUserRequest, UserViewModel>
    {
        private readonly IUserQueriesRepository _repository;

        public GetUserHandler(IUserQueriesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public async Task<UserViewModel> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetUserByEmailAsync(request.Email, cancellationToken);
        }
    }
}
