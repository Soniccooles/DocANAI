using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Repositories.DocumentChunks;
using DocANAI.Persistence.ValueObjects;
using DocANAI.Worker.Infrastructure.Chunking;
using MediatR;

namespace DocANAI.Worker.Features.SplitDocuments;

internal sealed class SplitDocumentsIntoChunksCommandHandler(
    IDocumentChunksRepository chunksRepository,
    IDocumentChunker chunker)
    : IRequestHandler<SplitDocumentsIntoChunksCommand>
{
    public async Task Handle(SplitDocumentsIntoChunksCommand request, CancellationToken ct)
    {
        await chunksRepository.DeleteByTaskIdAsync(request.TaskId, ct);

        var chunks = new List<DocumentChunk>();

        foreach (var document in request.Documents)
        {
            foreach (var textChunk in chunker.Split(document.Text))
            {
                chunks.Add(DocumentChunk.Create(
                    IdOf<DocumentChunk>.New(),
                    document.SourceDocumentId,
                    textChunk.Index,
                    textChunk.Text,
                    pageNumber: null,
                    embedding: null));
            }
        }

        await chunksRepository.AddRangeAsync(chunks, ct);
    }
}