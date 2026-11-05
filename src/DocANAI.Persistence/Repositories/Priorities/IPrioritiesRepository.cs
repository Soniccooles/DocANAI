using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.Priorities;

namespace DocANAI.Persistence.Repositories.Priorities;

public interface IPrioritiesRepository
{
    Task<Maybe<Priority>> GetByLevelAsync(string levelName, CancellationToken ct = default);
}