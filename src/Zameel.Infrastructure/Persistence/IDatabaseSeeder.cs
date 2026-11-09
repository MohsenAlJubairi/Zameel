namespace Zameel.Infrastructure.Persistence;

public interface IDatabaseSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
