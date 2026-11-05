using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Worker.Features.ParseDocuments;

public sealed record ExtractTextFromDocumentsQuery(IdOf<ProcessingTask> TaskId)
    : IRequest<string>;