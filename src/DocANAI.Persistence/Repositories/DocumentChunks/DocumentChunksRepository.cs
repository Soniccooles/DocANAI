using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.DocumentChunks;

[Repository]
public class DocumentChunksRepository(PostgreSqlDbContext dbContext)
    : BaseRepository<DocumentChunk, IdOf<DocumentChunk>>(dbContext), IDocumentChunksRepository
{
    public Task DeleteByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default)
        => DbContext.DocumentChunks
            .Where(c => DbContext.SourceDocuments
                .Any(d => d.Id == c.SourceDocumentId && d.TaskId == taskId))
            .ExecuteDeleteAsync(ct);
}