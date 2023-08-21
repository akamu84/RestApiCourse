namespace Movies.Api.Endpoints.Movies;

public static class MoviesEndpointsExtensions
{
    public static IEndpointRouteBuilder MapMoviesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateMovie();
        app.MapGetMovie();
        app.MapGetAllMovies();
        app.MapUpdateMovie();
        app.MapDeleteMovie();
        return app;
    }
}