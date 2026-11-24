using DocANAI.Persistence.Entities.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;
using DocANAI.Worker.Features.ParseDocuments;
using MediatR;

namespace DocANAI.Worker.Features.SplitDocuments;

public sealed record SplitDocumentsIntoChunksCommand(
    IdOf<ProcessingTask> TaskId,
    IReadOnlyList<ExtractedDocument> Documents)
    : IRequest;