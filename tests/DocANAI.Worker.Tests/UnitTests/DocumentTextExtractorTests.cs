using System.Text;
using DocANAI.Worker.Infrastructure.Parsing;

namespace DocANAI.Worker.Tests.UnitTests;

public class DocumentTextExtractorTests
{
    [TestCase("txt")]
    [TestCase(".TXT")]
    [TestCase("md")]
    [TestCase(".md")]
    public async Task extract_supported_text_extension_returns_text(string extension)
    {
        var extractor = new DocumentTextExtractor();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("hello"));

        var result = await extractor.ExtractAsync(extension, stream);

        Assert.That(result, Is.EqualTo("hello"));
    }
}