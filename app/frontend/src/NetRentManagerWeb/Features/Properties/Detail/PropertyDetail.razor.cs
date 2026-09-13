using System.Net;
using Microsoft.AspNetCore.Components;
using Refit;
using NetRentManagerWeb.Services.Api.Properties;

namespace NetRentManagerWeb.Features.Properties.Detail;

public partial class PropertyDetail : ComponentBase, IAsyncDisposable
{
    private CancellationTokenSource? cancellationTokenSource;
    private PropertyDetailResponse? detail;
    private PropertyDetailStateType state = PropertyDetailStateType.Loading;
    private string? errorMessage;
    private long requestVersion;

    [Parameter]
    public string Id { get; set; } = default!;

    [Inject] private IPropertiesApi PropertiesApi { get; set; } = default!;

    [Inject] private NavigationManager Navigation { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        requestVersion++;
        var version = requestVersion;

        if (cancellationTokenSource is not null)
        {
            await cancellationTokenSource.CancelAsync();
            cancellationTokenSource.Dispose();
        }

        cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        state = PropertyDetailStateType.Loading;
        errorMessage = null;
        detail = null;
        StateHasChanged();

        try
        {
            var response = await PropertiesApi.GetPropertyByIdAsync(Id, cancellationToken);

            if (version != requestVersion)
            {
                return;
            }

            detail = response;
            state = PropertyDetailStateType.Success;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.NotFound)
        {
            if (version != requestVersion)
            {
                return;
            }

            state = PropertyDetailStateType.NotFound;
        }
        catch (Exception)
        {
            if (version != requestVersion)
            {
                return;
            }

            state = PropertyDetailStateType.Error;
            errorMessage = "No fue posible cargar el detalle de la propiedad. Intenta nuevamente.";
        }
        finally
        {
            if (version == requestVersion)
            {
                StateHasChanged();
            }
        }
    }

    private Task ReloadAsync() => LoadAsync();

    private void BackToList() => Navigation.NavigateTo("/");

    public async ValueTask DisposeAsync()
    {
        if (cancellationTokenSource is not null)
        {
            await cancellationTokenSource.CancelAsync();
            cancellationTokenSource.Dispose();
        }
    }
}
