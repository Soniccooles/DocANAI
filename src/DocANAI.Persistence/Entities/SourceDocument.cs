using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.Entities.Users;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities;

public sealed class SourceDocument : AuditableEntity<IdOf<SourceDocument>>
{
    public IdOf<User> UserId { get; private set; }
    public IdOf<ProcessingTask> TaskId { get; private set; }
    public string Extension { get; private set; }
    public string Filename { get; private set; }
    public string FilePath { get; private set; }
    public int Size { get; private set; }

    private SourceDocument(
        IdOf<SourceDocument> id,
        IdOf<User> userId,
        IdOf<ProcessingTask> taskId,
        string extension,
        string filename,
        string filePath,
        int size)
    {
        Id = id;
        UserId = userId;
        TaskId = taskId;
        Extension = extension;
        Filename = filename;
        FilePath = filePath;
        Size = size;
    }

    public static SourceDocument Create(
        IdOf<SourceDocument> id,
        IdOf<User> userId,
        IdOf<ProcessingTask> taskId,
        string extension,
        string filename,
        string filePath,
        int size)
        => new(id, userId, taskId, extension, filename, filePath, size);

    [UsedImplicitly]
    #pragma warning disable CS8618
    private SourceDocument() { }
    #pragma warning restore CS8618
}