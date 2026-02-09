using Asp.Versioning.Builder;

namespace Pipchi.Api.Abstractions;

public interface IEndpointDefinition
{
    void RegisterEndpoint(WebApplication app, ApiVersionSet apiVersionSet);
}
