using Microsoft.AspNetCore.Components;
using NetRentManagerWeb.Services.Api.Properties;

namespace NetRentManagerWeb.Components.Pages;

public partial class Home
{
    [Inject]
    public IPropertiesApi PropertiesApi { get; set; } = null!;

    [SupplyParameterFromQuery(Name = "page")]
    public int? PageQuery { get; set; }

    [SupplyParameterFromQuery(Name = "pageSize")]
    public int? PageSizeQuery { get; set; }

    public int Page => PageQuery ?? 1;

    public int PageSize => PageSizeQuery ?? 6;

    public bool IsLoading { get; private set; } = true;

    public bool HasError { get; private set; }

    public PagedPropertiesResponse? Result { get; private set; }

    protected override async Task OnInitializedAsync() => await LoadAsync();

    public async Task LoadAsync()
    {
        IsLoading = true;
        HasError = false;
        Result = null;

        try
        {
            var response = await PropertiesApi.GetPropertiesAsync(Page, PageSize);
            if (response.IsSuccessful && response.Content is not null)
            {
                Result = response.Content;
            }
            else
            {
                HasError = true;
            }
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
}
