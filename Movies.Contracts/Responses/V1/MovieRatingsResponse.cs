namespace Movies.Contracts.Responses.V1;

public class MovieRatingsResponse
{
    public required IEnumerable<MovieRatingResponse> Items { get; init; } = Enumerable.Empty<MovieRatingResponse>();
}
