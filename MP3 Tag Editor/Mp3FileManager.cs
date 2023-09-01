using Microsoft.VisualBasic.FileIO;
using MP3_Tag_Editor.Exceptions;
using File = System.IO.File;

namespace MP3_Tag_Editor;

/// <summary>
/// Provides methods for managing and loading MP3 files.
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
    /// <exception cref="InvalidMp3PathException">Thrown when the specified MP3s do not exist. </exception>
    public static IEnumerable<TagLib.File> LoadMp3Files(string path)
    {
        if (File.Exists(path))
            return new List<TagLib.File> { LoadMp3FileFromPath(path) };
        if (Directory.Exists(path))
            return LoadMp3FilesFromDirectory(path);
        throw new InvalidMp3PathException(path);
    }

    /// <summary>
    ///     Loads new MP3 files from the given path.
    ///     If the path is a single file, it will copy and load the new MP3 file.
    ///     If the path is a directory, it will copy and load all new MP3 files in the directory.
    /// </summary>
    /// <param name="path">The path of the file or directory.</param>
    /// <returns>A collection of loaded <see cref="TagLib"/> <see cref="TagLib.File"/> MP3 files.</returns>
    /// <exception cref="InvalidMp3PathException">Thrown when the specified MP3s do not exist. </exception>
    public static IEnumerable<TagLib.File> LoadNewMp3Files(string path)
    {
        if (File.Exists(path))
        {
            var newPath = CopyMp3File(path);
            return new List<TagLib.File> { LoadMp3FileFromPath(newPath) };
        }

        if (Directory.Exists(path))
        {
            CopyMp3FilesInDirectory(path);
            return LoadNewMp3FilesFromDirectory(path);
        }
        //The design for File vs Directory are different because it was simpler that way
        //I chose to repeat some code rather than have one function do two things depending on a boolean input

        throw new InvalidMp3PathException(path);
    }

    /// <summary>
    ///     Makes a copy of the MP3 file in the given path.
    /// </summary>
    /// <param name="path">The path of the file or directory. </param>
    /// <returns>The path of the MP3 file copy,</returns>
    private static string CopyMp3File(string path)
    {
        var newFileName = Path.GetFileNameWithoutExtension(path) + " - Copy" + Path.GetExtension(path);
        File.Copy(path, newFileName, true);

        return newFileName;
    }

    /// <summary>
    ///     Makes a copy of the MP3 files in the given directory.
    /// </summary>
    /// <param name="path">The path of the file or directory.</param>
    private static void CopyMp3FilesInDirectory(string path)
    {
        var filesPaths = Directory.EnumerateFiles(path, "*.mp3");

        foreach (var filePath in filesPaths)
        {
            var newFileName = path + @"\" + Path.GetFileNameWithoutExtension(filePath) + " - Copy" + Path.GetExtension(filePath);
            File.Copy(filePath, newFileName, true);
        }
    }

    /// <summary>
    ///     Loads an MP3 file from the specified path.
    /// </summary>
    /// <param name="filePath">The path of the MP3 file.</param>
    /// <returns>The loaded <see cref="TagLib"/> <see cref="TagLib.File"/> MP3 file.</returns>
    /// <exception cref="InvalidMp3PathException">Thrown when the specified MP3 does not exist. </exception>
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
    ///     Loads copied MP3 files from a directory.
    /// </summary>
    /// <param name="directoryPath">The path of the directory containing MP3 files.</param>
    /// <returns>A collection of <see cref="TagLib"/> <see cref="TagLib.File"/> MP3 files.</returns>
    private static IEnumerable<TagLib.File> LoadNewMp3FilesFromDirectory(string directoryPath)
    {
        var filesPaths = Directory.EnumerateFiles(directoryPath, "* - Copy.mp3");

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

    /// <summary>
    /// Saves changes made to the given MP3 files.
    /// </summary>
    /// <param name="mp3Files">
    /// The collection of <see cref="TagLib"/> <see cref="TagLib.File"/> MP3 files to save.
    /// </param>
    public static void SaveMp3Files(IEnumerable<TagLib.File> mp3Files)
    {
        foreach (var mp3File in mp3Files)
        {
            mp3File.Save();
        }
    }

    public static void ReadWriteTest()
    {
        var mp3Files = LoadMp3Files(Mp3TestingDirectory);

        foreach (var mp3File in mp3Files)
        {
            Console.WriteLine(mp3File.Tag.Title);
            mp3File.Tag.Title = "Among Us - Main Theme (Elsa and Spider-man Cover)";
        }

        SaveMp3Files(mp3Files);
        DisposeMp3Files(mp3Files);

        mp3Files = LoadNewMp3Files(Mp3TestingDirectory);

        foreach (var mp3File in mp3Files)
        {
            Console.WriteLine(mp3File.Tag.Title);
            mp3File.Tag.Title = "Among Us - Main Theme (GIGACHAD Phonk Mix 2022)";
        }

        SaveMp3Files(mp3Files);
    }

}