using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.FileStorage;
using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.Entities.Users;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.UploadFiles;

public sealed record UploadFilesCommand(IdOf<ProcessingTask> TaskId, IdOf<User> UserId, IFormFile File, bool IsQuestionFile)
    : IRequest<Result<UploadFileResponse, string>>;