using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    public class ModifyObjectCommandHandler : IRequestHandler<ModifyObjectCommand, bool>
    {
        public Task<bool> Handle(ModifyObjectCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
