using Id3;

namespace MP3_Tag_Editor;

public class Mp3Manager
{
    public IEnumerable<Mp3> LoadMp3Files(string path)
    {
        var mp3Files = new List<Mp3>();
        
        if (File.Exists(path))
        {
            mp3Files.Add(LoadMp3FileFromPath(path));
        }
        else if (Directory.Exists(path))
        {
            mp3Files = LoadMp3FilesFromDirectory(path).ToList();
        }
        else
        {
            throw new ArgumentException("Invalid path", nameof(path));
        }

        return mp3Files;
    }

    private Mp3 LoadMp3FileFromPath(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        extension = extension.TrimStart('.');
        
        if (extension.Equals("mp3", StringComparison.OrdinalIgnoreCase))
        {
            var mp3File = new Mp3(filePath);
            return mp3File;
        }
        else
        {
            throw new ArgumentException("File is not an MP3", nameof(filePath));
        }
    }

    private IEnumerable<Mp3> LoadMp3FilesFromDirectory(string directoryPath)
    {
        var mp3Files = new List<Mp3>();
        
        var filesPaths = Directory.EnumerateFiles(directoryPath);
        foreach (var filePath in filesPaths)
        {
            try
            {
                var mp3File = LoadMp3FileFromPath(filePath);
                mp3Files.Add(mp3File);
            }
            catch (ArgumentException) { /* Ignored, program proceeds to next file */ }
        }

        return mp3Files;
    }
}