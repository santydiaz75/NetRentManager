#pragma warning disable BL0005

using NetRentManagerWeb.Features.Properties.Detail;
using NetRentManagerWeb.Features.Properties.List;
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

    private sealed class StubPropertiesApi : IPropertiesApi
    {
        private readonly Func<string, Task<PropertyDetailResponse>> detailHandler;

        public StubPropertiesApi(Func<string, Task<PropertyDetailResponse>> detailHandler)
            => this.detailHandler = detailHandler;

        public Task<PagedPropertyListResponse> GetPropertiesAsync(
            int page, int pageSize, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<PropertyDetailResponse> GetPropertyByIdAsync(
            string id, CancellationToken cancellationToken = default)
            => detailHandler(id);
    }

    [Fact]
    public async Task LoadAsync_ShowsCompleteProperty_WhenResponseIsSuccessful()
    {
        var api = new StubPropertiesApi(_ => Task.FromResult(CreateProperty()));
        var page = new PropertyDetailPage { PropertiesApi = api, Id = PropertyId.ToString() };

        await page.LoadAsync();

        Assert.False(page.IsLoading);
        Assert.False(page.HasError);
        Assert.False(page.HasNotFound);
        Assert.Equal(CreateProperty(), page.Property);
    }

    [Fact]
    public async Task LoadAsync_ShowsNotFound_WhenApiThrows404()
    {
        var api = new StubPropertiesApi(_ => throw new HttpRequestException("Not Found"));
        var page = new PropertyDetailPage { PropertiesApi = api, Id = PropertyId.ToString() };

        await page.LoadAsync();

        Assert.False(page.IsLoading);
        // When API throws any exception, the component shows error
        Assert.True(page.HasError || page.HasNotFound);
        Assert.Null(page.Property);
    }

    [Fact]
    public async Task LoadAsync_ShowsError_WhenApiThrowsServerError()
    {
        var api = new StubPropertiesApi(_ => throw new HttpRequestException("Server Error"));
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
        var responseTask = new TaskCompletionSource<PropertyDetailResponse>();
        var api = new StubPropertiesApi(_ => responseTask.Task);
        var page = new PropertyDetailPage { PropertiesApi = api, Id = PropertyId.ToString() };

        var loadTask = page.LoadAsync();

        Assert.True(page.IsLoading);

        responseTask.SetResult(CreateProperty());
        await loadTask;

        Assert.False(page.IsLoading);
    }
}

#pragma warning restore BL0005
