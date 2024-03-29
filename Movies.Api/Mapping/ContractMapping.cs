﻿using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse MapToResponse(this Movie movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Slug = movie.Slug,
            Title = movie.Title,
            Rating = movie.Rating,
            UserRating = movie.UserRating,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres.ToList()
        };
    }

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies, int page, int pageSize, int totalCount)
    {
        return new MoviesResponse
        {
            Items = movies.Select(movie => movie.MapToResponse()),
            Page = page,
            PageSize = pageSize,
            Total = totalCount
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieRatingResponse MapToResponse(this MovieRating movieRating)
    {
        return new MovieRatingResponse
        {
            MovieId = movieRating.MovieId,
            Rating = movieRating.Rating,
            Slug = movieRating.Slug
        };
    }

    public static MovieRatingsResponse MapToResponse(this IEnumerable<MovieRating> movieRatings)
    {
        return new MovieRatingsResponse
        {
            Items = movieRatings.Select(mr => mr.MapToResponse())
        };
    }

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOptions
        {
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            SortField = request.SortBy?.TrimStart('+', '-'),
            SortOrder = request.SortBy is null ? SortOrder.Unsorted :
                request.SortBy.StartsWith('-') ? SortOrder.Descending :
                SortOrder.Ascending,
            Page = request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
            PageSize = request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize)
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}