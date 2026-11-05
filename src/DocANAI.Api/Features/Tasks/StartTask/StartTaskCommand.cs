using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.Entities.Users;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Tasks.StartTask;

public sealed record StartTaskCommand(IdOf<ProcessingTask> TaskId, IdOf<User> UserId)
    : IRequest<Result<StartTaskResponse, string>>;
