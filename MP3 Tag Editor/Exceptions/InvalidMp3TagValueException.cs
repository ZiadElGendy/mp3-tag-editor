using System.Runtime.Serialization;
using MP3_Tag_Editor.Enums;

namespace MP3_Tag_Editor.Exceptions;
[Serializable]
public class InvalidMp3TagValueException : Exception
{
    public InvalidMp3TagValueException(dynamic value, Mp3Tag tag)
        : base($"Value type '{value.GetType()}' does not match the expected type for tag '{tag}'.'")
    {
    }

    public InvalidMp3TagValueException(dynamic value, Mp3Tag tag, string? message)
        : base($"Value type '{value.GetType()}' does not match the expected type for tag '{tag}'. {message}")
    {
    }

    public InvalidMp3TagValueException(string? message, Exception? innerException)
        : base($"This tag is invalid. {message}", innerException)
    {
    }

    public InvalidMp3TagValueException(dynamic value, Mp3Tag tag, Exception? innerException)
        : base($"Value type '{value.GetType()}' does not match the expected type for tag '{tag}'.", innerException)
    {
    }

    public InvalidMp3TagValueException(dynamic value, Mp3Tag tag, string? message, Exception? innerException)
        : base($"Value type '{value.GetType()}' does not match the expected type for tag '{tag}'. {message}", innerException)
    {
    }

    protected InvalidMp3TagValueException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

}