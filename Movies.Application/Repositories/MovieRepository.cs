using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO movies (id, slug, title, yearofrelease)
                VALUES (@Id, @Slug, @Title, @YearOfRelease);
            """, movie, transaction, cancellationToken: cancellationToken));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    INSERT INTO genres (movieId, name)
                    VALUES (@MovieId, @Name);                        
                """, new { MovieId = movie.Id, Name = genre }, transaction, cancellationToken: cancellationToken));
            }
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                DELETE FROM genres
                WHERE movieId = @Id;
            """, new { Id = id }, transaction, cancellationToken: cancellationToken));

        var result = await connection.ExecuteAsync(new CommandDefinition("""
               DELETE FROM movies
               WHERE id = @Id;
            """, new { Id = id }, transaction, cancellationToken: cancellationToken));

        transaction.Commit();
        return result > 0;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync(new CommandDefinition("""
                SELECT m.*, STRING_AGG(g.name, ',') AS genres
                FROM movies m LEFT JOIN genres g ON m.id = g.movieId
                GROUP BY m.id;
            """, cancellationToken: cancellationToken));

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
                SELECT id, slug, title, yearofrelease
                FROM movies
                WHERE id = @id;                                 
            """, new { id }, cancellationToken: cancellationToken));

        if (movie is null) return null;

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
                SELECT name
                FROM genres
                WHERE movieId = @Id;
            """, new { movie.Id }, cancellationToken: cancellationToken));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
                SELECT id, slug, title, yearofrelease
                FROM movies
                WHERE slug = @slug;                                 
            """, new { slug }, cancellationToken: cancellationToken));

        if (movie is null) return null;

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
                SELECT name
                FROM genres
                WHERE movieId = @Id;
            """, new { movie.Id }, cancellationToken: cancellationToken));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                DELETE FROM genres
                WHERE movieId = @Id;
            """, new { movie.Id }, transaction, cancellationToken: cancellationToken));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO genres (movieId, name)
                VALUES (@MovieId, @Name);
            """, new { MovieId = movie.Id, Name = genre }, transaction, cancellationToken: cancellationToken));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
               UPDATE movies
               SET slug = @Slug,
               title = @Title,
               yearofrelease = @YearOfRelease
                WHERE id = @Id;
            """, movie, transaction, cancellationToken: cancellationToken));

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                SELECT COUNT(1)
                FROM movies
                WHERE id = @id
            """, new { id }, cancellationToken: cancellationToken));
    }
}