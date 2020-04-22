using AutoMapper;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Services.Dto;
using DrifterApps.Holefeeder.Services.DTO;

namespace DrifterApps.Holefeeder.Services.Api
{
    public class ServicesApiProfile : Profile
    {
        public ServicesApiProfile()
        {
            CreateMap<AccountEntity, AccountDto>().ReverseMap();
            CreateMap<CashflowEntity, CashflowDto>();
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
