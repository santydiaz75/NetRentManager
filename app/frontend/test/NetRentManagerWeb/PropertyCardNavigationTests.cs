namespace NetRentManagerWeb;

public class PropertyCardNavigationTests
{
    [Fact]
    public void PropertyCard_ContainsDetailNavigationWithPropertyId()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..", "src", "NetRentManagerWeb",
            "Features", "Properties", "List", "PropertyCard.razor");
        var content = File.ReadAllText(path);

        Assert.Contains("href=\"@($\"/properties/{Item.Id}\")\"", content);
        Assert.Contains(">Ver</NavLink>", content);
    }
}
