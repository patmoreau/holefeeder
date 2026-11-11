using System.Globalization;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Settings.Core.SaveSettings;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Settings.Api.SaveSettings;

internal sealed record SaveSettingsRequestRaw([FromBody] SaveSettingsBody Body)
{
    public static Result<SaveSettingsRequest> ToSaveSettingsRequest(SaveSettingsRequestRaw raw, User user)
    {
        var body = raw.Body;
        var effectiveDate = IsoDate.Parse(body.EffectiveDate, SaveSettingsErrors.EffectiveDateInvalid);
        var intervalType = IntervalType.Create(body.IntervalType);
        var frequency = int.TryParse(body.Frequency, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value)
            ? PeriodSettings.CreateFrequency(value)
            : (Result<int>)PeriodSettingsErrors.FrequencyInvalid;

        var validation = ResultAggregate.Create();
        AddFailure(validation, effectiveDate);
        AddFailure(validation, intervalType);
        AddFailure(validation, frequency);

        return validation.OnSuccess<SaveSettingsRequest>(() =>
            new SaveSettingsRequest(user, PeriodSettings.Valid(effectiveDate.Value, intervalType.Value, frequency.Value)));
    }

    private static void AddFailure<T>(ResultAggregate validation, Result<T> result)
    {
        if (result.IsFailure)
        {
            validation.AddResult(result.Error);
        }
    }
}

internal static class SaveSettingsErrors
{
    public const string CodeEffectiveDateInvalid = "settings-effective-date-invalid";

    public static ResultError EffectiveDateInvalid => new(CodeEffectiveDateInvalid, "Effective date must be a date as YYYY-MM-DD.");
}
