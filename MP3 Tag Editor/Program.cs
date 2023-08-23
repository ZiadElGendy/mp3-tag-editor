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
                case "e":
                    var path = Ui.ModifyMp3Menu();
                    var overwriteSelection = Ui.OverwriteMenu();
                    List<TagLib.File> mp3s;
                    if (overwriteSelection)
                        mp3s = Mp3FileManager.LoadMp3Files(path).ToList();
                    else
                        mp3s = Mp3FileManager.LoadNewMp3Files(path).ToList();

                    if (mp3s.Count == 1)
                    {

                    }
                    else
                    {

                    }



                    break;
                case "x":
                    _exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid input, please try again");
                    break;
            }
        }
    }
}