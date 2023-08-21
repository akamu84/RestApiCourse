namespace Movies.Api.Endpoints.Ratings;

public static class RatingsEndpointsExtensions
{
    public static IEndpointRouteBuilder MapRatingsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRateMovie();
        app.MapDeleteRating();
        app.MapGetUserRatings();
        return app;
    }
}