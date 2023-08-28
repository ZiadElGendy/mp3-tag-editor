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
        Console.Clear();
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
        Console.Clear();
        Console.WriteLine("Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify\n");
        var path = Console.ReadLine();
        return path ?? throw new NullReferenceException();
    }

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
        else
        {
            Console.WriteLine("Invalid input!");
            return OverwriteSelectionMenu();
        }
    }

    public static bool ImportSelectionMenu()
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
        else
        {
            Console.WriteLine("Invalid input!");
            return OverwriteSelectionMenu();
        }
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

    public static void ViewFilePropertiesMenu(TagLib.File mp3)
    {
        //FIXME: This line simply does not work. I hate it here
        PropertyInfo[] properties = mp3.GetTag(TagTypes.Id3v1).GetType().GetProperties();

        foreach (PropertyInfo p in properties)
        {
            System.Console.WriteLine(p.Name + " : " + p.GetValue(mp3));
        }
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

    public static (string, string) ModifyTagMenu(IEnumerable<TagLib.File> mp3s)
    {
        Console.WriteLine("\nEnter which tag you wish to modify: ");
        var tag = Console.ReadLine();
        Console.WriteLine("\nEnter the value you want: ");
        var value = Console.ReadLine();
        return (tag, value);

    }

}
