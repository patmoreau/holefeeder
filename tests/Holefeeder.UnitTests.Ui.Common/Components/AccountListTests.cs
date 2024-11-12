using Bogus;

using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Attributes;

using Holefeeder.Ui.Common.Components;
using Holefeeder.Ui.Common.Models;
using Holefeeder.Ui.Common.Services;
using Holefeeder.UnitTests.Ui.Common.Builders;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using MudBlazor;
using MudBlazor.Services;

namespace Holefeeder.UnitTests.Ui.Common.Components;

[UnitTest]
public class AccountListTests : TestContext
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenOnInitialized_WhenSelectedItemIsNull_ThenFirstItemIsSelected()
    {
        // Arrange
        var categories = new AccountBuilder().BuildCollection();
        var driver = new Driver(this)
            .WithCategoryList(categories)
            .WithNoSelectedCategoryParameterPassed();

        // Act
        var cut = driver.Build();
        var select = cut.FindComponent<MudSelect<Account>>();

        // Assert
        select.Should().BeAssignableTo<IRenderedComponent<MudSelect<Account>>>();
        select.Instance.Value.Should().NotBeNull().And.BeEquivalentTo(categories.First());
        driver.ShouldHaveInvokedEventCallback()
            .ShouldHaveReceivedSelectedCategory(categories.First());
    }

    [Fact]
    public void GivenOnInitialized_WhenSelectedItemIsProvided_ThenItemSelected()
    {
        // Arrange
        var accounts = new AccountBuilder().BuildCollection();
        var selected = _faker.PickRandom<Account>(accounts);
        var driver = new Driver(this)
            .WithCategoryList(accounts)
            .WithSelectedCategoryParameterPassed(selected);

        // Act
        var cut = driver.Build();
        var select = cut.FindComponent<MudSelect<Account>>();

        // Assert
        select.Should().BeAssignableTo<IRenderedComponent<MudSelect<Account>>>();
        select.Instance.Value.Should().NotBeNull().And.BeEquivalentTo(selected);
        driver.ShouldHaveInvokedEventCallback()
            .ShouldHaveReceivedSelectedCategory(selected);
    }

    [Fact]
    public async Task GivenOnInitialized_WhenUserSelectsItem_ThenItemSelected()
    {
        // Arrange
        var categories = new AccountBuilder().BuildCollection(10);
        var selected = _faker.PickRandom<Account>(categories);
        var driver = new Driver(this)
            .WithCategoryList(categories)
            .WithNoSelectedCategoryParameterPassed();

        // Act
        var cut = driver.Build();
        var select = cut.FindComponent<MudSelect<Account>>();
        await select.InvokeAsync(() => select.Instance.SelectOption(selected));

        // Assert
        driver.ShouldHaveInvokedEventCallback().ShouldHaveReceivedSelectedCategory(selected);
    }

    private sealed class Driver : IDriverOf<IRenderedComponent<AccountList>>
    {
        private readonly TestContext _context;
        private readonly IHolefeederApiService _mockApiService;
        private readonly IApiResponse<IList<Account>> _mockApiResponse;
        private readonly EventCallback<Account> _selectedCategoryChangedEventCallback;

        private Account? _selected;
        private bool _selectedCategoryChangedInvoked;
        private Account? _selectedCategoryChanged;

        public Driver(TestContext context)
        {
            _context = context;

            _context.Services.AddMudServices(options => { options.PopoverOptions.CheckForPopoverProvider = false; });
            _context.JSInterop.Mode = JSRuntimeMode.Loose;

            _mockApiResponse = Substitute.For<IApiResponse<IList<Account>>>();
            _mockApiService = Substitute.For<IHolefeederApiService>();
            _context.Services.AddSingleton(_mockApiService);

            _selectedCategoryChangedInvoked = false;
            _selectedCategoryChangedEventCallback = EventCallback.Factory.Create<Account>(this, category =>
            {
                _selectedCategoryChangedInvoked = true;
                _selectedCategoryChanged = category;
            });
        }

        public IRenderedComponent<AccountList> Build()
        {
            var cut = _context.RenderComponent<AccountList>(parameters => parameters
                .Add(p => p.SelectedAccount, _selected)
                .Add(p => p.SelectedAccountChanged, _selectedCategoryChangedEventCallback));
            cut.WaitForState(() => cut.Instance.IsLoaded);
            return cut;
        }

        public Driver WithNoSelectedCategoryParameterPassed()
        {
            _selected = null;
            return this;
        }

        public Driver WithSelectedCategoryParameterPassed(Account category)
        {
            _selected = category;
            return this;
        }

        public Driver WithCategoryList(IReadOnlyCollection<Account> categories)
        {
            _mockApiResponse.IsSuccessStatusCode.Returns(true);
            _mockApiResponse.Content.Returns(categories.ToList());
            _mockApiService.GetAccounts(Arg.Any<string[]>(), Arg.Any<string[]>()).Returns(_mockApiResponse);
            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveInvokedEventCallback()
        {
            _selectedCategoryChangedInvoked.Should().BeTrue();
            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveReceivedSelectedCategory(Account category)
        {
            _selectedCategoryChanged.Should().NotBeNull().And.BeEquivalentTo(category);
            return this;
        }
    }
}
