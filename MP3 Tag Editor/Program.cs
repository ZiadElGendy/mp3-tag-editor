using System.Globalization;
using MP3_Tag_Editor.Enums;
using TagLib.Tiff.Arw;

namespace MP3_Tag_Editor;

internal static class Program
{
    private static void Main(string[] args)
    {
        const string testingDirectory = @"D:\Projects\Programming\Team Projects\BMZYY\mp3-tag-editor\Sample MP3s - Copy";
        
        // Using string for tags
        // Mp3TagsManager.ModifyMp3Tags(testingDirectory, "album artists", "Sans", true);

        // Using enum for tags
        Mp3TagsManager.ModifyMp3Tags(testingDirectory, Mp3Tag.AlbumArtists, "00:01:30", true);
    }
}