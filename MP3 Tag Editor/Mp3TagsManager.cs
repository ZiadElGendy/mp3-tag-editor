using System.Globalization;
using System.Text.RegularExpressions;
using MP3_Tag_Editor.Enums;
using MP3_Tag_Editor.Exceptions;

namespace MP3_Tag_Editor;

/// <summary>
/// Provides methods for modifying MP3 tags.
/// </summary>
public static class Mp3TagsManager
{
    /// <summary>
    /// Modifies the specified MP3 tags in the given file or directory.
    /// </summary>
    /// <param name="path">The path to the MP3 file or directory.</param>
    /// <param name="stringTag">The string representation of the MP3 tag to modify.</param>
    /// <param name="value">The new value to assign to the MP3 tag.</param>
    /// <param name="overwrite">Whether to overwrite existing tags if present.</param>
    /// <exception cref="InvalidMp3TagException">Thrown when the specified MP3 tag does not exist.</exception>
    /// <exception cref="InvalidMp3TagValueException">Thrown when the provided value is invalid for the specified MP3 tag.</exception>
    public static void ModifyMp3Tags(string path, string stringTag, dynamic value, bool overwrite = false)
    {
        if (string.IsNullOrEmpty(stringTag))
        {
            return;
        }
        
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
        
        
        ModifyMp3Tags(path, tag, value, overwrite);
    }


    /// <summary>
    /// Modifies the specified MP3 tags in the given file or directory.
    /// </summary>
    /// <param name="path">The path to the MP3 file or directory.</param>
    /// <param name="tag">The MP3 tag to modify.</param>
    /// <param name="value">The new value to assign to the MP3 tag.</param>
    /// <param name="overwrite">Whether to overwrite existing tags if present.</param>
    /// <exception cref="InvalidMp3TagException">Thrown when the specified MP3 tag does not exist.</exception>
    /// <exception cref="InvalidMp3TagValueException">Thrown when the provided value is invalid for the specified MP3 tag.</exception>
    public static void ModifyMp3Tags(string path, Mp3Tag tag, dynamic value, bool overwrite = false)
    {
        var files = Mp3FileManager.LoadMp3Files(path).ToList();
        files.ForEach(file => ModifyMp3Tag(file, tag, value));
        Mp3FileManager.SaveNewMp3File(files, Path.GetDirectoryName(path) ?? throw new InvalidMp3PathException(path), overwrite);
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
        var propertyInfo = typeof(TagLib.Tag).GetProperty(mp3Tag.ToString());

        if (propertyInfo != null)
        {
            try
            {
                var tagInstance = propertyInfo.GetValue(file.Tag);
                var tagType = tagInstance?.GetType();
                try
                {
                    object? convertedValue = ConvertValueType(value, tagType);
                    propertyInfo.SetValue(file.Tag, convertedValue);
                }
                catch (InvalidCastException)
                {
                    if (tagType is { IsArray: true })
                    {
                        propertyInfo.SetValue(file.Tag, new[] { (string)value.ToString() });
                    }
                }
            }
            catch (Exception e)
            {
                if (e is System.Reflection.TargetException) throw;

                throw new InvalidMp3TagValueException(value, mp3Tag);
            }
        }
        else
        {
            throw new InvalidMp3TagException(file);
        }
    }

    private static dynamic ConvertValueType(dynamic value, dynamic newType) => Convert.ChangeType(value, newType.GetType());
}