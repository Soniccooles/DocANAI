using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.Entities.Users;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities;

public sealed class QuestionFile : AuditableEntity<IdOf<QuestionFile>>
{
    public IdOf<User> UserId { get; private set; }
    public IdOf<ProcessingTask> TaskId { get; private set; }
    public string Extension { get; private set; }
    public string Filename { get; private set; }
    public string FilePath { get; private set; }

    private QuestionFile(
        IdOf<QuestionFile> id,
        IdOf<User> userId,
        IdOf<ProcessingTask> taskId,
        string extension,
        string filename,
        string filePath)
    {
        Id = id;
        UserId = userId;
        TaskId = taskId;
        Extension = extension;
        Filename = filename;
        FilePath = filePath;
    }

    public static QuestionFile Create(
        IdOf<QuestionFile> id,
        IdOf<User> userId,
        IdOf<ProcessingTask> taskId,
        string extension,
        string filename,
        string filePath)
        => new(id, userId, taskId, extension, filename, filePath);

    [UsedImplicitly]
    #pragma warning disable CS8618
    private QuestionFile() { }
    #pragma warning restore CS8618
}