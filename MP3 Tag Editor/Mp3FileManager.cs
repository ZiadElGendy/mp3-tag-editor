using Id3;
using MP3_Tag_Editor.Exceptions;

namespace MP3_Tag_Editor;

/// <summary>
///     Provides methods for managing and loading MP3 files.
/// </summary>
public static class Mp3FileManager
{
    /// <summary>
    ///     Loads <see cref="Mp3"/> files from the given path.
    ///     If the path is a single file, it will load the <see cref="Mp3"/> file.
    ///     If the path is a directory, it will load all <see cref="Mp3"/> files in the directory.
    /// </summary>
    /// <param name="path">The path of the file or directory.</param>
    /// <returns>A collection of loaded <see cref="Mp3"/> files.</returns>
    public static IEnumerable<Mp3> LoadMp3Files(string path)
    {
        if (File.Exists(path))
            return new List<Mp3> { LoadMp3FileFromPath(path) };
        if (Directory.Exists(path))
            return LoadMp3FilesFromDirectory(path);
        throw new InvalidMp3PathException(path);
    }

    /// <summary>
    ///     Loads an <see cref="Mp3"/> file from the specified path.
    /// </summary>
    /// <param name="filePath">The path of the <see cref="Mp3"/> file.</param>
    /// <returns>The loaded <see cref="Mp3"/> file.</returns>
    private static Mp3 LoadMp3FileFromPath(string filePath)
    {
        if (!IsMp3FileExtensionValid(filePath))
            throw new InvalidMp3PathException(filePath);
        return new Mp3(filePath);
    }

    /// <summary>
    ///     Checks if the file extension is a valid <see cref="Mp3"/> extension.
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
    ///     Loads <see cref="Mp3"/> files from a directory.
    /// </summary>
    /// <param name="directoryPath">The path of the directory containing <see cref="Mp3"/> files.</param>
    /// <returns>A collection of loaded <see cref="Mp3"/> files.</returns>
    private static IEnumerable<Mp3> LoadMp3FilesFromDirectory(string directoryPath)
    {
        var filesPaths = Directory.EnumerateFiles(directoryPath, "*.mp3");

        return filesPaths.Select(LoadMp3FileFromPath).ToList();
    }
}