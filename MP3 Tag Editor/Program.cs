using System.Globalization;

namespace MP3_Tag_Editor;

internal static class Program
{
    public static CultureInfo CultureInfo = new("en-UK");
    private static void Main(string[] args)
    {
        Mp3Manager.ModifyMp3TagsTest();
    }
}