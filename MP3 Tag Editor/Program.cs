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

                case "e":
                    var isValidPath = false;
                    List<File> mp3s = new();
                    while(!isValidPath)
                    {
                        try
                        {
                            Console.Clear();
                            var mp3Path = Ui.ModifyMp3Menu();
                            var willOverwrite = Ui.OverwriteSelectionMenu();

                            mp3s = willOverwrite ?
                                Mp3FileManager.LoadMp3Files(mp3Path).ToList() :
                                Mp3FileManager.LoadNewMp3Files(mp3Path).ToList();

                            isValidPath = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("\nInvalid path, please try again!");
                            Thread.Sleep(1000);
                        }
                    }

                    var isFinishedModification = false;
                    while (!isFinishedModification)
                    {
                        Console.Clear();

                        var isViewAnotherProperty = true;
                        while (isViewAnotherProperty)
                        {
                            Console.Clear();
                            isViewAnotherProperty = Ui.ViewPropertiesMenu(mp3s);
                        }

                        try
                        {
                            var modifications = Ui.ModifyTagMenu();
                            Mp3TagsManager.ModifyMp3Tags(mp3s, modifications.Item1, modifications.Item2);
                            isFinishedModification = !Ui.ModifyAgainMenu(); //loops recursively if invalid input, was easier than making another while loop
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
                    break;

                default:
                    Console.WriteLine("Invalid input, please try again");
                    break;
            }
        }
    }
}