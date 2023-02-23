using Microsoft.AspNetCore.Mvc;
using ReelReflection.Features;
using ReelReflection.Model;

namespace ReelReflection.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ReelReflectionController : ControllerBase
{
    private readonly IMovieService _movieService;

    public ReelReflectionController(IMovieService movieService)
    {
        _movieService = movieService;
    }


    [HttpGet("{title}")]
    public async Task<ActionResult<Movie>> GetMovieByTitle(string title)
    {
        return Ok(await _movieService.GetMovieByTitle(title));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movie>> GetMovieById(string id)
    {
        return Ok(await _movieService.GetMovieById(id));
    }

    [HttpGet]
    public async Task<ActionResult<List<Movie>>> SearchHistory()
    {
        return Ok(await _movieService.SearchHistory(null));
    }
}
