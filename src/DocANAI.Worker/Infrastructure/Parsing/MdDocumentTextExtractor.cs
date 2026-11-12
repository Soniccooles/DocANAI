using System.Text;

namespace DocANAI.Worker.Infrastructure.Parsing;

internal class MdDocumentTextExtractor
{
    public static async Task<string> ExtractAsync(Stream content, CancellationToken ct)
    {
        byte[] bytes = await TextDocumentDecoder.ReadAllBytesAsync(content, ct);

        (Encoding encoding, int bomLength) = TextDocumentDecoder.DefineEncoding(bytes);
        
        TextDocumentDecoder.ValidateFile(bytes, encoding);

        string result = TextDocumentDecoder.DecodeAndNormalize(bytes, encoding, bomLength);

        if (string.IsNullOrWhiteSpace(result)) throw new InvalidDataException("Файл пуст");

        return result.Trim();
    }
}