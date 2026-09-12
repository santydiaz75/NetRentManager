using Microsoft.AspNetCore.Components;
using NetRentManagerWeb.Services.Api.Properties;
using Refit;

namespace NetRentManagerWeb.Features.Properties.Detail;

public partial class PropertyDetailPage
{
    [Inject]
    public IPropertiesApi PropertiesApi { get; set; } = null!;

    [Parameter]
    public string Id { get; set; } = string.Empty;

    public bool IsLoading { get; private set; } = true;

    public bool HasNotFound { get; private set; }

    public bool HasError { get; private set; }

    public PropertyDetailResponse? Property { get; private set; }

    protected override async Task OnParametersSetAsync()
    {
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        HasNotFound = false;
        HasError = false;
        Property = null;

        try
        {
            if (!Guid.TryParse(Id, out var propertyId))
            {
                HasError = true;
                return;
            }

            var response = await PropertiesApi.GetPropertyByIdAsync(propertyId.ToString());
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                HasNotFound = true;
            }
            else if (response.IsSuccessful && response.Content is not null)
            {
                Property = response.Content;
            }
            else
            {
                HasError = true;
            }
        }
        catch (ApiException exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            HasNotFound = true;
        }
        catch (Exception)
        {
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string StatusBadgeClass => Property?.Status switch
    {
        "Available" => "badge-success",
        "Maintenance" => "badge-warning",
        "Rented" => "badge-danger",
        _ => string.Empty
    };
}
