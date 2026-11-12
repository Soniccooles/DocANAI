using System.Text;
using DocANAI.Worker.Infrastructure.Parsing;
 
namespace DocANAI.Worker.Tests.UnitTests;

public class TextDocumentDecoderTests
{
    [TestCase(new byte[] { 0xFF, 0xFE, 0x41, 0x00 }, "utf-16", 2)] // UTF-16 LE с BOM
    [TestCase(new byte[] { 0xFE, 0xFF, 0x00, 0x41 }, "utf-16BE", 2)] // UTF-16 BE с BOM
    [TestCase(new byte[] { 0xEF, 0xBB, 0xBF, 0xD1, 0x81 }, "utf-8", 3)] // UTF-8 с BOM
    [TestCase(new byte[] { 0xD1, 0x81, 0xD0, 0xBB, 0xD0, 0xBE, 0xD0, 0xB2, 0xD0, 0xBE }, "utf-8", 0)] // UTF-8 без BOM
    [TestCase(new byte[] { 0xF1, 0xEB, 0xEE, 0xE2, 0xEE }, "windows-1251", 0)] // cp1251
    [TestCase(new byte[] { 0x68, 0x65, 0x6C, 0x6C, 0x6F }, "utf-8", 0)] // латиница → UTF-8
    [TestCase(new byte[] { }, "utf-8", 0)] // пустой массив, не падает
    [TestCase(new byte[] { 0x41 }, "utf-8", 0)] // один байт, не падает
    public void define_encoding_returns_expected_encoding_and_bom_length(
        byte[] content, string expectedEncodingName, int expectedBomLength)
    {
        var (encoding, bomLength) = TextDocumentDecoder.DefineEncoding(content);
 
        Assert.That(encoding.WebName, Is.EqualTo(expectedEncodingName));
        Assert.That(bomLength, Is.EqualTo(expectedBomLength));
    }
    
    [TestCase(new byte[] { 0xD1, 0x81, 0xD0, 0xBB, 0xD0, 0xBE, 0xD0, 0xB2, 0xD0, 0xBE }, "слово")] // UTF-8 без BOM
    [TestCase(new byte[] { 0xEF, 0xBB, 0xBF, 0xD1, 0x81, 0xD0, 0xBB, 0xD0, 0xBE, 0xD0, 0xB2, 0xD0, 0xBE }, "слово")] // UTF-8 с BOM
    [TestCase(new byte[] { 0xFF, 0xFE, 0x41, 0x04, 0x3B, 0x04, 0x3E, 0x04, 0x32, 0x04, 0x3E, 0x04 }, "слово")] // UTF-16 LE с BOM
    [TestCase(new byte[] { 0xFE, 0xFF, 0x04, 0x41, 0x04, 0x3B, 0x04, 0x3E, 0x04, 0x32, 0x04, 0x3E }, "слово")] // UTF-16 BE с BOM
    [TestCase(new byte[] { 0xF1, 0xEB, 0xEE, 0xE2, 0xEE }, "слово")] // cp1251
    public void decode_returns_correct_text(byte[] content, string expectedText)
    {
        var (encoding, bomLength) = TextDocumentDecoder.DefineEncoding(content);
 
        var result = TextDocumentDecoder.DecodeAndNormalize(content, encoding, bomLength);
 
        Assert.That(result, Is.EqualTo(expectedText));
    }
    
    [TestCase("a\r\nb", "a\nb")]
    [TestCase("a\rb", "a\nb")]
    [TestCase("a\nb", "a\nb")]
    [TestCase("a\r\n\r\nb", "a\n\nb")]
    public void decode_and_normalize_replaces_line_endings_with_lf(string input, string expected)
    {
        byte[] content = Encoding.UTF8.GetBytes(input);
 
        var result = TextDocumentDecoder.DecodeAndNormalize(content, Encoding.UTF8, 0);
 
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void validate_file_throws_if_null_byte_found_in_non_utf16()
    {
        byte[] content = [0x41, 0x00, 0x42];
 
        Assert.Throws<InvalidDataException>(() => TextDocumentDecoder.ValidateFile(content, Encoding.UTF8));
    }
 
    [TestCase("utf-16")]
    [TestCase("utf-16BE")]
    public void validate_file_does_not_throw_for_utf16_with_null_bytes(string encodingName)
    {
        byte[] content = [0x41, 0x00];
        var encoding = Encoding.GetEncoding(encodingName);
 
        Assert.DoesNotThrow(() => TextDocumentDecoder.ValidateFile(content, encoding));
    }
 
    [Test]
    public void validate_file_does_not_throw_for_text_without_null_bytes()
    {
        byte[] content = Encoding.UTF8.GetBytes("hello");
 
        Assert.DoesNotThrow(() => TextDocumentDecoder.ValidateFile(content, Encoding.UTF8));
    }
    
    [Test]
    public void utf16_without_bom_latin_is_rejected_as_not_text()
    {
        byte[] content = [0x41, 0x00, 0x42, 0x00];
        var (encoding, _) = TextDocumentDecoder.DefineEncoding(content);
 
        Assert.Throws<InvalidDataException>(() => TextDocumentDecoder.ValidateFile(content, encoding));
    }
 
    [Test]
    public void utf16_without_bom_cyrillic_is_not_rejected_but_text_is_garbled()
    {
        byte[] content = [0x41, 0x04, 0x3B, 0x04, 0x3E, 0x04, 0x32, 0x04, 0x3E, 0x04];
        var (encoding, bomLength) = TextDocumentDecoder.DefineEncoding(content);
 
        Assert.DoesNotThrow(() => TextDocumentDecoder.ValidateFile(content, encoding));
        Assert.That(TextDocumentDecoder.DecodeAndNormalize(content, encoding, bomLength), Is.Not.EqualTo("слово"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task read_all_bytes_returns_stream_content(bool wrapInBufferedStream)
    {
        byte[] expected = [0x68, 0x65, 0x6C, 0x6C, 0x6F];
        Stream stream = new MemoryStream(expected);
        if (wrapInBufferedStream) stream = new BufferedStream(stream);
        var result = await TextDocumentDecoder.ReadAllBytesAsync(stream, CancellationToken.None);
        Assert.That(result, Is.EqualTo(expected));
    }
}