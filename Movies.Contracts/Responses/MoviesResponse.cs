using System.ComponentModel.DataAnnotations;

namespace Movies.Contracts.Responses;

public class MoviesResponse
{
    [Required]
    public IEnumerable<MovieResponse> Items { get; init; } = Enumerable.Empty<MovieResponse>();
}
