using File = TagLib.File;

namespace MP3_Tag_Editor;

internal static class Program
{
    private static bool _exit = false;
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
                    var validPath = false;
                    List<File> mp3s = new();
                    while(!validPath)
                    {
                        try
                        {
                            Console.Clear();
                            var mp3Path = Ui.ModifyMp3Menu();
                            var willOverwrite = Ui.OverwriteSelectionMenu();

                            if (willOverwrite)
                            {
                                mp3s = Mp3FileManager.LoadMp3Files(mp3Path).ToList();
                            }
                            else
                            {
                                mp3s = Mp3FileManager.LoadNewMp3Files(mp3Path).ToList();
                            }
                            validPath = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("\nInvalid path, please try again!");
                            Thread.Sleep(1000);
                        }
                    }

                    var validModification = false;
                    while (!validModification)
                    {
                        Console.Clear();
                        Ui.ViewPropertiesMenu(mp3s);

                        try
                        {
                            var modifications = Ui.ModifyTagMenu(mp3s);
                            Mp3TagsManager.ModifyMp3Tags(mp3s, modifications.Item1, modifications.Item2);
                            validModification = true;
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