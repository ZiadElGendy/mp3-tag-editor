using System.ComponentModel;
using System.Reflection;
using MP3_Tag_Editor.Enums;
using TagLib;
using File = TagLib.File;

namespace MP3_Tag_Editor;

public static class Ui
{
    /*current flow (in my head)
     * in a loop, main will call MainMenu()
     * then depending on 3 options, will either call HelpMenu(), Exit, or enter the ModifyMenu() Flow, which will be self contained in it's methods
     * after the modify ends, the loop starts again at MainMenu(), prompting the user to choose what to do again
     */
    public static string MainMenu()
    {
        Console.WriteLine("Welcome to MP3 Tag Editor\n\n" +
            "Enter h for help\n" +
            "Enter e to enter MP3 modification\n" +
            "Enter x to exit program\n");

        var input = Console.ReadLine();
        return input;
    }

    public static void HelpMenu()
    {
        Console.WriteLine("This is MP3 Tag Editor, a program to modify MP3 file tags.\n" +
                          "Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify,\n" +
                          "When entering a path to a directory, changes are made in bulk.\n");

        Console.WriteLine("Do you want to view supported fields? (y/n)");
        var input = Console.ReadLine();

        if (input == "y")
        {
            Console.WriteLine("The accepted fields to change are:\n");
            foreach (Enum tag in Enum.GetValues(typeof(Mp3Tag)))
            {
                Console.WriteLine(tag);
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

    }

    public static string ModifyMp3Menu()
    {
        Console.WriteLine("Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify\n");
        var path = Console.ReadLine();
        return path ?? throw new NullReferenceException();
    }

    public static bool OverwriteSelectionMenu()
    {
        Console.WriteLine("\nDo you want to overwrite the original files? (y/n)");
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

    public static void ViewPropertiesMenu(IEnumerable<File> mp3s)
    {
        var mp3List = mp3s.ToList();
        if (mp3List.Count == 1)
        {
            ViewFilePropertiesMenu(mp3List[0]);
        }
        else
        {
            ViewDirectoryPropertiesMenu(mp3List);
        }
    }

    private static void ViewFilePropertiesMenu(TagLib.File mp3)
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
    }

    private static bool IsValidToStringHelper(string name, string value)
    {
        var undesiredNames = new string[] {"StartTag","EndTag","TagTypes","Tags","ReplayGainTrackGain",
                    "ReplayGainTrackPeak", "ReplayGainAlbumGain", "ReplayGainAlbumPeak", "IsEmpty", "Pictures"};
        var undesiredValues = new string[] {"System.String[]", "NaN"};

        if (string.IsNullOrEmpty(value)) return false;
        if (undesiredNames.Contains(name)) return false;
        if (undesiredValues.Contains(value)) return false;
        return true;
    }

    private static void ViewDirectoryPropertiesMenu(IEnumerable<File> mp3s)
    {
        var mp3List = mp3s.ToList();
        foreach (var mp3 in mp3List)
        {
            Console.WriteLine((mp3List.IndexOf(mp3)+1) + ") " + mp3.Name);
        }

        Console.WriteLine("\nDo you want to display file properties? (y/n)");

        var decision = Console.ReadLine();
        if (decision.ToLower() == "y")
        {
            Console.WriteLine("\n Select file number: ");
            var fileSelection = Console.ReadLine();
            var fileIndex = int.TryParse(fileSelection, out var index) ? index - 1 : -1;

            if (fileIndex == -1)
            {
                throw new InvalidCastException("Invalid input");
            }

            ViewFilePropertiesMenu(mp3List[fileIndex]);
        }
    }

    public static (string?, string?) ModifyTagMenu(IEnumerable<TagLib.File> mp3s)
    {
        Console.WriteLine("\nEnter which tag you wish to modify: ");
        var tag = Console.ReadLine();
        Console.WriteLine("\nEnter the value you want: ");
        var value = Console.ReadLine();
        return (tag, value);

    }

    public static void ViewPropertiesTest()
    {
        var mp3 = Mp3FileManager.LoadMp3Files(@"D:\Documents\Programming\Group Project\2023\ZMBY 3\mp3-tag-editor\Sample MP3s\01 No Escape.mp3");
        ViewPropertiesMenu(mp3);
    }

}
