using Bogus;

using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Attributes;

using Holefeeder.Ui.Shared.Components;
using Holefeeder.Ui.Shared.Models;
using Holefeeder.Ui.Shared.Services;
using Holefeeder.UnitTests.Ui.Common.Builders;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using MudBlazor;
using MudBlazor.Services;

namespace Holefeeder.UnitTests.Ui.Common.Components;

[UnitTest]
public class CategoryListTests : TestContext
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenOnInitialized_WhenSelectedItemIsNull_ThenFirstItemIsSelected()
    {
        // Arrange
        var categories = new CategoryBuilder().BuildCollection();
        var driver = new Driver(this)
            .WithCategoryList(categories)
            .WithNoSelectedCategoryParameterPassed();

        // Act
        var cut = driver.Build();
        var select = cut.FindComponent<MudSelect<Category>>();

        // Assert
        select.Should().BeAssignableTo<IRenderedComponent<MudSelect<Category>>>();
        select.Instance.Value.Should().NotBeNull().And.BeEquivalentTo(categories.First());
        driver.ShouldHaveInvokedEventCallback()
            .ShouldHaveReceivedSelectedCategory(categories.First());
    }

    [Fact]
    public void GivenOnInitialized_WhenSelectedItemIsProvided_ThenItemSelected()
    {
        // Arrange
        var categories = new CategoryBuilder().BuildCollection();
        var selected = _faker.PickRandom<Category>(categories);
        var driver = new Driver(this)
            .WithCategoryList(categories)
            .WithSelectedCategoryParameterPassed(selected);

        // Act
        var cut = driver.Build();
        var select = cut.FindComponent<MudSelect<Category>>();

        // Assert
        select.Should().BeAssignableTo<IRenderedComponent<MudSelect<Category>>>();
        select.Instance.Value.Should().NotBeNull().And.BeEquivalentTo(selected);
        driver.ShouldHaveInvokedEventCallback()
            .ShouldHaveReceivedSelectedCategory(selected);
    }

    [Fact]
    public async Task GivenOnInitialized_WhenUserSelectsItem_ThenItemSelected()
    {
        // Arrange
        var categories = new CategoryBuilder().BuildCollection(10);
        var selected = _faker.PickRandom<Category>(categories);
        var driver = new Driver(this)
            .WithCategoryList(categories)
            .WithNoSelectedCategoryParameterPassed();

        // Act
        var cut = driver.Build();
        var select = cut.FindComponent<MudSelect<Category>>();
        await select.InvokeAsync(() => select.Instance.SelectOption(selected));

        // Assert
        driver.ShouldHaveInvokedEventCallback().ShouldHaveReceivedSelectedCategory(selected);
    }

    private sealed class Driver : IDriverOf<IRenderedComponent<CategoryList>>
    {
        private readonly TestContext _context;
        private readonly IHolefeederApiService _mockApiService;
        private readonly IApiResponse<IList<Category>> _mockApiResponse;
        private readonly EventCallback<Category> _selectedCategoryChangedEventCallback;

        private Category? _selected;
        private bool _selectedCategoryChangedInvoked;
        private Category? _selectedCategoryChanged;

        public Driver(TestContext context)
        {
            _context = context;

            _context.Services.AddMudServices(options => { options.PopoverOptions.CheckForPopoverProvider = false; });
            _context.JSInterop.Mode = JSRuntimeMode.Loose;

            _mockApiResponse = Substitute.For<IApiResponse<IList<Category>>>();
            _mockApiService = Substitute.For<IHolefeederApiService>();
            _context.Services.AddSingleton(_mockApiService);

            _selectedCategoryChangedInvoked = false;
            _selectedCategoryChangedEventCallback = EventCallback.Factory.Create<Category>(this, category =>
            {
                _selectedCategoryChangedInvoked = true;
                _selectedCategoryChanged = category;
            });
        }

        public IRenderedComponent<CategoryList> Build()
        {
            var cut = _context.RenderComponent<CategoryList>(parameters => parameters
                .Add(p => p.SelectedCategory, _selected)
                .Add(p => p.SelectedCategoryChanged, _selectedCategoryChangedEventCallback));
            cut.WaitForState(() => cut.Instance.IsLoaded);
            return cut;
        }

        public Driver WithNoSelectedCategoryParameterPassed()
        {
            _selected = null;
            return this;
        }

        public Driver WithSelectedCategoryParameterPassed(Category category)
        {
            _selected = category;
            return this;
        }

        public Driver WithCategoryList(IReadOnlyCollection<Category> categories)
        {
            _mockApiResponse.IsSuccessStatusCode.Returns(true);
            _mockApiResponse.Content.Returns(categories.ToList());
            _mockApiService.GetCategories().Returns(_mockApiResponse);
            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveInvokedEventCallback()
        {
            _selectedCategoryChangedInvoked.Should().BeTrue();
            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveReceivedSelectedCategory(Category category)
        {
            _selectedCategoryChanged.Should().NotBeNull().And.BeEquivalentTo(category);
            return this;
        }
    }
}
