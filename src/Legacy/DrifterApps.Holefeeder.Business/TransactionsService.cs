using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;
using System.Threading;

namespace DrifterApps.Holefeeder.Business
{
    public class TransactionsService : BaseOwnedService<TransactionEntity>, ITransactionsService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        public TransactionsService(ITransactionsRepository transactionsRepository, IAccountsRepository accountsRepository, ICategoriesRepository categoriesRepository) : base(transactionsRepository)
        {
            _transactionsRepository = transactionsRepository.ThrowIfNull(nameof(transactionsRepository));
            _accountsRepository = accountsRepository.ThrowIfNull(nameof(accountsRepository));
            _categoriesRepository = categoriesRepository.ThrowIfNull(nameof(categoriesRepository));
        }

        public Task<int> CountAsync(Guid userId, QueryParams query, CancellationToken cancellationToken = default) => _transactionsRepository.CountAsync(userId, query, cancellationToken);
        public async Task<IEnumerable<TransactionDetailEntity>> FindWithDetailsAsync(Guid userId, QueryParams query, CancellationToken cancellationToken = default)
        {
            var transactions = await _transactionsRepository.FindAsync(userId, query, cancellationToken).ConfigureAwait(false);
            var accounts = (await _accountsRepository.FindAsync(userId, QueryParams.Empty, cancellationToken).ConfigureAwait(false)).Select(a => new AccountInfoEntity(a.Id, a.Name));
            var categories = (await _categoriesRepository.FindAsync(userId, QueryParams.Empty, cancellationToken).ConfigureAwait(false)).Select(c => new CategoryInfoEntity(c.Id, c.Name, c.Type, c.Color));

            return transactions
                .Join(accounts, t => t.Account, a => a.Id, (t, a) => (t, AccountInfo: a))
                .Join(categories, x => x.t.Category, c => c.Id, (x, c) => (x.t, x.AccountInfo, CategoryInfo: c))
                .Select(x =>
                    new TransactionDetailEntity(
                        x.t.Id,
                        x.t.Date,
                        x.t.Amount,
                        x.t.Description,
                        x.CategoryInfo,
                        x.AccountInfo,
                        new CashflowInfoEntity(x.t.Cashflow, x.t.CashflowDate),
                        x.t.Tags));
        }
    }
}
