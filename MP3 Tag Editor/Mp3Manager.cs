using Id3;

namespace MP3_Tag_Editor;

public static class Mp3Manager
{
    private static string workingFilePath; 
    private static List<Mp3> workingMp3Files;

    public static void ModifyMp3(string inputFilePath)
    {
        workingFilePath = inputFilePath;
        workingMp3Files = (List<Mp3>)Mp3FileManager.LoadMp3Files(workingFilePath);
    }
}