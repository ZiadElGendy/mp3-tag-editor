using System.Runtime.Serialization;

namespace MP3_Tag_Editor.Exceptions;

[Serializable]
public class InvalidMp3PathException : Exception
{
    public InvalidMp3PathException(string filePath)
        : base($"The file '{filePath}' is not a valid MP3 file.")
    {
    }

    public InvalidMp3PathException(string filePath, Exception innerException)
        : base($"The file '{filePath}' is not a valid MP3 file.", innerException)
    {
    }

    public InvalidMp3PathException(string filePath, string message)
        : base($"The file '{filePath}' is not a valid MP3 file. {message}")
    {
    }

    public InvalidMp3PathException(string filePath, string message, Exception innerException)
        : base($"The file '{filePath}' is not a valid MP3 file. {message}", innerException)
    {
    }

    protected InvalidMp3PathException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}