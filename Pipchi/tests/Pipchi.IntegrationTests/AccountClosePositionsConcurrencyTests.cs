using Microsoft.AspNetCore.Mvc.Testing;
using Pipchi.Api.Models.Symbol.Create;
using System.Net.Http.Json;

namespace Pipchi.IntegrationTests;

public class AccountClosePositionsConcurrencyTests
        : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccountClosePositionsConcurrencyTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CloseAllPositions_ShouldClosePositionsOnlyOnce_WhenCalledConcurrently()
    {
        // Arrange
        var symbolId = await CreateSymbol();
    }

    private async Task<int> CreateSymbol()
    {
        var request = new
        {
            Name = $"SYM-{Guid.NewGuid()}",
            Digits = 5,
            MinPrice = 1,
            MaxPrice = 5,
            MinVolume = 0.01m,
            MaxVolume = 100,
            VolumeStep = 0.01m,
            ContractSize = 100000,
            MarketOpenTime = TimeSpan.FromHours(0),
            MarketCloseTime = TimeSpan.FromHours(23)
        };

        var response = await _client.PostAsJsonAsync("/api/symbols", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CreateSymbolResponse>();

        return result!.SymbolDto.Id;
    }

}
