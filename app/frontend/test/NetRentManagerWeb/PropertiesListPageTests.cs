using NetRentManagerWeb.Components.Pages;
using NetRentManagerWeb.Features.Properties.List;
using NetRentManagerWeb.Services.Api.Properties;

namespace NetRentManagerWeb;

public class PropertiesListPageTests
{
    private static PropertyListItem CreateItem(string title = "Casa")
        => new(Guid.NewGuid(), title, null, 1000m, "Available", "Dirección", 2, 1, 60m);

    private sealed class StubPropertiesApi : IPropertiesApi
    {
        private readonly Func<int, int, Task<PagedPropertyListResponse>> handler;

        public StubPropertiesApi(Func<int, int, Task<PagedPropertyListResponse>> handler)
            => this.handler = handler;

        public (int Page, int PageSize)? LastCall { get; private set; }

        public Task<PagedPropertyListResponse> GetPropertiesAsync(
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            LastCall = (page, pageSize);
            return handler(page, pageSize);
        }

        public Task<PropertyDetailResponse> GetPropertyByIdAsync(
            string id, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }

    [Fact]
    public async Task LoadAsync_UsesDefaultPageAndPageSize_WhenQueryMissing()
    {
        var response = new PagedPropertyListResponse([CreateItem()], 1, 6, 1, 1, false, false);
        var api = new StubPropertiesApi((_, _) => Task.FromResult(response));
        var home = new Home { PropertiesApi = api };

        await home.LoadAsync();

        Assert.Equal(1, home.Page);
        Assert.Equal(6, home.PageSize);
        Assert.Equal((1, 6), api.LastCall);
    }

    [Fact]
    public async Task LoadAsync_ShowsEmptyState_WhenItemsAreEmpty()
    {
        var response = new PagedPropertyListResponse([], 1, 6, 0, 0, false, false);
        var api = new StubPropertiesApi((_, _) => Task.FromResult(response));
        var home = new Home { PropertiesApi = api };

        await home.LoadAsync();

        Assert.False(home.HasError);
        Assert.NotNull(home.Result);
        Assert.Empty(home.Result!.Items);
    }

    [Fact]
    public async Task LoadAsync_SetsHasError_WhenResponseIsNull()
    {
        var api = new StubPropertiesApi((_, _) => Task.FromResult((PagedPropertyListResponse?)null)!);
        var home = new Home { PropertiesApi = api };

        await home.LoadAsync();

        Assert.True(home.HasError);
        Assert.Null(home.Result);
    }

    [Fact]
    public async Task LoadAsync_SetsHasError_WhenExceptionIsThrown()
    {
        var api = new StubPropertiesApi((_, _) => throw new HttpRequestException("network down"));
        var home = new Home { PropertiesApi = api };

        await home.LoadAsync();

        Assert.True(home.HasError);
    }

    [Fact]
    public async Task LoadAsync_TogglesIsLoading_BeforeAndAfterCompletion()
    {
        var tcs = new TaskCompletionSource<PagedPropertyListResponse>();
        var api = new StubPropertiesApi((_, _) => tcs.Task);
        var home = new Home { PropertiesApi = api };

        var loadTask = home.LoadAsync();

        Assert.True(home.IsLoading);

        tcs.SetResult(new PagedPropertyListResponse([CreateItem()], 1, 6, 1, 1, false, false));
        await loadTask;

        Assert.False(home.IsLoading);
    }
}
