using ReelReflection.Model;

namespace ReelReflection.Features;

public interface IMovieService
{
    public Task<Movie?> GetMovieByTitle(string title);
    public Task<Movie?> GetMovieById(string id);
    public Task<List<Movie>> SearchHistory(Movie movie);
}
