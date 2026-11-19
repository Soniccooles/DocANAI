using DocANAI.Persistence.Repositories.SourceDocuments;
using DocANAI.Worker.Infrastructure.Parsing;
using DocANAI.Worker.Infrastructure.Storage;
using MediatR;

namespace DocANAI.Worker.Features.ParseDocuments;

internal sealed class ExtractTextFromDocumentsQueryHandler(
    ISourceDocumentsRepository sourceDocumentsRepository,
    IObjectStorageService storageService,
    IDocumentTextExtractor textExtractor
    ) : IRequestHandler<ExtractTextFromDocumentsQuery, IReadOnlyList<ExtractedDocument>>
{
    public async Task<IReadOnlyList<ExtractedDocument>> Handle(ExtractTextFromDocumentsQuery request, CancellationToken ct)
    {
        var sourceDocuments = await sourceDocumentsRepository.GetByTaskIdAsync(request.TaskId, ct);
        if (sourceDocuments.Count == 0) throw new InvalidOperationException("No source documents found for task");

        var resultDocuments = new List<ExtractedDocument>(sourceDocuments.Count);
        
        foreach (var document in sourceDocuments)
        {
            await using var stream = await storageService.DownloadAsync(document.FilePath, ct);
            var text = await textExtractor.ExtractAsync(document.Extension, stream, ct);

            resultDocuments.Add(new ExtractedDocument(document.Id, document.Filename, text));
        }

        return resultDocuments;
    }
}