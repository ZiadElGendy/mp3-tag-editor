using System.Xml;
using Microsoft.VisualBasic.FileIO;
using MP3_Tag_Editor.Enums;
using MP3_Tag_Editor.Exceptions;

namespace MP3_Tag_Editor;
/// <summary>
/// Provides methods for managing and loading external source files.
/// </summary>
public static class SourceFileManager
{

    /// <summary>
    /// Loads an external source file for the metadata of a collection of MP3 files.
    /// </summary>
    /// <param name="filePath">The path of the file.</param>
    /// <returns>A 2D array representing fields for each MP3 File in the collection</returns>
    /// <exception cref="InvalidSourcePathException">Thrown when the specified CSV file does not exist.</exception>
    public static dynamic LoadImportFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new InvalidSourcePathException(filePath, "The specified source file does not exist.");
        }

        if (Path.GetExtension(filePath) == ".csv")
        {
            return LoadCsvImportFile(filePath);
        }
        else if (Path.GetExtension(filePath) == ".xml")
        {
            return LoadXmlImportFile(filePath);
        }

        throw new InvalidSourcePathException(filePath, "The specified file is not a valid external source file.");
    }

    /// <summary>
    /// Loads a CSV file of the metadata of a collection of MP3 files.
    /// </summary>
    /// <param name="filePath">The path of the file.</param>
    /// <returns>A 2D array representing fields for each MP3 File in the collection</returns>
    /// <exception cref="InvalidSourcePathException">Thrown when the specified CSV file does not exist.</exception>
    private static (IEnumerable<string>, IEnumerable<string[]>) LoadCsvImportFile(string filePath)
    {
        //Assuming CSV is in format exported by mp3tag application
        using (var csvParser = new TextFieldParser(filePath))
        {
            csvParser.SetDelimiters(";");

            var properties = csvParser.ReadFields();
            var trackInfo = new List<string[]>();
            while (!csvParser.EndOfData)
            {
                trackInfo.Add(csvParser.ReadFields());
            }
            return (properties, trackInfo);
        }
    }

    /// <summary>
    /// Loads an XML file of the metadata for a collection of MP3 files.
    /// </summary>
    /// <param name="filePath">The path of the file</param>
    /// <returns>An XmlDocument object of the given XML file</returns>
    private static XmlDocument LoadXmlImportFile(string filePath)
    {
        var xmlFile = new XmlDocument();
        xmlFile.Load(filePath);
        return xmlFile;
    }

    public static void ModifyFromCsvTest()
    {
        var mp3DirTestPath = "D:\\Documents\\Programming\\Group Project\\2023\\ZMBY 3\\mp3-tag-editor\\Sample MP3s";
        var csvTestPath =
            "D:\\Documents\\Programming\\Group Project\\2023\\ZMBY 3\\mp3-tag-editor\\Sample MP3s\\mp3tagsample.csv";

        var mp3Files = Mp3FileManager.LoadMp3Files(mp3DirTestPath);
        var csvFile = LoadImportFile(csvTestPath);

        Mp3TagsManager.ModifyMp3Tags(mp3Files, Mp3Tag.Title, "Test");
        Mp3FileManager.SaveMp3Files(mp3Files);

        Console.WriteLine("Check");
        Console.ReadLine();

        Mp3TagsManager.ModifyMp3TagsWithCsv(mp3Files, csvFile.Item1, csvFile.Item2);
        Mp3FileManager.SaveMp3Files(mp3Files);
        Console.WriteLine("Done");
        Console.ReadLine();
    }

    private static void ModifyFromXmlTest()
    {
        var mp3DirTestPath = "ADD DIRPATH HERE";
        var xmlTestPath = "ADD FILEPATH HERE";

        var mp3Files = Mp3FileManager.LoadMp3Files(mp3DirTestPath);
        var csvFile = LoadImportFile(xmlTestPath);

        Mp3TagsManager.ModifyMp3Tags(mp3Files, Mp3Tag.Title, "Test");
        Mp3FileManager.SaveMp3Files(mp3Files);

        Console.WriteLine("Check");
        Console.ReadLine();

        Mp3TagsManager.ModifyMp3TagsWithXml(mp3Files, csvFile);
        Mp3FileManager.SaveMp3Files(mp3Files);
        Console.WriteLine("Done");
        Console.ReadLine();
    }

}