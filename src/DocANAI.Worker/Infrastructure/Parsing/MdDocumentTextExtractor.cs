using System.Text;

namespace DocANAI.Worker.Infrastructure.Parsing;

public class MdDocumentTextExtractor
{
    public static Task<string> ExtractAsync(Stream content, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        using var memoryStream = new MemoryStream();
        content.CopyTo(memoryStream);
        byte[] bytes = memoryStream.ToArray();

        (Encoding encoding, int bomLength) = TextDocumentDecoder.DefineEncoding(bytes);
        
        TextDocumentDecoder.ValidateFile(bytes, encoding);

        string result = TextDocumentDecoder.DecodeAndNormalize(bytes, encoding, bomLength);

        if (string.IsNullOrWhiteSpace(result)) throw new InvalidDataException("Файл пуст");
        
        return Task.FromResult(result.Trim());
    }
}