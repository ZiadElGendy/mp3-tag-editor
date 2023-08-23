namespace MP3_Tag_Editor;

internal static class Program
{
    private static bool _exit = false;
    private static void Main(string[] args)
    {
        while (!_exit)
        {
            var input = Ui.MainMenu();
            switch (input)
            {
                case "h":
                    Ui.HelpMenu();
                    break;
                case "e":
                    Ui.ModifyMp3Menu();
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