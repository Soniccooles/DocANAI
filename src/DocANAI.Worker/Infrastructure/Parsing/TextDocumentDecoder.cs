using System.Text;

namespace DocANAI.Worker.Infrastructure.Parsing;

internal static class TextDocumentDecoder
{
    static TextDocumentDecoder()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static (Encoding encoding, int bomLength) DefineEncoding(byte[] content)
    {
        if (content.Length >= 2 && content[0] == 0xFF && content[1] == 0xFE)
        {
            return (Encoding.Unicode, 2);
        }
        if (content.Length >= 2 && content[0] == 0xFE && content[1] == 0xFF)
        {
            return (Encoding.BigEndianUnicode, 2);
        }
        if (content.Length >= 3 && content[0] == 0xEF && content[1] == 0xBB && content[2] == 0xBF)
        {
            return (Encoding.UTF8, 3);
        }

        try
        {
            var strictUtf8 = new UTF8Encoding(false, throwOnInvalidBytes: true);
            strictUtf8.GetCharCount(content);
            return (Encoding.UTF8, 0);
        } catch (DecoderFallbackException) {
            return (Encoding.GetEncoding(1251), 0);
        }
    }

    public static void ValidateFile(byte[] content, Encoding encoding)
    {
        if (encoding.CodePage == Encoding.Unicode.CodePage || encoding.CodePage == Encoding.BigEndianUnicode.CodePage)
        {
            return;
        }
        
        if (Array.IndexOf(content, (byte)0) >= 0)
        {
            throw new InvalidDataException("Файл не является текстовым");
        }
    }

    public static string DecodeAndNormalize(byte[] content, Encoding encoding, int bomLength)
    {
        return encoding.GetString(content, bomLength, content.Length - bomLength).ReplaceLineEndings("\n");
    }
}