using System.Runtime.Serialization;

namespace MP3_Tag_Editor.Exceptions;
[Serializable]
public class InvalidMp3TagException : Exception
{
    public InvalidMp3TagException(TagLib.File mp3File)
        : base($"The mp3 '{mp3File}' has an invalid tag.")
    {
    }

    public InvalidMp3TagException(TagLib.File mp3File, string? message)
        : base($"The mp3 '{mp3File}' has an invalid tag. {message}")
    {
    }

    public InvalidMp3TagException(string? message, Exception? innerException)
        : base($"This tag is invalid. {message}", innerException)
    {
    }

    public InvalidMp3TagException(TagLib.File mp3File, Exception? innerException)
        : base($"The mp3 '{mp3File}' has an invalid tag.", innerException)
    {
    }

    public InvalidMp3TagException(TagLib.File mp3File, string? message, Exception? innerException)
        : base($"The mp3 '{mp3File}' has an invalid tag. {message}", innerException)
    {
    }

    protected InvalidMp3TagException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

}