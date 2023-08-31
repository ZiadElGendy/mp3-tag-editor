using System.ComponentModel;
using MP3_Tag_Editor.Enums;

namespace MP3_Tag_Editor;

/// <summary>
/// Provides methods for taking inputs and outputting to console.
/// </summary>
public static class Ui
{

    /// <summary>
    /// Displays main menu and returns user input.
    /// </summary>
    /// <returns>The user's choice of MP3 Tag Editor functionality.</returns>
    public static string MainMenu()
    {
        Console.WriteLine("Welcome to MP3 Tag Editor\n\n" +
            "Enter h for help\n" +
            "Enter m to manually edit MP3 tags\n" +
            "Enter i to import MP3 tags from an external source\n" +
            "Enter x to exit program\n");

        var input = Console.ReadLine();
        return input;
    }

    /// <summary>
    /// Displays help menu.
    /// </summary>
    public static void HelpMenu()
    {
        Console.WriteLine("This is MP3 Tag Editor, a program to modify MP3 file tags.\n" +
                          "Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify,\n" +
                          "When entering a path to a directory, changes are made in bulk.\n");

        Console.WriteLine("Do you want to view supported fields? (y/n)");
        var input = Console.ReadLine();

        if (input == "y")
        {
            Console.WriteLine("The supported fields to change are:\n");
            foreach (Enum tag in Enum.GetValues(typeof(Mp3Tag)))
            {
                Console.WriteLine(tag);
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

    }

    /// <summary>
    /// Displays prompt for mp3 path input.
    /// </summary>
    /// <returns>The path of the mp3 file or directory to modify.</returns>
    /// <exception cref="NullReferenceException">Thrown when input is null.</exception>
    public static string ModifyMp3Menu()
    {
        Console.WriteLine("Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify\n");
        var path = Console.ReadLine();
        return path ?? throw new NullReferenceException();
    }

    /// <summary>
    /// Displays prompt for overwrite selection.
    /// </summary>
    /// <returns>True if user wants to overwrite, False if not.</returns>
    /// <exception cref="ArgumentException">Thrown when input is not 'y' or 'n'.</exception>
    public static bool OverwriteSelectionMenu()
    {
        Console.WriteLine("\nDo you want to overwrite the original files? (y/n)");
        var input = Console.ReadLine();
        if(input.ToLower() == "y")
        {
            return true;
        }
        else if(input.ToLower() == "n")
        {
            return false;
        }
        throw new ArgumentException("Input was not 'y' or 'n'");
    }

    public static bool ImportSelectionMenu()
    {
        Console.WriteLine("\nDo you want to import tag information? (y/n)");
        var input = Console.ReadLine() ?? throw new NullReferenceException();
        if(input.ToLower() == "y")
        {
            return true;
        }
        else if(input.ToLower() == "n")
        {
            return false;
        }
        throw new ArgumentException("Input was not 'y' or 'n'");
    }

    /// <summary>
    /// Displays appropriate properties menu based on file or directory path.
    /// </summary>
    /// <returns>true if user want to view another file in directory, false if not</returns>
    /// <param name="mp3s">MP3 files to modify</param>
    public static bool ViewPropertiesMenu(IEnumerable<TagLib.File> mp3s)
    {
        var mp3List = mp3s.ToList();
        if (mp3List.Count == 1)
        {
            return ViewFilePropertiesMenu(mp3List[0]);
        }
        else
        {
            return ViewDirectoryPropertiesMenu(mp3List);
        }
    }

    /// <summary>
    /// Display relevant properties for a single MP3 file.
    /// </summary>
    /// <param name="mp3">The MP3 file to inspect</param>
    private static bool ViewFilePropertiesMenu(TagLib.File mp3)
    {
        var tag = mp3.Tag;
        foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(tag))
        {
            try
            {
                var name = descriptor.Name;
                var value = descriptor.GetValue(tag);
                var valueString = value is Array? string.Join(", ", value) : value?.ToString();

                if (IsValidToStringHelper(name, valueString))
                {
                    Console.WriteLine("{0}: {1}", name, valueString);
                }
            }
            catch (Exception e)
            {
                //Skip the weirdos
            }
        }

        return false;
    }

    /// <summary>
    /// Removes undesired properties from the properties menu.
    /// </summary>
    private static bool IsValidToStringHelper(string name, string value)
    {
        var undesiredNames = new [] {"StartTag","EndTag","TagTypes","Tags","ReplayGainTrackGain",
                    "ReplayGainTrackPeak", "ReplayGainAlbumGain", "ReplayGainAlbumPeak", "IsEmpty", "Pictures"};
        var undesiredValues = new [] {"System.String[]", "NaN"};

        if (string.IsNullOrEmpty(value)) return false;
        if (undesiredNames.Contains(name)) return false;
        if (undesiredValues.Contains(value)) return false;
        return true;
    }

    /// <summary>
    /// Display menu for selecting MP3 file from directory to view properties.
    /// </summary>
    /// <param name="mp3s">The MP3 directory to inspect</param>
    /// <returns>true if user want to view another file, false if not</returns>
    /// <exception cref="ArgumentException">Thrown when user gives incorrect selection input</exception>
    private static bool ViewDirectoryPropertiesMenu(IEnumerable<TagLib.File> mp3s)
    {
        var mp3List = mp3s.ToList();
        foreach (var mp3 in mp3List)
        {
            if (!string.IsNullOrEmpty(mp3.Tag.Title))
            {
                Console.WriteLine((mp3List.IndexOf(mp3) + 1) + ") " + mp3.Tag.Title);
            }
            else
            {
                Console.WriteLine((mp3List.IndexOf(mp3) + 1) + ") " + mp3.Name);
            }
        }

        Console.WriteLine("\nDo you want to display file properties? (y/n)");

        var displayFileInput = Console.ReadLine();
        if (displayFileInput.ToLower() == "y")
        {
            Console.WriteLine("\nSelect file number: ");
            var fileSelection = Console.ReadLine();
            var fileIndex = int.TryParse(fileSelection, out var index) ? index - 1 : -1;

            if (fileIndex == -1)
            {
                throw new ArgumentException("Invalid input");
            }

            ViewFilePropertiesMenu(mp3List[fileIndex]);
        }
        else if (displayFileInput.ToLower() == "n")
        {
            return false;
        }
        else
        {
            throw new ArgumentException("Input was not 'y' or 'n'");
        }

        Console.WriteLine("\nDo you want to view another property? (y/n)");
        var displayAnotherInput = Console.ReadLine();

        if (displayAnotherInput.ToLower() == "y")
        {
            return true;
        }
        else if (displayAnotherInput.ToLower() == "n")
        {
            return false;
        }
        else
        {
            throw new ArgumentException("Input was not 'y' or 'n'");
        }

    }

    /// <summary>
    /// Displays menu for modification input
    /// </summary>
    /// <param name="mp3s">The MP3 files to modify</param>
    /// <returns>A tuple of the mp3 tag name and value respectively</returns>
    public static (string?, string?) ModifyTagMenu()
    {
        Console.WriteLine("\nEnter which tag you wish to modify: ");
        var tag = Console.ReadLine();
        Console.WriteLine("\nEnter the value you want: ");
        var value = Console.ReadLine();
        return (tag, value);

    }

    /// <summary>
    /// Displays menu for modifying another tag.
    /// </summary>
    /// <returns>true if user wants to select another tag, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when input is not 'y' or 'n'.</exception>
    public static bool ModifyAgainMenu()
    {
        Console.WriteLine("\nDo you want to modify another tag? (y/n)");
        var input = Console.ReadLine();

        if(input.ToLower() == "y")
        {
                return true;
        }
        else if(input.ToLower() == "n")
        {
                return false;
        }
        else
        {
            Console.WriteLine("Incorrect input, please try again.");
            return ModifyAgainMenu();
        }
    }

    public static void ViewPropertiesTest()
    {
        var mp3 = Mp3FileManager.LoadMp3Files(@"D:\Documents\Programming\Group Project\2023\ZMBY 3\mp3-tag-editor\Sample MP3s");
        ViewPropertiesMenu(mp3);
    }

}
