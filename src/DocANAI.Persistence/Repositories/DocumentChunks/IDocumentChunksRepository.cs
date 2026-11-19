using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.DocumentChunks;

public interface IDocumentChunksRepository
{
    Task AddRangeAsync(IEnumerable<DocumentChunk> chunks, CancellationToken ct = default);
    Task DeleteByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default);
}