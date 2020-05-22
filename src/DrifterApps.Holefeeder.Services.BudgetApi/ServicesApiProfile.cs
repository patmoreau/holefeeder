using System;
using AutoMapper;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Services.BudgetDto;

namespace DrifterApps.Holefeeder.Services.BudgetApi
{
    public class ServicesApiProfile : Profile
    {
        public ServicesApiProfile()
        {
            CreateMap<AccountEntity, AccountDto>().ReverseMap();
            CreateMap<CashflowEntity, CashflowDto>().ReverseMap();
            CreateMap<CashflowInfoEntity, CashflowInfoDto>();
            CreateMap<CategoryEntity, CategoryInfoEntity>();
            CreateMap<CategoryEntity, CategoryDto>().ReverseMap();
            CreateMap<CategoryInfoEntity, CategoryInfoDto>();
            CreateMap<ObjectDataEntity, ObjectDataDto>().ReverseMap();
            CreateMap<UserEntity, UserDto>().ReverseMap();
            CreateMap<UpcomingEntity, UpcomingDto>().ReverseMap();
            CreateMap<TransactionEntity, TransactionDto>().ReverseMap();
            CreateMap<TransactionDetailEntity, TransactionDetailDto>();
            CreateMap<AccountDetailEntity, AccountDetailDto>();
            CreateMap<AccountInfoEntity, AccountInfoDto>();
            CreateMap(typeof(StatisticsEntity<>), typeof(StatisticsDto<>));
            CreateMap(typeof(SeriesEntity), typeof(SeriesDto));
        }
    }
}
