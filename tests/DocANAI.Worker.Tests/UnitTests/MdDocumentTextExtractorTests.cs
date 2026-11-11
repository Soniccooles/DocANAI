using System.Text;
using DocANAI.Worker.Infrastructure.Parsing;

namespace DocANAI.Worker.Tests.UnitTests;

public class MdDocumentTextExtractorTests
{
    [TestCase("")]
    [TestCase("   ")]
    public void extract_empty_or_whitespace_stream_throws_invalid_data_exception(string text)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        
        Assert.ThrowsAsync<InvalidDataException>(
            async () => await MdDocumentTextExtractor.ExtractAsync(stream, CancellationToken.None));
    }
}