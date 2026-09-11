namespace NetRentManagerApi.Features.Properties.ListProperties;

public sealed class ListPropertiesRequest
{
    public int? Page { get; init; }

    public int? PageSize { get; init; }

    public int EffectivePage => Page ?? 1;

    public int EffectivePageSize => PageSize ?? 6;
}
