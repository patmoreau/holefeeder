using AutoMapper;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;
using Transaction = DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext.Transaction;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure
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
