namespace Holefeeder.Application.Features.Transactions.Notifications;

public class TransactionCreatedNotificationHandler : INotificationHandler<TransactionCreatedNotification>
{
    public Task Handle(TransactionCreatedNotification notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
