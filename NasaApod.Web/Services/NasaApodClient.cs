using NasaApodMvc.DTOs;
using System.Net.Http;
using System.Text.Json;

namespace NasaApodMvc.Services;

public class NasaApodClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public NasaApodClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<ApodDto>> GetRangeAsync(DateOnly start, DateOnly end)
    {
        var apiKey = _configuration["Nasa:ApiKey"];
        if (string.IsNullOrEmpty(apiKey)) throw new InvalidOperationException("Nasa:ApiKey is missing in configuration.");

        // WORKAROUND: Use IP directly to bypass DNS issues
        // NASA API format: YYYY-MM-DD
        // Original: https://api.nasa.gov/planetary/apod
        string url = $"https://3.31.25.7/planetary/apod?api_key={apiKey}&start_date={start:yyyy-MM-dd}&end_date={end:yyyy-MM-dd}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Host = "api.nasa.gov";

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        var dtoList = JsonSerializer.Deserialize<List<ApodDto>>(json);
        return dtoList ?? new List<ApodDto>();
    }
}
