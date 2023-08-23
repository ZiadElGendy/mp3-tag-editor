using MP3_Tag_Editor;
using System;

namespace Mp3MP3_Tag_Editor;

public static class UI
{
    /*current flow (in my head)
     * in a loop, main will call MainMenu()
     * then depending on 3 options, will either call HelpMenu(), Exit, or enter the ModifyMenu() Flow, which will be self contained in it's methods
     * after the modify ends, the loop starts again at MainMenu(), prompting the user to choose what to do again
     */
    public static String MainMenu()
    {
        Console.WriteLine("Welcome to MP3 Tag Editor\n" +
            "Enter h for help\n" +
            "Enter e to enter MP3 modification\n" +
            "Enter x to exit program\n");
        String input = Console.ReadLine("Enter your choice:");
        return input;
    }

    public static void HelpMenu() {
        Console.WriteLine("This is MP3 Tag Editor, a program to modify MP3 file tags.\n" +
            "Enter the path to your Mp3 file or directory that includes Mp3 files that you wish to modify\n")
    }

    public static void ModifyMp3(String filePath)
    {


        /* 
         * Mp3FileManager.LoadMp3Files(filePath);
         */
    }

}
