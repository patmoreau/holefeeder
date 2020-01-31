﻿using AutoMapper;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SimpleInjector;

namespace DrifterApps.Holefeeder.Common.IoC
{
    public static class ContainerRegistration
    {
        public static void Initialize(this Container container, IConfiguration configuration)
        {
            // add singleton
            container.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);
            
            // add AutoMapper
            var mapperConfig = MapperRegistration.Initialize();
            container.Register<IMapper>(() => mapperConfig.CreateMapper(), Lifestyle.Scoped);

            // add MongoDb components
            container.RegisterSingleton<IMongoClient>(() => new MongoClient(configuration.GetValue<string>("DATABASE_URL")));
            container.RegisterSingleton<IMongoDatabase>(() => container.GetInstance<IMongoClient>().GetDatabase(configuration.GetValue<string>("DATABASE_NAME")));
            container.Register<IMongoCollection<AccountSchema>>(() => container.GetInstance<IMongoDatabase>().GetCollection<AccountSchema>("accounts"), Lifestyle.Scoped);
            container.Register<IMongoCollection<CashflowSchema>>(() => container.GetInstance<IMongoDatabase>().GetCollection<CashflowSchema>("cashflows"), Lifestyle.Scoped);
            container.Register<IMongoCollection<CategorySchema>>(() => container.GetInstance<IMongoDatabase>().GetCollection<CategorySchema>("categories"), Lifestyle.Scoped);
            container.Register<IMongoCollection<ObjectDataSchema>>(() => container.GetInstance<IMongoDatabase>().GetCollection<ObjectDataSchema>("objectsData"), Lifestyle.Scoped);
            container.Register<IMongoCollection<UserSchema>>(() => container.GetInstance<IMongoDatabase>().GetCollection<UserSchema>("users"), Lifestyle.Scoped);
            container.Register<IMongoCollection<TransactionSchema>>(() => container.GetInstance<IMongoDatabase>().GetCollection<TransactionSchema>("transactions"), Lifestyle.Scoped);

            // add application services
            container.Register<IAccountsService, AccountsService>(Lifestyle.Scoped);
            container.Register<ICashflowsService, CashflowsService>(Lifestyle.Scoped);
            container.Register<ICategoriesService, CategoriesService>(Lifestyle.Scoped);
            container.Register<IObjectsService, ObjectsService>(Lifestyle.Scoped);
            container.Register<IUsersService, UsersService>(Lifestyle.Scoped);
            container.Register<IStatisticsService, StatisticsService>(Lifestyle.Scoped);
            container.Register<ITransactionsService, TransactionsService>(Lifestyle.Scoped);

            // add application repositories
            container.Register<IAccountsRepository, AccountsRepository>(Lifestyle.Scoped);
            container.Register<ICashflowsRepository, CashflowsRepository>(Lifestyle.Scoped);
            container.Register<ICategoriesRepository, CategoriesRepository>(Lifestyle.Scoped);
            container.Register<IObjectsRepository, ObjectsRepository>(Lifestyle.Scoped);
            container.Register<IUsersRepository, UsersRepository>(Lifestyle.Scoped);
            container.Register<ITransactionsRepository, TransactionsRepository>(Lifestyle.Scoped);
        }
    }
}