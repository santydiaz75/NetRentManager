namespace NetRentManagerApi.Infrastructure.Endpoints;

public interface ISlice
{
    void AddEndpoint(IEndpointRouteBuilder app);
}