using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Worker.Features.ProcessTask;

public sealed record ProcessTaskCommand(IdOf<ProcessingTask> TaskId)
    : IRequest;
