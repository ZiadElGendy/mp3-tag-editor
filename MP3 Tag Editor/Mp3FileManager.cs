using MP3_Tag_Editor.Exceptions;
using File = System.IO.File;

namespace MP3_Tag_Editor;

/// <summary>
///     Provides methods for managing and loading MP3 files.
/// </summary>
public static class Mp3FileManager
{
    private const string Mp3TestingDirectory =
        @"D:\Documents\MP3DIRECTORY"; //Change this to your own directory

    /// <summary>
    ///     Loads MP3 files from the given path.
    ///     If the path is a single file, it will load the MP3 file.
    ///     If the path is a directory, it will load all MP3 files in the directory.
    /// </summary>
    /// <param name="path">The path of the file or directory.</param>
    /// <returns>A collection of loaded <see cref="TagLib"/> <see cref="TagLib.File"/> MP3 files.</returns>
    public static IEnumerable<TagLib.File> LoadMp3Files(string path)
    {
        if (File.Exists(path))
            return new List<TagLib.File> { LoadMp3FileFromPath(path) };
        if (Directory.Exists(path))
            return LoadMp3FilesFromDirectory(path);
        throw new InvalidMp3PathException(path);
    }

    /// <summary>
    ///     Loads an MP3 file from the specified path.
    /// </summary>
    /// <param name="filePath">The path of the MP3 file.</param>
    /// <returns>The loaded <see cref="TagLib"/> <see cref="TagLib.File"/> MP3 file.</returns>
    private static TagLib.File LoadMp3FileFromPath(string filePath)
    {
        if (!IsMp3FileExtensionValid(filePath))
            throw new InvalidMp3PathException(filePath);
        return TagLib.File.Create(filePath);
    }

    /// <summary>
    ///     Checks if the file extension is a valid MP3 extension.
    /// </summary>
    /// <param name="filePath">The path of the file.</param>
    /// <returns><c>true</c> if the extension is valid; otherwise, <c>false</c>.</returns>
    private static bool IsMp3FileExtensionValid(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        extension = extension.TrimStart('.');

        return extension.Equals("mp3", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Loads MP3 files from a directory.
    /// </summary>
    /// <param name="directoryPath">The path of the directory containing MP3 files.</param>
    /// <returns>A collection of <see cref="TagLib"/> <see cref="TagLib.File"/> MP3 files.</returns>
    private static IEnumerable<TagLib.File> LoadMp3FilesFromDirectory(string directoryPath)
    {
        var filesPaths = Directory.EnumerateFiles(directoryPath, "*.mp3");

        return filesPaths.Select(LoadMp3FileFromPath).ToList();
    }

    /// <summary>
    ///     Disposes and frees the MP3 files in the given list.
    /// </summary>
    /// <param name="mp3Files">The list of <see cref="TagLib"/> <see cref="TagLib.File"/> MP3 files to dispose of.</param>
    public static void DisposeMp3Files(IEnumerable<TagLib.File> mp3Files)
    {
        foreach (var mp3File in mp3Files)
        {
            mp3File.Dispose();
        }
    }

    public static void SaveOverwriteMp3File(IEnumerable<TagLib.File> mp3Files)
    {
        foreach (var mp3File in mp3Files)
        {
            mp3File.Save();
        }
    }

    //FIXME: Make this not stupid and backwards
    public static void SaveNewMp3File(IEnumerable<TagLib.File> mp3Files, string directoryFilePath)
    {
        foreach (var mp3File in mp3Files)
        {
            var fileName = mp3File.Name;
            var newFileName = Path.GetFileNameWithoutExtension(fileName) + " - Original" + Path.GetExtension(fileName);

            File.Copy(Path.Combine(directoryFilePath,fileName),Path.Combine(directoryFilePath,newFileName) , false);
            mp3File.Save();
        }
    }


    public static void ReadWriteTest()
    {
        var mp3Files = LoadMp3Files(Mp3TestingDirectory);

        foreach (var mp3File in mp3Files)
        {
            Console.WriteLine(mp3File.Tag.Title);
        }

        DisposeMp3Files(mp3Files);

        SaveNewMp3File(mp3Files, Mp3TestingDirectory);

    }

}