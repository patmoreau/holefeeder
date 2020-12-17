using System;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.ObjectStore.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Application.Validators;

using FluentValidation.TestHelper;

using NSubstitute;

using Xunit;

namespace ObjectStore.UnitTests.Application
{
    public class CreateStoreItemCommandValidatorTests
    {
        private readonly IStoreQueriesRepository _repository;
        private readonly ItemsCache _cache;
        
        public CreateStoreItemCommandValidatorTests()
        {
            _repository = Substitute.For<IStoreQueriesRepository>();
            _cache = new ItemsCache {{"UserId", Guid.NewGuid()}};
        }
        
        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenCodeIsEmpty_ThenShouldHaveError()
        {
            _repository.CodeExistsAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(Task.FromResult(false));
            var validator = new CreateStoreItemCommandValidator(_repository, _cache);
            validator.ShouldHaveValidationErrorFor(m => m.Code, null as string);
        }
        
        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenCodeAlreadyExists_ThenShouldHaveError()
        {
            _repository.CodeExistsAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(Task.FromResult(true));
            var validator = new CreateStoreItemCommandValidator(_repository, _cache);
            validator.ShouldHaveValidationErrorFor(m => m.Code, "Code");
        }

        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenDataIsEmpty_ThenShouldHaveError()
        {
            _repository.CodeExistsAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(Task.FromResult(false));
            var validator = new CreateStoreItemCommandValidator(_repository, _cache);
            validator.ShouldHaveValidationErrorFor(m => m.Data, null as string);
        }
    }
}
