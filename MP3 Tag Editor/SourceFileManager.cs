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
    /// <returns>A tuple containing a collection of properties and a 2D array of id information per song respectively</returns>
    /// <exception cref="InvalidSourcePathException">Thrown when the specified CSV file does not exist.</exception>
    private static (IEnumerable<string>?, IEnumerable<string[]>?) LoadCsvImportFile(string filePath)
    {
        //Assuming CSV is in format exported by mp3tag application
        using (var csvParser = new TextFieldParser(filePath))
        {
            csvParser.SetDelimiters(";");

            var idProperties = csvParser.ReadFields()?? throw new InvalidSourcePathException(filePath, "The specified CSV file is in an invalid format.");
            var idValues = new List<string[]>();
            while (!csvParser.EndOfData)
            {
                idValues.Add(csvParser.ReadFields()?? throw new InvalidSourcePathException(filePath, "The specified CSV file is in an invalid format."));
            }
            return (idProperties, idValues);
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
}