namespace Holefeeder.Application.SeedWork.BackgroundRequest;

public class MediatorSerializedObject
{
    public string FullTypeName { get; }

    public string Data { get; }

    public string AdditionalDescription { get; }

    public MediatorSerializedObject(string fullTypeName, string data, string additionalDescription)
    {
        this.FullTypeName = fullTypeName;
        this.Data = data;
        this.AdditionalDescription = additionalDescription;
    }

    /// <summary>
    /// Override for Hangfire dashboard display.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var commandName = this.FullTypeName.Split('.').Last();
        return $"{commandName} {this.AdditionalDescription}";
    }
}
