using System.Runtime.Serialization;
using Id3;

namespace MP3_Tag_Editor.Exceptions;
[Serializable]
public class InvalidId3TagException : Exception
{
    public InvalidId3TagException(Mp3 mp3)
        : base($"The mp3 '{mp3}' has an invalid tag.")
    {
    }

    public InvalidId3TagException(Mp3 mp3, string? message)
        : base($"The mp3 '{mp3}' has an invalid tag. {message}")
    {
    }

    public InvalidId3TagException(string? message, Exception? innerException)
        : base($"This tag is invalid. {message}", innerException)
    {
    }

    public InvalidId3TagException(Mp3 mp3, Exception? innerException)
        : base($"The mp3 '{mp3}' has an invalid tag.", innerException)
    {
    }

    public InvalidId3TagException(Mp3 mp3, string? message, Exception? innerException)
        : base($"The mp3 '{mp3}' has an invalid tag. {message}", innerException)
    {
    }

    protected InvalidId3TagException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

}