using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.DocumentChunks;

public interface IDocumentChunksRepository
{
    Task ReplaceByTaskIdAsync(IdOf<ProcessingTask> taskId, IReadOnlyCollection<DocumentChunk> chunks, CancellationToken ct = default);
}