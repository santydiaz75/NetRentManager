#pragma warning disable BL0005

using NetRentManagerWeb.Features.Properties.Detail;
using NetRentManagerWeb.Services.Api.Properties;
using Refit;

namespace NetRentManagerWeb;

public class PropertyDetailPageTests
{
    private static readonly Guid PropertyId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static PropertyDetailResponse CreateProperty()
        => new(
            PropertyId,
            "Casa central",
            "Descripción completa",
            "Calle Principal 123",
            1500m,
            "Available",
            3,
            2,
            90m,
            null);

    private static ApiResponse<PropertyDetailResponse> Response(
        System.Net.HttpStatusCode statusCode,
        PropertyDetailResponse? content = null)
        => new(
            new HttpResponseMessage(statusCode) { RequestMessage = new HttpRequestMessage() },
            content,
            new RefitSettings());

    private sealed class StubPropertiesApi : IPropertiesApi
    {
        private readonly Func<string, Task<ApiResponse<PropertyDetailResponse>>> detailHandler;

        public StubPropertiesApi(Func<string, Task<ApiResponse<PropertyDetailResponse>>> detailHandler)
            => this.detailHandler = detailHandler;

        public Task<ApiResponse<PagedPropertiesResponse>> GetPropertiesAsync(
            int page, int pageSize, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<ApiResponse<PropertyDetailResponse>> GetPropertyByIdAsync(
            string id, CancellationToken cancellationToken = default)
            => detailHandler(id);
    }

    [Fact]
    public async Task LoadAsync_ShowsCompleteProperty_WhenResponseIsSuccessful()
    {
        var api = new StubPropertiesApi(id => Task.FromResult(Response(
            System.Net.HttpStatusCode.OK,
            CreateProperty())));
        var page = new PropertyDetailPage { PropertiesApi = api, Id = PropertyId.ToString() };

        await page.LoadAsync();

        Assert.False(page.IsLoading);
        Assert.False(page.HasError);
        Assert.False(page.HasNotFound);
        Assert.Equal(CreateProperty(), page.Property);
    }

    [Fact]
    public async Task LoadAsync_ShowsNotFound_WhenApiReturns404()
    {
        var api = new StubPropertiesApi(_ => Task.FromResult(Response(System.Net.HttpStatusCode.NotFound)));
        var page = new PropertyDetailPage { PropertiesApi = api, Id = PropertyId.ToString() };

        await page.LoadAsync();

        Assert.False(page.IsLoading);
        Assert.True(page.HasNotFound);
        Assert.False(page.HasError);
        Assert.Null(page.Property);
    }

    [Fact]
    public async Task LoadAsync_ShowsError_WhenApiFails()
    {
        var api = new StubPropertiesApi(_ => Task.FromResult(Response(System.Net.HttpStatusCode.InternalServerError)));
        var page = new PropertyDetailPage { PropertiesApi = api, Id = PropertyId.ToString() };

        await page.LoadAsync();

        Assert.False(page.IsLoading);
        Assert.True(page.HasError);
        Assert.False(page.HasNotFound);
    }

    [Fact]
    public async Task LoadAsync_ShowsError_WhenNetworkFails()
    {
        var api = new StubPropertiesApi(_ => throw new HttpRequestException("network down"));
        var page = new PropertyDetailPage { PropertiesApi = api, Id = PropertyId.ToString() };

        await page.LoadAsync();

        Assert.False(page.IsLoading);
        Assert.True(page.HasError);
        Assert.Null(page.Property);
    }

    [Fact]
    public async Task LoadAsync_ShowsErrorWithoutCallingApi_WhenIdIsInvalid()
    {
        var api = new StubPropertiesApi(_ => throw new InvalidOperationException("API should not be called"));
        var page = new PropertyDetailPage { PropertiesApi = api, Id = "id-invalido" };

        await page.LoadAsync();

        Assert.False(page.IsLoading);
        Assert.True(page.HasError);
        Assert.Null(page.Property);
    }

    [Fact]
    public async Task LoadAsync_ReportsLoadingUntilRequestCompletes()
    {
        var responseTask = new TaskCompletionSource<ApiResponse<PropertyDetailResponse>>();
        var api = new StubPropertiesApi(_ => responseTask.Task);
        var page = new PropertyDetailPage { PropertiesApi = api, Id = PropertyId.ToString() };

        var loadTask = page.LoadAsync();

        Assert.True(page.IsLoading);

        responseTask.SetResult(Response(System.Net.HttpStatusCode.OK, CreateProperty()));
        await loadTask;

        Assert.False(page.IsLoading);
    }
}

#pragma warning restore BL0005
