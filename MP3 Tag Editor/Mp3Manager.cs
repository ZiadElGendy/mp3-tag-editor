using Id3;
using MP3_Tag_Editor.Enums;
using MP3_Tag_Editor.Exceptions;

namespace MP3_Tag_Editor;

/// <summary>
///  Provides methods for editing Id3 tags of MP3 files.
/// </summary>
public static class Mp3Manager
{
    private static List<Mp3> _workingMp3Files = new();

    /// <summary>
    /// Modifies the given property of the given <see cref="Mp3"/> file.
    /// </summary>
    public static void ModifyMp3Tags(string inputFilePath, string selectedProperty, string inputValue)
    {
        LoadFilesToMemory(inputFilePath);
        dynamic newValue = TypeCastValue(selectedProperty, inputValue);

        if (newValue.GetType().IsArray)
        {
            ModifyListMp3Tags(selectedProperty, newValue);
        }
        else
        {
            ModifyNonListMp3Tags(selectedProperty, newValue);
        }
    }

    /// <summary>
    /// Modifies the inputted property of the working <see cref="Mp3"/> file.
    /// Applies to "Artists" and "Composers" properties.
    /// </summary>
    private static void ModifyListMp3Tags(string selectedProperty, string[] newValue)
    {
        foreach (var mp3 in _workingMp3Files)
        {
            var newTag = mp3.GetTag(GetTagFamily(mp3));

            switch (selectedProperty)
            {
                case "Artists":
                case "Artist":
                    foreach (string value in newValue)
                    {
                        Console.WriteLine($"value {value}");
                        newTag.Artists.Value.Add(value);
                        Console.WriteLine($"artists after: {newTag.Artists}");
                    }
                    break;

                case "Composers":
                case "Composer":
                    foreach (string value in newValue)
                    {
                        newTag.Composers.Value.Add(value);
                    }
                    break;

                default:
                    throw new InvalidId3TagException("Unknown error occured!");
            }

            mp3.WriteTag(newTag);
        }
    }

    /// <summary>
    /// Modifies the inputted property of the working <see cref="Mp3"/> file.
    /// Applies to most properties.
    /// </summary>
    private static void ModifyNonListMp3Tags(string selectedProperty, dynamic newValue)
    {
        foreach (var mp3 in _workingMp3Files)
        {
            var newTag = mp3.GetTag(GetTagFamily(mp3));

            //Uses Reflection to dynamically change property instead of switch statement
            //selectedProperty must be exactly equal property in Id3Tag
            try
            {
                var tagPropertyInfo =
                    newTag.GetType().GetProperty(selectedProperty);

                var tagPropertyFrame =
                    Activator.CreateInstance(tagPropertyInfo.PropertyType, newValue);
                //Directly setting it didn't seem to work so I created a frame manually

                tagPropertyInfo.SetValue(newTag, tagPropertyFrame, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not change value due to error: {e.Message}");
                throw;
            }

            mp3.WriteTag(newTag);
        }
    }

    /// <summary>
    /// Casts the given inputValue <see cref="string"/> to the appropriate type according to selectedProperty.
    /// </summary>
    /// <returns>
    /// A dynamically typed value which can be <see cref="string"/>, <see cref="Int32"/>, <see cref="DateTime"/>,
    /// <see cref="TimeSpan"/> or an array of <see cref="string"/>s
    /// </returns>
    private static dynamic TypeCastValue(string selectedProperty, string inputValue)
    {

        if (Enum.IsDefined(typeof(StringFrames), selectedProperty))
        {
            return inputValue;
        }
        else if (Enum.IsDefined(typeof(IntFrames), selectedProperty))
        {
            return Int32.Parse(inputValue);
        }
        else if (Enum.IsDefined(typeof(DateTimeFrames), selectedProperty))
        {
            return DateTime.Parse(inputValue, Program.CultureInfo);
        }
        else if (Enum.IsDefined(typeof(TimeSpanFrames), selectedProperty))
        {
            return TimeSpan.Parse(inputValue, Program.CultureInfo);
        }
        else if (Enum.IsDefined(typeof(StringListFrames), selectedProperty))
        {
            inputValue = inputValue.Replace(" ", "");
            return inputValue.Split(',');
        }

        throw new InvalidId3PropertyException($"{selectedProperty} is an unsupported property!");
    }

    /// <summary>
    ///  Loads <see cref="Mp3"/> files into memory from the given path.
    /// </summary>
    /// <param name="inputFilePath">The path of the file or directory.</param>
    private static void LoadFilesToMemory(string inputFilePath)
    {
        _workingMp3Files = (List<Mp3>)Mp3FileManager.LoadMp3Files(inputFilePath);
    }

    /// <summary>
    ///  Gets the tag family of the given <see cref="Mp3"/> file.
    /// </summary>
    private static Id3TagFamily GetTagFamily(Mp3 mp3)
    {
        if (mp3.GetTag(Id3TagFamily.Version2X) != null)
        {
            return Id3TagFamily.Version2X;
        }

        if (mp3.GetTag(Id3TagFamily.Version1X) != null)
        {
            return Id3TagFamily.Version1X;
        }

        throw new InvalidId3TagException(mp3);
    }


    #region Tests

    public static void ModifyMp3TagsTest()
    {
        ModifyMp3Tags(@"D:\Documents\Programming\Group Project\2023\ZMBY 3\Sample MP3s","Year", "2023");
        ModifyMp3Tags(@"D:\Documents\Programming\Group Project\2023\ZMBY 3\Sample MP3s", "Artist", "Me, You");
    }

    public static void ModifyArtistTest()
    {
        string[] musicFiles = Directory.GetFiles(@"D:\Documents\Programming\Group Project\2023\ZMBY 3\Sample MP3s","*.mp3");
        foreach (string musicFile in musicFiles)
        {
            using (var mp3 = new Mp3(musicFile))
            {
                Id3Tag tag = mp3.GetTag(Id3TagFamily.Version1X);
                tag.Artists.Value.Add("Me");
                tag.Artists.Value.Add("You");
                tag.Year = 1000;
                Console.WriteLine(tag.Artists);
                Console.WriteLine(tag.Year);
            }
        }
    }

    #endregion
}