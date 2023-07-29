using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;

var services = new ServiceCollection();

services.AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (_, token) =>
            await s.GetRequiredService<AuthTokenProvider>().GetTokenAsync(token)
    })
    .ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5001"));

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

var movie = await moviesApi.GetMovieAsync("nick-the-greek-2023");

var movies = await moviesApi.GetMoviesAsync(new GetAllMoviesRequest
{
    Title = null,
    YearOfRelease = null,
    SortBy = null,
    Page = 1,
    PageSize = 3
});

Console.WriteLine(JsonSerializer.Serialize(movie));
Console.WriteLine(JsonSerializer.Serialize(movies));