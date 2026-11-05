using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;

namespace DocANAI.Persistence.Entities.ProcessingTasks;

public sealed class ProcessingTask : AuditableEntity<IdOf<ProcessingTask>>
{
    public IdOf<Users.User> UserId { get; private set; }
    public IdOf<AIModel> ModelId { get; private set; }
    public IdOf<Priorities.Priority> PriorityId { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }

    private ProcessingTask(
        IdOf<ProcessingTask> id,
        IdOf<Users.User> userId,
        IdOf<AIModel> modelId,
        IdOf<Priorities.Priority> priorityId)
    {
        Id = id;
        UserId = userId;
        ModelId = modelId;
        PriorityId = priorityId;
        Status = TaskStatus.InQueue;
    }

    public static ProcessingTask Create(
        IdOf<ProcessingTask> id,
        IdOf<Users.User> userId,
        IdOf<AIModel> modelId,
        IdOf<Priorities.Priority> priorityId)
        => new(id, userId, modelId, priorityId);

    public void Start()
    {
        Status = TaskStatus.Processing;
        StartTime = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = TaskStatus.Done;
        EndTime = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = TaskStatus.Failed;
        EndTime = DateTime.UtcNow;
    }

    [UsedImplicitly]
#pragma warning disable CS8618
    private ProcessingTask() { }
#pragma warning restore CS8618
}