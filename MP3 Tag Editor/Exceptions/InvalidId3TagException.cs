using System.Runtime.Serialization;
using Id3;

namespace MP3_Tag_Editor.Exceptions;
[Serializable]
public class InvalidId3TagException : Exception
{
    public InvalidId3TagException()
        : base($"This tag is invalid.")
    {
    }

    public InvalidId3TagException(Mp3 mp3)
        : base($"The mp3 '{mp3}' has an invalid tag.")
    {
    }

    protected InvalidId3TagException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidId3TagException(string? message) : base($"This tag is invalid. {message}")
    {
    }

    public InvalidId3TagException(string? message, Exception? innerException) : base($"This tag is invalid. {message}", innerException)
    {
    }
}