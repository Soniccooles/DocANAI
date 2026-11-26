using DocANAI.Persistence.Context;
using DocANAI.Persistence.Context.Options;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.Priorities;
using DocANAI.Persistence.Seeder;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.PostgreSql;

namespace DocANAI.Worker.Tests.IntegrationTests;

public abstract class PostgresTestBase
{
    protected static readonly IdOf<AIModel> SeededModelId =
        IdOf<AIModel>.From(Guid.Parse("99c6eae3-f2f4-400d-a02a-7fc5c76844f3"));

    protected static readonly IdOf<Priority> SeededPriorityId =
        IdOf<Priority>.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("pgvector/pgvector:pg17").Build();

    protected string ConnectionString { get; private set; } = string.Empty;

    protected PostgreSqlDbContext CreateDbContext() =>
        new(DbContextOptionsFactory.CreateOptions<PostgreSqlDbContext>(ConnectionString));

    [OneTimeSetUp]
    public async Task StartDatabase()
    {
        await _postgres.StartAsync();
        ConnectionString = _postgres.GetConnectionString();

        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();
        await new DatabaseSeeder(db, NullLogger<DatabaseSeeder>.Instance).SeedAsync("Development");
    }

    [OneTimeTearDown]
    public async Task StopDatabase() => await _postgres.DisposeAsync();
}