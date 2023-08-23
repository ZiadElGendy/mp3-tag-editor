using MP3_Tag_Editor.Enums;

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

    public static void ModifyMp3Menu()
    {
        Console.WriteLine("Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify\n");
        var path = Console.ReadLine();
        var overwriteSelection = OverwriteMenu();
        List<TagLib.File> mp3s;
        if (overwriteSelection)
            mp3s = Mp3FileManager.LoadMp3Files(path).ToList();
        else
            mp3s = Mp3FileManager.LoadNewMp3Files(path).ToList();
        if (mp3s.Count == 1)//there's gotta be a way to access the only element in a list
        {
            Console.WriteLine(mp3s[0].name); //same as the comment down there vvv
            ViewPropertiesMenu(mp3s[0]);
        }
        else
        {
            for mp3 in mp3s {
                Console.WriteLine(mp3.name); //the library doesn't work for me i literally don't know what properties they have
                ViewPropertiesMenu(mp3)
            }
        }



    }

    public static bool OverwriteMenu()
    {
        Console.WriteLine("\nDo you want to overwrite the original files? (y/n)");
        var input = Console.ReadLine();
        if(input.ToLower() == "y")
        {
            return true;
        }

        return false;
    }

    public static void ViewPropertiesMenu(TagLib.File mp3)
    {
        // print properties of mp3 file
    }

}
