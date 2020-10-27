using System;
using AutoMapper;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.Transactions.Models;
using DrifterApps.Holefeeder.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Infrastructure.Database.Schemas;
using Transaction = DrifterApps.Holefeeder.Domain.BoundedContext.TransactionContext.Transaction;

namespace DrifterApps.Holefeeder.Infrastructure.Database
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountSchema, Account>()
                .ReverseMap()
                .ForMember(schema => schema.MongoId, opts => opts.Ignore());

            CreateMap<TransactionSchema, Transaction>()
                .ReverseMap()
                .ForMember(schema => schema.MongoId, opts => opts.Ignore());

            CreateMap<CategorySchema, CategoryViewModel>();
        }
    }
}
