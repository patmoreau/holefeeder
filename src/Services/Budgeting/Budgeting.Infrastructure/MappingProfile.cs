using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using AutoMapper;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext;

using AccountContext = DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using TransactionContext = DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;

using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<string, ImmutableArray<string>>()
                .ConstructUsing(x =>
                    x.Split(',', StringSplitOptions.TrimEntries).Where(y => !string.IsNullOrWhiteSpace(y))
                        .ToImmutableArray());
            CreateMap<IReadOnlyList<string>, string>()
                .ConstructUsing(x => string.Join(',', x));
            CreateMap<AccountEntity, AccountInfoViewModel>();
            CreateMap<AccountEntity, AccountViewModel>()
                .ForMember(d => d.Balance, opts => opts.Ignore())
                .ForMember(d => d.TransactionCount, opts => opts.Ignore())
                .ForMember(d => d.Updated, opts => opts.Ignore());
            CreateMap<AccountEntity, AccountContext.Account>()
                .ForMember(d => d.Cashflows, opts => opts.Ignore())
                .ReverseMap();
            CreateMap<CategoryEntity, Category>().ReverseMap();
            CreateMap<CashflowEntity, TransactionContext.Cashflow>().ReverseMap();
            CreateMap<CashflowEntity, CashflowViewModel>();
            CreateMap<CategoryEntity, CategoryViewModel>();
            CreateMap<CategoryEntity, CategoryInfoViewModel>();
            CreateMap<TransactionEntity, TransactionViewModel>();
            CreateMap<TransactionEntity, TransactionContext.Transaction>()
                .ReverseMap();
        }
    }
}
