namespace MP3_Tag_Editor;

internal static class Program
{
    private static bool _exit = false;
    private static void Main(string[] args)
    {
        while (!_exit)
        {
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
                    var mp3Path = Ui.ModifyMp3Menu();
                    var overwriteSelection = Ui.OverwriteSelectionMenu();

                    List<TagLib.File> mp3s;
                    if (overwriteSelection)
                        mp3s = Mp3FileManager.LoadMp3Files(mp3Path).ToList();
                    else
                        mp3s = Mp3FileManager.LoadNewMp3Files(mp3Path).ToList();

                    //TODO: Add this back after fixing
                    //ViewPropertiesMenu(mp3s);

                    var correctInput = false;
                    while (!correctInput)
                    {
                        try
                        {
                            var modifications = Ui.ModifyTagMenu(mp3s);
                            Mp3TagsManager.ModifyMp3Tags(mp3s, modifications.Item1, modifications.Item2);
                            correctInput = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Incorrect input, please try again!");
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