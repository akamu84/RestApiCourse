﻿using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public const string Name = "UpdateMovie";

    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Update,
                async (Guid id, UpdateMovieRequest request, IMovieService movieService,
                    IOutputCacheStore outputCacheStore, HttpContext context,
                    CancellationToken token) =>
                {
                    var userId = context.GetUserId();
                    var movie = request.MapToMovie(id);
                    var updatedMovie = await movieService.UpdateAsync(movie, userId, token);
                    if (updatedMovie is null) return Results.NotFound();
                    await outputCacheStore.EvictByTagAsync("movies", token);
                    var response = movie.MapToResponse();
                    return TypedResults.Ok(response);
                })
            .WithName(Name)
            .Produces<MovieResponse>()
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}