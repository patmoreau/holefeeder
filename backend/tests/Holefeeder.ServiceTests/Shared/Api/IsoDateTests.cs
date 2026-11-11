using Holefeeder.Service.Shared.Api;

namespace Holefeeder.ServiceTests.Shared.Api;

public class IsoDateTests
{
    [Fact]
    public void Parse_ReadsAnIsoDate() =>
        IsoDate.Parse("2026-11-03", IsoDateErrors.DateInvalid).Should().BeSuccessful().And.WithValue(new DateOnly(2026, 11, 3));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("11/03/2026")]
    [InlineData("2026-02-30")]
    public void Parse_FailsWithTheGivenError_WhenNotAnIsoDate(string? value) =>
        IsoDate.Parse(value, IsoDateErrors.CashflowDateInvalid).Should().BeFailure().And.WithError(IsoDateErrors.CashflowDateInvalid);

    [Fact]
    public void Errors_NameTheField()
    {
        IsoDateErrors.DateInvalid.Code.Should().Be("date-invalid");
        IsoDateErrors.DateInvalid.Description.Should().Be("Date must be a date as YYYY-MM-DD.");
        IsoDateErrors.CashflowDateInvalid.Code.Should().Be("cashflow-date-invalid");
        IsoDateErrors.CashflowDateInvalid.Description.Should().Be("Cashflow date must be a date as YYYY-MM-DD.");
    }
}
