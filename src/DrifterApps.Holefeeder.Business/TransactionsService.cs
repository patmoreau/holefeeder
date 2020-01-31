using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;

namespace DrifterApps.Holefeeder.Business
{
    public class TransactionsService : BaseOwnedService<TransactionEntity>, ITransactionsService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IMapper _mapper;

        public TransactionsService(ITransactionsRepository transactionsRepository, IAccountsRepository accountsRepository, ICategoriesRepository categoriesReporitory, IMapper mapper) : base(transactionsRepository)
        {
            _transactionsRepository = transactionsRepository.ThrowIfNull(nameof(transactionsRepository));
            _accountsRepository = accountsRepository.ThrowIfNull(nameof(accountsRepository));
            _categoriesRepository = categoriesReporitory.ThrowIfNull(nameof(categoriesReporitory));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public async Task<long> CountAsync(string userId, QueryParams query) => await _transactionsRepository.CountAsync(userId, query);
        public async Task<IEnumerable<TransactionDetailEntity>> FindWithDetailsAsync(string userId, QueryParams query)
        {
            var transactions = await _transactionsRepository.FindAsync(userId, query);
            var accounts = (await _accountsRepository.FindAsync(userId, QueryParams.Empty)).Select(a => new AccountInfoEntity(a.Id, a.Name));
            var categories = (await _categoriesRepository.FindAsync(userId, QueryParams.Empty)).Select(c => new CategoryInfoEntity(c.Id, c.Name, c.Type, c.Color));

            return transactions
                .Join(accounts, t => t.Account, a => a.Id, (t, a) => new { t, AccountInfo = a })
                .Join(categories, x => x.t.Category, c => c.Id, (x, c) => new { x.t, x.AccountInfo, CategoryInfo = c })
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