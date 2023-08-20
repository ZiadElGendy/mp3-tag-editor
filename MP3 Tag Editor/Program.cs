using System.Globalization;
using MP3_Tag_Editor.Enums;
using TagLib.Tiff.Arw;

namespace MP3_Tag_Editor;

internal static class Program
{
    private static void Main(string[] args)
    {
        const string testingDirectory = @"YOUR DIR";
        var mp3Files = Mp3FileManager.LoadMp3Files(testingDirectory);
        
        var modifiedFiles = Mp3TagsManager.ModifyMp3Tags(mp3Files, Mp3Tag.AlbumArtists, "00:01:30");
        Mp3FileManager.SaveNewMp3File(modifiedFiles, testingDirectory, true);
    }
}