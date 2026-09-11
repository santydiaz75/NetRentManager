using Microsoft.Playwright;

namespace NetRentManagerApiTests.Infrastructure.Swagger;

public sealed class SwaggerUiBrowserTests
{
    [Fact]
    public async Task Development_ui_can_execute_health_request_with_try_it_out()
    {
        var baseUrl = Environment.GetEnvironmentVariable("SWAGGER_UI_BASE_URL");
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return;
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var page = await browser.NewPageAsync();

        await page.GotoAsync($"{baseUrl}/swagger/index.html");
        await page.Locator("#swagger-ui").WaitForAsync();
        await page.GetByText("GET", new PageGetByTextOptions { Exact = true }).First.ClickAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Try it out" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Execute" }).ClickAsync();

        await Assertions.Expect(page.GetByText("200", new PageGetByTextOptions { Exact = true }).Last)
            .ToBeVisibleAsync();
        await Assertions.Expect(page.Locator(".response-col_description").Last)
            .ToContainTextAsync("OK");
    }

    [Fact]
    public async Task Development_ui_reports_invalid_openapi_document()
    {
        var baseUrl = Environment.GetEnvironmentVariable("SWAGGER_UI_BASE_URL");
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return;
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var page = await browser.NewPageAsync();

        await page.RouteAsync("**/openapi/v1.json", async route =>
        {
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = "{ invalid"
            });
        });

        await page.GotoAsync($"{baseUrl}/swagger/index.html");
        await Assertions.Expect(page.Locator(".errors-wrapper")).ToBeVisibleAsync();
    }
}
