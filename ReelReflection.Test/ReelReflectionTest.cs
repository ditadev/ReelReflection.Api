using System.Net;
using System.Text.Json;
using Moq;
using ReelReflection.Features;
using ReelReflection.Features.Helper;
using ReelReflection.Model;

namespace ReelReflection.Test;

public class ReelReflectionTest
{
    private const int MaxHistory = 5;

    [Fact]
    public async Task GetMovieByTitle_ReturnsNull_WhenHttpResponseIsNotSuccessful()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            Api = "http://example.com",
            ApiKey = "123",
        };
        var expectedMovie = new Movie
        {
            Title = "Foo",
        };

        var mockHttpClient = new Mock<IHttpClientWrapper>();

        mockHttpClient.Setup(x => x.GetAsync($"{appSettings.Api}?t={expectedMovie.Title}?&apikey={appSettings.ApiKey}"))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var movieService = new MovieService(appSettings, mockHttpClient.Object);

        // Act
        Movie? result = await movieService.GetMovieByTitle(expectedMovie.Title);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetMovieByTitle_ReturnsMovie_WhenHttpResponseIsSuccessful()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            Api = "http://example.com",
            ApiKey = "123",
        };
        var mockHttpClient = new Mock<IHttpClientWrapper>();
        var expectedMovie = new Movie
        {
            Title = "Test Movie",
        };
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        string content = JsonSerializer.Serialize(expectedMovie, jsonOptions);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(content),
        };

        mockHttpClient.Setup(x => x.GetAsync($"{appSettings.Api}?t={expectedMovie.Title}?&apikey={appSettings.ApiKey}"))
            .ReturnsAsync(response);

        var movieService = new MovieService(appSettings, mockHttpClient.Object);

        // Act
        Movie? actualMovie = await movieService.GetMovieByTitle("Test Movie");

        // Assert
        Assert.NotNull(actualMovie);
        Assert.Equal(expectedMovie.Title, actualMovie?.Title);
    }

    [Fact]
    public async Task SearchHistory_AddsMovieToHistory_WhenMovieIsNotNull()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            Api = "http://example.com",
            ApiKey = "123",
        };

        var movie = new Movie
        {
            Title = "Test Movie 3",
        };

        var mockHttpClient = new Mock<IHttpClientWrapper>();

        mockHttpClient.Setup(x => x.GetAsync($"{appSettings.Api}?t={movie.Title}?&apikey={appSettings.ApiKey}"))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));


        var movieService = new MovieService(appSettings, mockHttpClient.Object);

        // Act
        List<Movie> searchHistory = await movieService.SearchHistory(movie);

        // Assert
        Assert.Contains(searchHistory, m => m.Title == movie.Title);
        Assert.Equal("Test Movie 3", searchHistory[0].Title);
    }


    [Fact]
    public async Task GetMovieById_ReturnsMovie_WhenHttpResponseIsSuccessful()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            Api = "http://example.com",
            ApiKey = "123",
        };
        var mockHttpClient = new Mock<IHttpClientWrapper>();

        var expectedMovie = new Movie
        {
            imdbID = "tt1234567",
            Title = "Test Movie",
        };
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        string content = JsonSerializer.Serialize(expectedMovie, jsonOptions);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(content),
        };
        mockHttpClient.Setup(x => x.GetAsync($"{appSettings.Api}?i={expectedMovie.imdbID}&apikey={appSettings.ApiKey}"))
            .ReturnsAsync(response);

        var movieService = new MovieService(appSettings, mockHttpClient.Object);

        // Act
        Movie? actualMovie = await movieService.GetMovieById(expectedMovie.imdbID);

        // Assert
        Assert.NotNull(actualMovie);
        Assert.Equal(expectedMovie.imdbID, actualMovie.imdbID);
        Assert.Equal(expectedMovie.Title, actualMovie.Title);
    }


    [Fact]
    public async Task SearchHistory_RemovesOldestMovieFromHistory_WhenMaxHistoryIsReached()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            Api = "http://example.com",
            ApiKey = "123",
        };

        var movie1 = new Movie
        {
            Title = "Test Movie 1",
        };
        var movie2 = new Movie
        {
            Title = "Test Movie 2",
        };
        var movie3 = new Movie
        {
            Title = "Test Movie 3",
        };
        var movie4 = new Movie
        {
            Title = "Test Movie 4",
        };
        var movie5 = new Movie
        {
            Title = "Test Movie 5",
        };
        var movie6 = new Movie
        {
            Title = "Test Movie 6",
        };

        var mockHttpClient = new Mock<IHttpClientWrapper>();

        mockHttpClient.Setup(x => x.GetAsync($"{appSettings.Api}?t={movie6.Title}?&apikey={appSettings.ApiKey}"))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));


        var movieService = new MovieService(appSettings, mockHttpClient.Object);

        // Act
        await movieService.SearchHistory(movie1);
        await movieService.SearchHistory(movie2);
        await movieService.SearchHistory(movie3);
        await movieService.SearchHistory(movie4);
        await movieService.SearchHistory(movie5);
        List<Movie> searchHistory = await movieService.SearchHistory(movie6);

        // Assert
        Assert.Equal(ReelReflectionTest.MaxHistory, searchHistory.Count);
        Assert.DoesNotContain(movie1, searchHistory);
        Assert.Contains(movie2, searchHistory);
        Assert.Contains(movie3, searchHistory);
        Assert.Contains(movie4, searchHistory);
        Assert.Contains(movie5, searchHistory);
        Assert.Contains(movie6, searchHistory);
    }
}
