using Holefeeder.Ui.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Holefeeder.Ui.Shared.Components;

public partial class CustomErrorBoundary : ErrorBoundary
{
    [Inject] private ExceptionService ExceptionService { get; set; } = default!;

    protected override Task OnErrorAsync(Exception exception)
    {
        // Store the exception
        ExceptionService.SetException(exception);

        // Navigate to the error page
        Navigation.NavigateTo("/error");

        return Task.CompletedTask;
    }
}
