using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
    public const string Name = "GetAllMovies";

    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.GetAll,
                async ([AsParameters] GetAllMoviesRequest request, IMovieService movieService, HttpContext context,
                    CancellationToken token) =>
                {
                    var userId = context.GetUserId();
                    var options = request.MapToOptions().WithUser(userId);
                    var movies = await movieService.GetAllAsync(options, token);
                    var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
                    var response = movies.MapToResponse(request.Page.GetValueOrDefault(PagedRequest.DefaultPageSize),
                        request.PageSize.GetValueOrDefault(PagedRequest.DefaultPage), movieCount);
                    return TypedResults.Ok(response);
                })
            .CacheOutput("MovieCache")
            .Produces<MoviesResponse>()
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .WithName($"{Name}V1");

        app.MapGet(ApiEndpoints.Movies.GetAll,
                async ([AsParameters] GetAllMoviesRequest request, IMovieService movieService, HttpContext context,
                    CancellationToken token) =>
                {
                    var userId = context.GetUserId();
                    var options = request.MapToOptions().WithUser(userId);
                    var movies = await movieService.GetAllAsync(options, token);
                    var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
                    var response = movies.MapToResponse(request.Page.GetValueOrDefault(PagedRequest.DefaultPageSize),
                        request.PageSize.GetValueOrDefault(PagedRequest.DefaultPage), movieCount);
                    return TypedResults.Ok(response);
                })
            .CacheOutput("MovieCache")
            .Produces<MoviesResponse>()
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(2.0)
            .WithName($"{Name}V2");

        return app;
    }
}