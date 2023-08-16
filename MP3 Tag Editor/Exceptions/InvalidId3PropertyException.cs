using System.Runtime.Serialization;

namespace MP3_Tag_Editor.Exceptions;

[Serializable]
public class InvalidId3PropertyException : Exception
{
    public InvalidId3PropertyException()
    {
    }

    protected InvalidId3PropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidId3PropertyException(string? message) : base(message)
    {
    }

    public InvalidId3PropertyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}