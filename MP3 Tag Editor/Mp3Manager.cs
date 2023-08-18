using Id3;
using Id3.Frames;
using MP3_Tag_Editor.Enums;
using MP3_Tag_Editor.Exceptions;

namespace MP3_Tag_Editor;

/// <summary>
///  Provides methods for editing Id3 tags of MP3 files.
/// </summary>
public static class Mp3Manager
{
    #region Static Variables

    private static List<Mp3> _workingMp3Files = new();

    private const string Mp3TestingDirectory =
        @"D:\Documents\Programming\Group Project\2023\ZMBY 3\Sample MP3s";

    #endregion


    #region Methods

    /// <summary>
    /// Modifies the given property of the given <see cref="Mp3"/> file.
    /// </summary>
    /// <param name="inputFilePath">File path of mp3 files to be modified.</param>
    /// <param name="selectedProperty">The <see cref="Id3Tag"/> property to be changed.</param>
    /// <param name="inputValue">The new value to assign to the selectedProperty.</param>
    /// <returns>
    /// An array of Id3Tag corresponding to the input MP3s.
    /// </returns>
    public static IEnumerable<Id3Tag> ModifyMp3Tags(string inputFilePath, string selectedProperty, string inputValue)
    {
        LoadFilesToMemory(inputFilePath);
        dynamic newValue = TypeCastValue(selectedProperty, inputValue);

        if (newValue.GetType().IsArray)
        {
            return ModifyListMp3Tags(selectedProperty, newValue);
        }
        else
        {
            return ModifyNonListMp3Tags(selectedProperty, newValue);
        }
    }

    /// <summary>
    /// Modifies the inputted property of the working <see cref="Mp3"/> file.
    /// Applies to "Artists" and "Composers" properties.
    /// </summary>
    /// <param name="selectedProperty">The <see cref="Id3Tag"/> property to be changed.</param>
    /// <param name="newValue">The collection of values to assign to the selectedProperty.</param>
    /// /// <returns>
    /// An array of <see cref="Id3Tag"/>s corresponding to the input <see cref="Mp3"/>s.
    /// </returns>
    private static IEnumerable<Id3Tag> ModifyListMp3Tags(string selectedProperty, IEnumerable<string> newValue)
    {
        Id3Tag[] newTags = new Id3Tag[_workingMp3Files.Count];
        newValue = newValue.ToArray(); //prevent multiple enumeration

        foreach (var mp3 in _workingMp3Files)
        {
            var newTag = mp3.GetTag(GetTagFamily(mp3));

            switch (selectedProperty)
            {
                case "Artists":
                case "Artist":
                    foreach (var value in newValue)
                    {
                        newTag.Artists.Value.Add(value);
                    }
                    break;

                case "Composers":
                case "Composer":
                    foreach (var value in newValue)
                    {
                        newTag.Composers.Value.Add(value);
                    }
                    break;

                default:
                    throw new InvalidId3TagException("Unknown error occured!");
            }

            newTags[_workingMp3Files.IndexOf(mp3)] = newTag;
        }
        return newTags;
    }

    /// <summary>
    /// Modifies the inputted property of the working <see cref="Mp3"/> file.
    /// Applies to most properties.
    /// </summary>
    /// <param name="selectedProperty">The <see cref="Id3Tag"/> property to be changed.</param>
    /// <param name="newValue">The value to assign to the selectedProperty.</param>
    /// <returns>
    /// An array of <see cref="Id3Tag"/>s corresponding to the input <see cref="Mp3"/>s.
    /// </returns>
    private static IEnumerable<Id3Tag> ModifyNonListMp3Tags(string selectedProperty, dynamic newValue)
    {
        Id3Tag[] newTags = new Id3Tag[_workingMp3Files.Count];
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

            newTags[_workingMp3Files.IndexOf(mp3)] = newTag;
        }
        return newTags;
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
        else if (Enum.IsDefined(typeof(StringListFrames), selectedProperty))
        {
            inputValue = inputValue.Replace(" ", "");
            return inputValue.Split(',');
        }

        else if (Enum.IsDefined(typeof(DateTimeFrames), selectedProperty))
        {
            return DateTime.Parse(inputValue, Program.CultureInfo);
        }
        else if (Enum.IsDefined(typeof(TimeSpanFrames), selectedProperty))
        {
            return TimeSpan.Parse(inputValue, Program.CultureInfo);
        }

        else if (selectedProperty == "FileAudioType")
        {
            return (FileAudioType)Enum.Parse(typeof(FileAudioType), inputValue);
        }
        else if (selectedProperty == "PictureType")
        {
            return (PictureType)Enum.Parse(typeof(PictureType), inputValue);
        }

        throw new InvalidId3PropertyException($"{selectedProperty} is an unsupported property!");
    }

    /// <summary>
    ///  Loads <see cref="Mp3"/> files into memory from the given path.
    /// </summary>
    /// <param name="inputFilePath">The path of the file or directory.</param>
    private static void LoadFilesToMemory(string inputFilePath)
    {
        Mp3FileManager.DisposeMp3Files(_workingMp3Files);
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

    #endregion


    #region Tests

    public static void ModifyMp3TagsTest()
    {
        ModifyMp3Tags(Mp3TestingDirectory,"Year", "2023");
        ModifyMp3Tags(Mp3TestingDirectory, "Artist", "Me, You");
        ModifyMp3Tags(Mp3TestingDirectory, "Track", "1"); //Does not work with multiple inputs (value,trackCount)
        ModifyMp3Tags(Mp3TestingDirectory, "RecordingDate", "2023-01-01");
    }

    #endregion
}