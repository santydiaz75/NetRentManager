using NetRentManagerWeb.Components.Pages;
using NetRentManagerWeb.Services.Api.Properties;
using Refit;

namespace NetRentManagerWeb;

public class PropertiesListPageTests
{
    private static PropertyListItem CreateItem(string title = "Casa")
        => new(Guid.NewGuid(), title, "Descripción", "Dirección", 1000m, "Available", 2, 1, 60m, null);

    private static ApiResponse<PagedPropertiesResponse> SuccessResponse(PagedPropertiesResponse content)
        => new(
            new HttpResponseMessage(System.Net.HttpStatusCode.OK) { RequestMessage = new HttpRequestMessage() },
            content,
            new RefitSettings());

    private static ApiResponse<PagedPropertiesResponse> FailureResponse()
        => new(
            new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { RequestMessage = new HttpRequestMessage() },
            null,
            new RefitSettings());

    private sealed class StubPropertiesApi : IPropertiesApi
    {
        private readonly Func<int, int, Task<ApiResponse<PagedPropertiesResponse>>> handler;

        public StubPropertiesApi(Func<int, int, Task<ApiResponse<PagedPropertiesResponse>>> handler)
            => this.handler = handler;

        public (int Page, int PageSize)? LastCall { get; private set; }

        public Task<ApiResponse<PagedPropertiesResponse>> GetPropertiesAsync(
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            LastCall = (page, pageSize);
            return handler(page, pageSize);
        }
    }

    [Fact]
    public async Task LoadAsync_UsesDefaultPageAndPageSize_WhenQueryMissing()
    {
        var response = SuccessResponse(new PagedPropertiesResponse([CreateItem()], 1, 6, 1, 1, false, false));
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
        var response = SuccessResponse(new PagedPropertiesResponse([], 1, 6, 0, 0, false, false));
        var api = new StubPropertiesApi((_, _) => Task.FromResult(response));
        var home = new Home { PropertiesApi = api };

        await home.LoadAsync();

        Assert.False(home.HasError);
        Assert.NotNull(home.Result);
        Assert.Empty(home.Result!.Items);
    }

    [Fact]
    public async Task LoadAsync_SetsHasError_WhenResponseIsNotSuccessful()
    {
        var api = new StubPropertiesApi((_, _) => Task.FromResult(FailureResponse()));
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
        var tcs = new TaskCompletionSource<ApiResponse<PagedPropertiesResponse>>();
        var api = new StubPropertiesApi((_, _) => tcs.Task);
        var home = new Home { PropertiesApi = api };

        var loadTask = home.LoadAsync();

        Assert.True(home.IsLoading);

        tcs.SetResult(SuccessResponse(new PagedPropertiesResponse([CreateItem()], 1, 6, 1, 1, false, false)));
        await loadTask;

        Assert.False(home.IsLoading);
    }
}
