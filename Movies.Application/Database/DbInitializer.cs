using Dapper;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
                    CREATE TABLE IF NOT EXISTS movies (
                        id UUID PRIMARY KEY,
                        slug TEXT NOT NULL,
                        title TEXT NOT NULL,
                        yearofrelease INTEGER NOT NULL
                    );
                """);

        await connection.ExecuteAsync("""
                    CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS movies_slug_idx ON movies USING BTREE(slug);
                """);

        await connection.ExecuteAsync("""
                    CREATE TABLE IF NOT EXISTS genres (
                        movieId UUID REFERENCES movies (Id),
                        name TEXT NOT NULL
                    );
                """);

        await connection.ExecuteAsync("""
                    CREATE TABLE IF NOT EXISTS ratings (
                        userid UUID NOT NULL,
                        movieId UUID REFERENCES movies (Id),
                        rating INTEGER NOT NULL,
                        PRIMARY KEY (userid, movieId)
                    );
                """);
    }
}