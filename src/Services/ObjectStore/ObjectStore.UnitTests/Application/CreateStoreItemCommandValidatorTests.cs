using System;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.ObjectStore.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Application.Validators;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace ObjectStore.UnitTests.Application
{
    public class CreateStoreItemCommandValidatorTests
    {
        private readonly ILogger<CreateStoreItemCommandValidator> _logger;
        
        public CreateStoreItemCommandValidatorTests()
        {
            _logger = Substitute.For<ILogger<CreateStoreItemCommandValidator>>();
        }
        
        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenCodeIsEmpty_ThenShouldHaveError()
        {
            var validator = new CreateStoreItemCommandValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Code, null as string);
        }

        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenDataIsEmpty_ThenShouldHaveError()
        {
            var validator = new CreateStoreItemCommandValidator(_logger);
            validator.ShouldHaveValidationErrorFor(m => m.Data, null as string);
        }
    }
}
