using File = TagLib.File;

namespace MP3_Tag_Editor;

internal static class Program
{
    private static bool _exit;
    private static void Main(string[] args)
    {
        while (!_exit)
        {
            Console.Clear();
            var methodSelection = Ui.MainMenu();
            switch (methodSelection)
            {
                case "h":
                    Ui.HelpMenu();
                    break;

                case "x":
                    _exit = true;
                    break;

                case "m":
                    ManualModificationProgram();
                    break;

                case "i":
                    ImportModificationProgram();
                    break;

                default:
                    Console.WriteLine("Invalid input, please try again");
                    Thread.Sleep(1000);
                    break;
            }
        }
    }

    /// <summary>
    /// Executes the program flow for importing and external source file for modifying MP3 tags.
    /// </summary>
    private static void ImportModificationProgram()
    {
        var mp3s = LoadMp3sProgram();
        ViewPropertiesProgram(mp3s);
        var source = LoadSourcesProgram();

        try
        {
            Mp3TagsManager.ModifyMp3TagsWithExternalSource(mp3s, source);
            //This method prints out any failed changes
            //Ideally it would write to an error log that would then be printed by the UI class
        }
        catch (Exception e)
        {
            Console.WriteLine("An unknown error occurred:" + e);
        }

        Mp3FileManager.SaveMp3Files(mp3s);
        Console.WriteLine("Modification Complete." +
                          "\nPress any key to continue.");
        Console.ReadKey();

    }

    /// <summary>
    /// Executes the program flow for manually modifying MP3 tags.
    /// </summary>
    private static void ManualModificationProgram()
    {
        var mp3s = LoadMp3sProgram();

        var isFinishedModification = false;
        while (!isFinishedModification)
        {
            Console.Clear();
            ViewPropertiesProgram(mp3s);

            try
            {
                var modifications = Ui.ModifyTagMenu();
                Mp3TagsManager.ModifyMp3Tags(mp3s, modifications.Item1, modifications.Item2);
                isFinishedModification =
                    !Ui.ModifyAgainMenu(); //loops recursively if invalid input, was easier than making another while loop
            }
            catch (Exception e)
            {
                Console.WriteLine("\nInvalid input, please try again!");
                Thread.Sleep(1000);
                Console.Clear();
            }
        }

        Mp3FileManager.SaveMp3Files(mp3s);
        Console.WriteLine("\nModification complete.");
        Thread.Sleep(1500);
    }

    /// <summary>
    /// Executes the program flow for viewing the properties of a collection of MP3 files.
    /// </summary>
    /// <param name="mp3s"></param>
    private static void ViewPropertiesProgram(List<File> mp3s)
    {
        var isViewAnotherProperty = true;
        while (isViewAnotherProperty)
        {
            Console.Clear();
            isViewAnotherProperty = Ui.ViewPropertiesMenu(mp3s);
        }
    }

    /// <summary>
    /// Executes the program flow for loading a collection of MP3 files.
    /// </summary>
    /// <returns></returns>
    private static List<File> LoadMp3sProgram()
    {
        var isValidPath = false;
        List<File> mp3s = new();
        while (!isValidPath)
        {
            try
            {
                Console.Clear();
                var mp3Path = Ui.Mp3PathMenu();
                var willOverwrite = Ui.OverwriteSelectionMenu();

                mp3s = willOverwrite
                    ? Mp3FileManager.LoadMp3Files(mp3Path).ToList()
                    : Mp3FileManager.LoadNewMp3Files(mp3Path).ToList();

                isValidPath = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nInvalid path, please try again!");
                Thread.Sleep(1000);
            }
        }

        return mp3s;
    }

    /// <summary>
    /// Executes the program flow for loading an external source file.
    /// </summary>
    /// <returns></returns>
    private static dynamic LoadSourcesProgram()
    {
        var isValidPath = false;
        dynamic source = null;
        while (!isValidPath)
        {
            try
            {
                Console.Clear();
                var sourcePath = Ui.SourcePathMenu();
                source = SourceFileManager.LoadSourceFile(sourcePath);
                isValidPath = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nInvalid path, please try again!");
                Thread.Sleep(1000);
            }
        }

        return source;
    }
}