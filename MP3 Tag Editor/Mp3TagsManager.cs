using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using MP3_Tag_Editor.Enums;
using MP3_Tag_Editor.Exceptions;
using TagLib;

namespace MP3_Tag_Editor;

/// <summary>
/// Provides methods for modifying MP3 tags.
/// </summary>
public static class Mp3TagsManager
{
    private static Dictionary<string, Type>? _tagLibTagTypes;

    //FIXME: Modifying the track number is not working

    /// <summary>
    /// Modifies the specified MP3 tags in the given file or directory.
    /// </summary>
    /// <param name="files">The list of MP3 files to modify.</param>
    /// <param name="stringTag">The string representation of the MP3 tag to modify.</param>
    /// <param name="value">The new value to assign to the MP3 tag.</param>
    /// <exception cref="InvalidMp3TagException">Thrown when the specified MP3 tag does not exist.</exception>
    /// <exception cref="InvalidMp3TagValueException">Thrown when the provided value is invalid for the specified MP3 tag.</exception>
    /// <returns>A list of modified mp3 files.</returns>
    public static IEnumerable<TagLib.File> ModifyMp3Tags(IEnumerable<TagLib.File> files, string stringTag, dynamic value)
    {
        // Remove unwanted characters and convert to PascalCase
        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        var words = Regex.Split(stringTag, @"\W+");

        for (var i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
                words[i] = textInfo.ToTitleCase(words[i].ToLower());
        }
        
        stringTag = string.Join("", words);
        Enum.TryParse(stringTag, out Mp3Tag tag);
        
        return ModifyMp3Tags(files, tag, value);
    }

    /// <summary>
    /// Modifies the specified MP3 tags in the given file or directory.
    /// </summary>
    /// <param name="files">The list of MP3 files to modify.</param>
    /// <param name="tag">The MP3 tag to modify.</param>
    /// <param name="value">The new value to assign to the MP3 tag.</param>
    /// <exception cref="InvalidMp3TagException">Thrown when the specified MP3 tag does not exist.</exception>
    /// <exception cref="InvalidMp3TagValueException">Thrown when the provided value is invalid for the specified MP3 tag.</exception>
    /// <returns>A list of modified mp3 files.</returns>
    public static IEnumerable<TagLib.File> ModifyMp3Tags(IEnumerable<TagLib.File> files, Mp3Tag tag, dynamic value)
    {
        var mp3Files = files.ToList();
        mp3Files.ForEach(file => ModifyMp3Tag(file, tag, value));
        return mp3Files;
    }

    /// <summary>
    /// Modifies the specified MP3 tag of a TagLib.File.
    /// </summary>
    /// <param name="file">The TagLib.File object to modify.</param>
    /// <param name="mp3Tag">The MP3 tag to modify.</param>
    /// <param name="value">The new value to assign to the MP3 tag.</param>
    /// <returns>The modified TagLib.File object.</returns>
    /// <exception cref="InvalidMp3TagException">Thrown when the specified MP3 tag does not exist.</exception>
    /// <exception cref="InvalidMp3TagValueException">Thrown when the provided value is invalid for the specified MP3 tag.</exception>
    private static void ModifyMp3Tag(TagLib.File file, Mp3Tag mp3Tag, dynamic value)
    {
        if (_tagLibTagTypes is null)
        {
            InitializeTagLibTagTypesList();
        }

        var propertyInfo = typeof(Tag).GetProperty(mp3Tag.ToString());

        if (propertyInfo is null)
        {
            throw new InvalidMp3TagException(file);
        }

        try
        {
            var tagType = _tagLibTagTypes?[mp3Tag.ToString()];
            ModifyTagProperty(file, propertyInfo, tagType, value);
        }
        catch (Exception e)
        {
            HandlePropertyModificationException(e, mp3Tag, value);
        }
    }
    
    /// <summary>
    /// Modifies the specified MP3 tags using a CSV file.
    /// It is assumed the MP3 files are in the same order as the CSV format.
    /// </summary>
    /// <param name="files">The list of MP3 files to modify.</param>
    /// <param name="sourceProperties">The properties to modify as given by the CSV file</param>
    /// <param name="sourceValues">A 2D array consisting of an array of id values per song as given by the CSV file</param>
    public static void ModifyMp3TagsWithCsv(IEnumerable<TagLib.File> files,IEnumerable<string> sourceProperties,IEnumerable<string[]> sourceValues)
    {
        var mp3Files = files.ToList();
        var idValues = sourceValues.ToArray();
        var i = 0;
        mp3Files.ForEach(file => ModifyMp3TagWithCsv(file, sourceProperties.ToArray(), idValues[i++]));
    }

