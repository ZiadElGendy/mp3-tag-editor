using System.Runtime.Serialization;

namespace MP3_Tag_Editor.Exceptions;

[Serializable]
public class InvalidSourcePathException : Exception
{
    public InvalidSourcePathException(string filePath)
        : base($"The file '{filePath}' is not a valid external source file.")
    {
    }

    public InvalidSourcePathException(string filePath, Exception innerException)
        : base($"The file '{filePath}' is not a valid external source file.", innerException)
    {
    }

    public InvalidSourcePathException(string filePath, string message)
        : base($"The file '{filePath}' is not a valid external source file. {message}")
    {
    }

    public InvalidSourcePathException(string filePath, string message, Exception innerException)
        : base($"The file '{filePath}' is not a valid external source file. {message}", innerException)
    {
    }

    protected InvalidSourcePathException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}