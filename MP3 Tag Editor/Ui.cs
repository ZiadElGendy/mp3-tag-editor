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
        Console.WriteLine("Welcome to MP3 Tag Editor\n" +
            "Enter h for help\n" +
            "Enter e to enter MP3 modification\n" +
            "Enter x to exit program\n" +
            "Enter your choice:");

        var input = Console.ReadLine();
        return input;
    }

    public static void HelpMenu()
    {
        Console.WriteLine("This is MP3 Tag Editor, a program to modify MP3 file tags.\n" +
                          "Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify,\n" +
                          "When entering a path to a directory, changes are made in bulk.\n"+
                          "The accepted fields to change are:\n");

        foreach (Enum tag in Enum.GetValues(typeof(Mp3Tag)))
        {
            Console.WriteLine(tag);
        }

    }

    public static void ModifyMp3Menu()
    {
        Console.WriteLine("Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify");
    }

}
