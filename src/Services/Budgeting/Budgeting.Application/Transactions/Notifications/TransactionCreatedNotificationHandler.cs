using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Notifications;

public class TransactionCreatedNotificationHandler : INotificationHandler<TransactionCreatedNotification>
{
    public Task Handle(TransactionCreatedNotification notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
