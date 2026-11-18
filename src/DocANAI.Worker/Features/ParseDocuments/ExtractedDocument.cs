using DocANAI.Persistence.Entities;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Worker.Features.ParseDocuments;

public sealed record ExtractedDocument(
    IdOf<SourceDocument> SourceDocumentId,
    string Filename,
    string Text
    );