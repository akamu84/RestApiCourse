using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";

    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Ratings.GetUserRatings,
                async (IRatingService ratingService, HttpContext context,
                    CancellationToken token) =>
                {
                    var userId = context.GetUserId();
                    var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
                    var ratingsResponse = ratings.MapToResponse();
                    return TypedResults.Ok(ratingsResponse);
                })
            .WithName(Name)
            .Produces<MovieRatingsResponse>()
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization();

        return app;
    }
}