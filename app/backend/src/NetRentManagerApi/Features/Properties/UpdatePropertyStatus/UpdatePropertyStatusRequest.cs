using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetRentManagerApi.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusRequest
{
    public string? Status { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; init; }
}