using System;

using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions;

using FluentAssertions;

using Xunit;

namespace ObjectStore.UnitTests.Domain
{
    public class StoreItemTests
    {
        [Fact]
        public void GivenCreateStoreItem_WhenCodeEmpty_ThenThrowsException()
        {
            Action action = () => StoreItem.Create("", "data", Guid.NewGuid());

            action.Should().Throw<ObjectStoreDomainException>();
        }

        [Fact]
        public void GivenNewStoreItem_WhenIdEmpty_ThenThrowsException()
        {
            Action action = () => new StoreItem(Guid.Empty, "code", "data", Guid.NewGuid());

            action.Should().Throw<ObjectStoreDomainException>();
        }

        [Fact]
        public void GivenNewStoreItem_WhenUserIdEmpty_ThenThrowsException()
        {
            Action action = () => new StoreItem(Guid.NewGuid(), "code", "data", Guid.Empty);

            action.Should().Throw<ObjectStoreDomainException>();
        }
    }
}