    /// <summary>
    /// Assigns the values from a CSV file to the MP3 tags of a TagLib.File.
    /// </summary>
    /// <param name="file">The TagLib.File object to modify.</param>
    /// <param name="sourceProperties">The properties to modify as given by the CSV file</param>
    /// <param name="sourceValues">The values to use as given by the CSV file</param>
    private static void ModifyMp3TagWithCsv(TagLib.File file, string[] sourceProperties,string[] sourceValues)
    {
        foreach (var property in sourceProperties)
        {
            Enum.TryParse(property, out Mp3Tag tag);
            ModifyMp3Tag(file, tag, sourceValues[Array.IndexOf(sourceProperties, property)]);
        }
    }

    /// <summary>
    /// Modifies the specified MP3 tags using a an XML.
    /// It is assumed the MP3 files are in the same order as the XML format.
    /// </summary>
    /// <param name="files">The list of MP3 files to modify.</param>
    /// <param name="sourceXml">The XML file to use.</param>
    public static void ModifyMp3TagsWithXml(IEnumerable<TagLib.File> files, XmlDocument sourceXml)
    {
        var mp3Files = files.ToArray();
        var xmlRoot = sourceXml.DocumentElement;
        var i = 0;

        foreach (XmlNode track in xmlRoot.ChildNodes)
        {
            foreach (XmlNode property in track.ChildNodes)
            {
                Enum.TryParse(property.Name, out Mp3Tag tag);
                ModifyMp3Tag(mp3Files[i], tag, property.InnerText);
            }
            i++;
        }
    }

    /// <summary>
    /// Initializes the dictionary of tag property names and their associated types.
    /// </summary>
    private static void InitializeTagLibTagTypesList()
    {
        _tagLibTagTypes = new Dictionary<string, Type>();
        var file = TagLib.File.Create(Path.Combine(Directory.GetCurrentDirectory(), @"Data\data.mp3"));

        foreach (var propertyInfo in file.Tag.GetType().GetProperties())
        {
            _tagLibTagTypes.Add(propertyInfo.Name, propertyInfo.PropertyType);
        }
    }
    
    /// <summary>
    /// Modifies a specific MP3 tag property of a TagLib.File.
    /// </summary>
    /// <param name="file">The TagLib.File object to modify.</param>
    /// <param name="propertyInfo">The property information representing the MP3 tag.</param>
    /// <param name="tagType">The expected type of the MP3 tag property.</param>
    /// <param name="value">The new value to assign to the MP3 tag property.</param>
    private static void ModifyTagProperty(TagLib.File file, PropertyInfo propertyInfo, Type? tagType, dynamic value)
    {
        if (tagType is null)
        {
            return;
        }

        try
        {
            if (value.GetType() != tagType)
            {
                value = Convert.ChangeType(value, tagType);
            }
            propertyInfo.SetValue(file.Tag, value);
        }
        catch (InvalidCastException)
        {
            HandleInvalidCastException(file, propertyInfo, value);
        }
    }

    /// <summary>
    /// Handles an InvalidCastException by modifying the MP3 tag property if it's an array type and the value is not an array.
    /// </summary>
    /// <param name="file">The TagLib.File object to modify.</param>
    /// <param name="propertyInfo">The property information representing the MP3 tag.</param>
    /// <param name="value">The value that was being assigned to the MP3 tag property.</param>
    private static void HandleInvalidCastException(TagLib.File file, PropertyInfo propertyInfo, dynamic value)
    {
        if (propertyInfo.PropertyType.IsArray && value is not Array)
        {
            propertyInfo.SetValue(file.Tag, new[] { (string)value.ToString() });
        }
    }
    
    /// <summary>
    /// Handles exceptions that occur during property modification.
    /// </summary>
    /// <param name="e">The exception that occurred during property modification.</param>
    /// <param name="mp3Tag">The MP3 tag that was being modified.</param>
    /// <param name="value">The value that was being assigned to the MP3 tag.</param>
    private static void HandlePropertyModificationException(Exception e, Mp3Tag mp3Tag, dynamic value)
    {
        if (e is TargetException)
        {
            throw e;
        }

        throw new InvalidMp3TagValueException(value, mp3Tag, e);
    }
}