using System.Reflection;
using Id3;
using Id3.Frames;
using Microsoft.VisualBasic.CompilerServices;
using MP3_Tag_Editor.Enums;
using MP3_Tag_Editor.Exceptions;

namespace MP3_Tag_Editor;

/// <summary>
///   Provides methods for editing Id3 tags of MP3 files.
/// </summary>
public static class Mp3Manager
{
    private static List<Mp3> _workingMp3Files = new List<Mp3>();

    /// <summary>
    ///  Modifies the given property of the given <see cref="Mp3"/> file.
    /// </summary>
    public static void ModifyMp3Tags(string inputFilePath, string selectedProperty, string inputValue)
    {
        LoadFilesToMemory(inputFilePath);
        dynamic newValue = TypeCastValue(selectedProperty, inputValue);

        foreach (var mp3 in _workingMp3Files)
        {
            var newTag = mp3.GetTag(GetTagFamily(mp3));

            //Uses Reflection to dynamically change property instead of switch statement
            //selectedProperty must be exactly equal property in Id3Tag
            try
            {
                var tagPropertyInfo =
                    newTag.GetType().GetProperty(selectedProperty); //dynamically get selected property
                var tagPropertyFrame =
                    Activator.CreateInstance(tagPropertyInfo.PropertyType,
                        newValue); //create new instance of property type with parameter
                tagPropertyInfo.SetValue(newTag, tagPropertyFrame, null);

                Console.WriteLine(tagPropertyInfo.GetValue(newTag, null));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not change value due to error: {e.StackTrace}");
                throw;
            }
        }
    }

    //FIXME: If blocks are never entered
    /// <summary>
    ///  Casts the given inputValue <see cref="string"/> to the appropriate type according to selectedProperty
    /// </summary>
    /// <returns> A dynamic type value </returns>
    private static dynamic TypeCastValue(string selectedProperty, string inputString)
    {
        if (Enum.IsDefined(typeof(StringFrames), inputString))
        {
            return inputString;
        }
        else if (Enum.IsDefined(typeof(IntFrames), inputString))
        {
            return Int32.Parse(inputString);
        }
        else if (Enum.IsDefined(typeof(DateTimeFrames), inputString))
        {
            return DateTime.Parse(inputString);
        }
        else if (Enum.IsDefined(typeof(TimeSpanFrames), inputString))
        {
            return TimeSpan.Parse(inputString);
        }

        throw new InvalidCastException($"{selectedProperty} is unsupported!");
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
        ModifyMp3Tags(@"D:\Documents\Programming\Group Project\2023\ZMBY 3\mp3-tag-editor\Sample MP3s", "Year", "2021");
    }

    public static void ModifyArtistTest()
    {
        string[] musicFiles =
            Directory.GetFiles(@"D:\Documents\Programming\Group Project\2023\ZMBY 3\mp3-tag-editor\Sample MP3s",
                "*.mp3");
        foreach (string musicFile in musicFiles)
        {
            using (var mp3 = new Mp3(musicFile))
            {
                Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);

                List<string> artists = new List<string>();
                artists.Add("Me");

                //tag.Artists = artists;
            }
        }
    }

    #endregion
}