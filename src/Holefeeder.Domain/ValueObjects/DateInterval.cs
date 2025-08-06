namespace Holefeeder.Domain.ValueObjects;
/// <summary>
/// Represents a date interval with a start and end date.
/// </summary>
public record DateInterval
{
    /// <summary>
    /// Gets the start date of the interval.
    /// </summary>
    public DateOnly Start { get; init; }

    /// <summary>
    /// Gets the end date of the interval.
    /// </summary>
    public DateOnly End { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateInterval"/> record.
    /// </summary>
    /// <param name="start">The start date of the interval.</param>
    /// <param name="end">The end date of the interval.</param>
    /// <exception cref="ArgumentException">Thrown when the end date is earlier than the start date.</exception>
    public DateInterval(DateOnly start, DateOnly end)
    {
        if (end < start)
        {
            throw new ArgumentException("End date must be greater than or equal to start date.");
        }

        Start = start;
        End = end;
    }
}
