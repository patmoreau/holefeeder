namespace Holefeeder.Ui.Shared.Services;

public class ExceptionService
{
    public Exception? LastException { get; private set; }

    public void SetException(Exception ex) => LastException = ex;

    public void ClearException() => LastException = null;
}
