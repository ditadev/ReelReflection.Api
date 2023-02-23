using System.Text.Json;
using ReelReflection.Features.Helper;
using ReelReflection.Model;

namespace ReelReflection.Features;

public class MovieService : IMovieService
{
    private const int MaxHistory = 5;
    private static readonly List<Movie> _searchHistory = new();
    private readonly AppSettings _appSettings;
    private readonly IHttpClientWrapper _httpClientWrapper;

    public MovieService(AppSettings appSettings, IHttpClientWrapper httpClientWrapper)
    {
        _appSettings = appSettings;
        _httpClientWrapper = httpClientWrapper;
    }

    public async Task<Movie?> GetMovieByTitle(string title)
    {
        var queryParams = $"{_appSettings.Api}?t={title}?&apikey={_appSettings.ApiKey}";
        HttpResponseMessage response = await _httpClientWrapper.GetAsync(queryParams);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var searchResult = JsonSerializer.Deserialize<Movie>(content, jsonOptions);
            await SearchHistory(searchResult);
            return searchResult;
        }

        return null;
    }

    public async Task<Movie?> GetMovieById(string id)
    {
        var queryParams = $"{_appSettings.Api}?i={id}&apikey={_appSettings.ApiKey}";
        HttpResponseMessage response = await _httpClientWrapper.GetAsync(queryParams);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var movieDetails = JsonSerializer.Deserialize<Movie>(content, jsonOptions);
            return movieDetails;
        }

        return null;
    }

    public Task<List<Movie>> SearchHistory(Movie? movie = null)
    {
        if (movie != null)
        {
            MovieService._searchHistory.Add(movie);

            if (MovieService._searchHistory.Count > MovieService.MaxHistory)
            {
                MovieService._searchHistory.RemoveAt(0);
            }
        }

        return  Task.FromResult(MovieService._searchHistory);
    }
}
